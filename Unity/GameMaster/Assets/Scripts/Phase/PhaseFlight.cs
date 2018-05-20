using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：飛行本番
/// 
/// ＊各端末の操作結果を踏まえて、実際に走らせる
/// 
/// </summary>
public class PhaseFlight : PhaseBase {

	/// <summary>
	/// 飛行フェーズのステップ
	/// </summary>
	public enum FlightGameStep {
		BeforePrepare,      // 仕込み前
		Preparing,          // 仕込み開始後
		StartFlight,        // 飛行開始
		Flighting,          // 飛行中間
		StartSupport,       // 援護開始
		EndSupport,         // 援護終了
		EndFlight,          // 飛行着地
	}

	/// <summary>
	/// 地点表示を行う秒数
	/// </summary>
	public const float MapAddressShowTimeSecond = 3.0f;

	/// <summary>
	/// 飛距離
	/// </summary>
	private float score;

	/// <summary>
	/// 現在地点の名称
	/// </summary>
	private string mapAddress;

	/// <summary>
	/// 実況テキスト送りを開始したかどうか
	/// </summary>
	private bool isStreamStarted;

	/// <summary>
	/// 実況テキストの現在表示している文字列インデックス
	/// </summary>
	private int textCursorIndex;

	/// <summary>
	/// 実況テキスト
	/// </summary>
	private string streamTextBuffer;

	/// <summary>
	/// 最後に読み取った実況テキスト
	/// </summary>
	private static string lastStreamText;

	/// <summary>
	/// 実況のステップ
	/// これが外部から変更されたとき、直後のフレームで実況テキストが更新されます。
	/// </summary>
	public static FlightGameStep StreamStep;

	/// <summary>
	/// 実況メッセージを格納するゲームオブジェクトのコンポーネント
	/// </summary>
	private UnityEngine.UI.Text textArea;

	/// <summary>
	/// 機体オブジェクト
	/// </summary>
	private GameObject airplane;

	/// <summary>
	/// 地点表示コルーチン
	/// </summary>
	private Coroutine mapAddressCoroutine;

	/// <summary>
	/// 実況テキスト送りコルーチン
	/// </summary>
	private Coroutine messageCoroutine;

	/// <summary>
	/// オーディエンス投票イベントID
	/// </summary>
	private string eventId;

	/// <summary>
	/// 操作端末の結果データの配列
	/// </summary>
	private Dictionary<string, string>[] controllerData;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="parameters">[0]=イベントID, [1]=操作端末の結果データの配列</param>
	public PhaseFlight(PhaseManager parent, object[] parameters) : base(parent, parameters) {
		this.score = 0;
		this.mapAddress = "";
		this.eventId = parameters[0] as string;
		this.controllerData = parameters[1] as Dictionary<string, string>[];
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public override void Start() {
		// メインカメラのアニメーション（舞台背景カメラワーク）を無効化
		GameObject.Find("Main Camera").GetComponent<Animator>().enabled = false;

		// TODO: 機体オブジェクトを保管
		this.airplane = GameObject.Find("Airplane");
		GameObject.Find("Flight_ScoreText").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00} m", 0);
		this.setScore(123.45f);

		// 実況テキスト準備
		PhaseFlight.StreamStep = FlightGameStep.BeforePrepare;
		this.textArea = GameObject.Find("Flight_StreamWindow").transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		// TODO: 距離計算して setScore(..., true); する

		// 現在地点の表示
		string afterMapAddress = this.getMapAddress(this.airplane.transform.position);
		if(this.mapAddress != afterMapAddress) {
			// 変更があったときに一時的に表示する
			if(this.mapAddressCoroutine != null) {
				this.parent.StopCoroutine(this.mapAddressCoroutine);
			}
			this.mapAddress = afterMapAddress;
			this.mapAddressCoroutine = this.parent.StartCoroutine(this.showMapAddress());
		}

		if(this.isStreamStarted == false) {
			// 実況テキスト送りを開始
			this.isStreamStarted = true;
			this.messageCoroutine = this.parent.StartCoroutine(this.nextStreamTextCharacter());
		}

#if UNITY_EDITOR
		// デバッグ用の機能
		if(Input.GetKeyDown(KeyCode.Alpha1) == true) {
			PhaseFlight.StreamStep = FlightGameStep.BeforePrepare;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) == true) {
			PhaseFlight.StreamStep = FlightGameStep.Preparing;
		}
		if(Input.GetKeyDown(KeyCode.Alpha3) == true) {
			PhaseFlight.StreamStep = FlightGameStep.StartFlight;
		}
		if(Input.GetKeyDown(KeyCode.Alpha4) == true) {
			PhaseFlight.StreamStep = FlightGameStep.Flighting;
		}
		if(Input.GetKeyDown(KeyCode.Alpha5) == true) {
			PhaseFlight.StreamStep = FlightGameStep.StartSupport;
		}
		if(Input.GetKeyDown(KeyCode.Alpha6) == true) {
			PhaseFlight.StreamStep = FlightGameStep.EndSupport;
		}
		if(Input.GetKeyDown(KeyCode.Alpha7) == true) {
			PhaseFlight.StreamStep = FlightGameStep.EndFlight;
		}
#endif
	}

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public override void Destroy() {
		// メインカメラのアニメーション（舞台背景カメラワーク）を有効化
		GameObject.Find("Main Camera").GetComponent<Animator>().enabled = true;

		// コルーチン停止
		this.parent.StopCoroutine(this.messageCoroutine);
		if(this.mapAddressCoroutine != null) {
			this.parent.StopCoroutine(this.mapAddressCoroutine);
		}
	}

	/// <summary>
	/// 飛距離をセットします。
	/// </summary>
	/// <param name="score">飛距離</param>
	/// <param name="disableEase">iTweenによる滑らかな増減を無効にするかどうか</param>
	private void setScore(float score, bool disableEase = false) {
		var beforeScore = this.score;
		this.score = score;

		// １フレームごとの更新処理内容
		var updateCallback = new Action<object>((value) => {
			GameObject.Find("Flight_ScoreText").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00} m", (float)value);
		});

		if(disableEase == true) {
			// 直接変更
			updateCallback(score);
		} else {
			// 滑らかに変更
			iTween.ValueTo(
				GameObject.Find("Flight_ScoreText"),
				iTween.Hash(
					"from", beforeScore,
					"to", score,
					"time", 2.0f,
					"easeType", iTween.EaseType.linear,
					"onupdate", updateCallback
				)
			);
		}
	}

	/// <summary>
	/// 現在地点名称表示
	/// </summary>
	private IEnumerator showMapAddress() {
		var window = GameObject.Find("Flight_AddressWindow");

		// 座標と透明度を変化させながら表示する
		GameObject.Find("Flight_AddressText").GetComponent<UnityEngine.UI.Text>().text = this.mapAddress;
		window.transform.position = new Vector3(
			1400f,
			window.transform.position.y,
			window.transform.position.z
		);
		iTween.MoveTo(
			window,
			iTween.Hash(
				"x", 960f,
				"time", PhaseFlight.MapAddressShowTimeSecond,
				"easeType", iTween.EaseType.easeOutQuart,
				"isLocal", true
			)
		);
		yield return new WaitForSeconds(3.0f);

		// 一定時間表示した後、自動的に非表示にする
		yield return new WaitForSeconds(3.0f);

		iTween.MoveTo(
			window,
			iTween.Hash(
				"x", 1400f,
				"time", PhaseFlight.MapAddressShowTimeSecond,
				"easeType", iTween.EaseType.easeOutQuart,
				"isLocal", true
			)
		);
		yield return new WaitForSeconds(3.0f);
	}

	/// <summary>
	/// 座標から大通付近の住所を割り出します。
	/// </summary>
	/// <param name="position">座標</param>
	/// <returns>住所テキスト</returns>
	private string getMapAddress(Vector3 position) {

		// TODO: 座標に応じて住所割り出し

		return "札幌市 大通公園";
	}

	/// <summary>
	/// メッセージの文字を１文字進めるコルーチン
	/// </summary>
	private IEnumerator nextStreamTextCharacter() {
		while(true) {
			// 実況メッセージを取得
			var newStreamText = this.getStreamText();
			if(this.streamTextBuffer != newStreamText) {
				// 実況メッセージが更新されている場合は初期化
				this.streamTextBuffer = newStreamText;
				this.textCursorIndex = 0;
			}

			if(this.textCursorIndex + 1 > this.streamTextBuffer.Length) {
				// 最後まで達したとき：停止
				yield return new WaitForSeconds(0.1f);
				continue;
			} else {
				// 先に遅延
				yield return new WaitForSeconds(PhaseManager.MessageSpeed);
			}

			// １文字進める
			this.textCursorIndex++;
			this.textArea.text = this.streamTextBuffer.Substring(0, this.textCursorIndex);
		}
	}

	/// <summary>
	/// 現在の実況ステップに対応する実況テキストを返します。
	/// </summary>
	/// <returns>実況テキスト</returns>
	private string getStreamText() {
		var optionA = int.Parse(this.controllerData[(int)NetworkConnector.RoleIds.A_Prepare]["option"]);
		var paramB = int.Parse(this.controllerData[(int)NetworkConnector.RoleIds.B_Flight]["param"]);
		var optionC = int.Parse(this.controllerData[(int)NetworkConnector.RoleIds.C_Assist]["option"]);

		switch(PhaseFlight.StreamStep) {
			case FlightGameStep.BeforePrepare:
				return @"いよいよフライトの時間がやってまいりました！
このチームは一体どのようなフライトを見せてくれるのでしょうか！";

			case FlightGameStep.Preparing:
				switch(optionA) {
					case (int)PhaseControllers.OptionA.Bomb:
						return @"爆弾を積み上げて着火したようです！
リスキーな手段ですが…おおっと、よく見ればパイロットが衝撃で気絶しているっ！？";

					case (int)PhaseControllers.OptionA.Human:
						return @"ここはオーソドックスな人力加速！
やはり最後に頼れるのは人間の力だということを証明してくれるのかっ！";

					case (int)PhaseControllers.OptionA.Car:
						return @"暴走族のごとくアクセルを吹かして――ぐんっと加速！
さすが車は馬力が違う！";
				}
				return @"";

			case FlightGameStep.StartFlight:
				switch(optionA) {
					case (int)PhaseControllers.OptionA.Bomb:
						return @"ここで爆弾が炸裂ゥ！
お、おや……なんと、爆発の衝撃でパイロットの意識が飛んでいる！";

					case (int)PhaseControllers.OptionA.Human:
						return @"助走をつけて難なくスタートしていく！
しかしこの初速でどこまで飛べるのか……！？";

					case (int)PhaseControllers.OptionA.Car:
						return @"乗り物×乗り物のパワーでぐんぐん加速していく！
なお会場にタイヤ痕をつけたため後程罰金されるそうですね……。";
				}
				return @"";

			case FlightGameStep.Flighting:
				if(paramB * 100f / PhaseControllers.ControllerLimitSeconds >= 70) {
					return @"かなり調子よく飛んでいく！
これは期待大ですね…！";
				} else if(paramB * 100f / PhaseControllers.ControllerLimitSeconds >= 40) {
					return @"平均的な滑空を見せていく！
それだけに、この後のサポートの成否が試されます！";
				} else {
					return @"他のチームよりも少し機体が安定しません…！
ここは飛行中のサポートに期待したいところです！";
				}

			case FlightGameStep.StartSupport:
				switch(optionC) {
					case (int)PhaseControllers.OptionC.Bomb:
						return @"ここで飛行中のサポートに回る……
おや、おもむろに爆弾を取り出しました！？";

					case (int)PhaseControllers.OptionC.Human:
						return @"急にチームメンバーが応援をして気合を注入しだした！
果たして効果があるのでしょうか！";

					case (int)PhaseControllers.OptionC.Wairo:
						return @"なぜか飛行中のサポートに動き出す様子がない！
何らかのトラブルでしょうか！？";
				}
				return @"";

			case FlightGameStep.EndSupport:
				switch(optionC) {
					case (int)PhaseControllers.OptionC.Bomb:
						return @"無慈悲に起爆！
さすがの推進力のようですが、機体の損傷は大丈夫なのかーっ！";

					case (int)PhaseControllers.OptionC.Human:
						return @"気絶した者さえ起こしてしまうほど熱のある応援！
さあ、ラストスパートに向けて引き続きフライトは続きます！";

					case (int)PhaseControllers.OptionC.Wairo:
						return @"これは！ 計測されている飛距離と運営側が発表している飛距離に食い違いが生まれている！？
一体何が起こったのでしょうか！";
				}
				return @"";

			case FlightGameStep.EndFlight:
				if(optionA == (int)PhaseControllers.OptionA.Bomb && optionC == (int)PhaseControllers.OptionC.Bomb) {
					return @"ここで着地、見事なフライトでした！";
				} else {
					return @"おおーっと！ 二度目の爆弾は流石に耐えきれなかった！
バカの一つ覚えは行けませんねェ！";
				}
		}

		return @"";
	}

	/// <summary>
	/// 前のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>前のフェーズのインスタンス</returns>
	public override PhaseBase GetPreviousPhase() {
		return new PhaseControllers(this.parent);
	}

	/// <summary>
	/// 次のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>次のフェーズのインスタンス</returns>
	public override PhaseBase GetNextPhase() {
		return new PhaseResult(this.parent, new object[] {
			this.eventId,
			this.score,
		});
	}

}
