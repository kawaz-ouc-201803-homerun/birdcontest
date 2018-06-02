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
	/// 地点表示を行う時間秒数
	/// </summary>
	public const float MapAddressShowTimeSeconds = 5.0f;

	/// <summary>
	/// 飛距離の滑らかな加算にかける時間秒数
	/// </summary>
	public const float ScoreEaseTimeSeconds = 2.0f;

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
	/// 実況メッセージを格納するゲームオブジェクトのコンポーネント
	/// </summary>
	private UnityEngine.UI.Text textArea;

	/// <summary>
	/// 舞台背景カメラワークの材料
	/// </summary>
	private List<GameObject> backgroundSapporoObjects;

	/// <summary>
	/// 3DObject_Kasaharaオブジェクト
	/// ＊プレハブから動的生成します。
	/// </summary>
	private GameObject flight3DObjects;

	/// <summary>
	/// FlightOpeningCameraオブジェクト
	/// ＊プレハブから動的生成します。
	/// </summary>
	private GameObject flightOpeningCamera;

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
	/// 着地が完了したかどうか
	/// </summary>
	private bool isFlightLanding = false;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="parameters">[0]=イベントID, [1]=操作端末の結果データの配列</param>
	public PhaseFlight(PhaseManager parent, object[] parameters) : base(parent, parameters) {
		this.score = 0;
		this.isFlightLanding = false;
		this.mapAddress = "";
		this.eventId = parameters[0] as string;
		this.controllerData = parameters[1] as Dictionary<string, string>[];
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public override void Start() {
		// 舞台背景カメラワークを無効化
		this.backgroundSapporoObjects = new List<GameObject>(new GameObject[] {
			GameObject.Find("BackgroundSapporoCamera"),
			GameObject.Find("BackgroundSapporoMap"),
		});
		this.backgroundSapporoObjects.ForEach(
			(obj) => obj.SetActive(false)
		);

		// 再利用されるオブジェクト群（古いもの）を取得しておく
		var previousRespawnObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Respawn"));

		// 飛行フェーズの3Dオブジェクトを生成
		var previousFlight3DObjects = previousRespawnObjects.Find((obj) => obj.name.IndexOf("3DObjects_Kasahara") != -1);
		if(previousFlight3DObjects != null) {
			// 先に前回のオブジェクトを削除する
			Debug.LogWarning("以前の [3DObjects_Kasahara] オブジェクトが残っているため、削除します");
			GameObject.Destroy(previousFlight3DObjects);
		}
		this.flight3DObjects = GameObject.Instantiate<GameObject>(
			Resources.Load<GameObject>("Prefabs/3DObjects_Kasahara"),
			Vector3.zero,
			Quaternion.identity,
			null
		);
		this.flight3DObjects.GetComponent<DataContainerDebugger>().enabled = false;

		// 操作端末の結果データを引き継ぐ
		this.flight3DObjects.GetComponent<DataContainer>().Setup(this.controllerData);
		this.setScore(0, true);

		// 着地したときのイベントハンドラーをセット
		this.flight3DObjects.GetComponent<LandingJudge>().LandingEvent.AddListener(this.EndFlight);

		// 実況テキスト準備
		this.textArea = GameObject.Find("Flight_StreamWindow").transform.Find("Text").GetComponent<UnityEngine.UI.Text>();

		// オープニングカメラワークを生成して開始
		var previousOpeningCameraWorks = previousRespawnObjects.Find((obj) => obj.name.IndexOf("FlightOpeningCamera") != -1);
		if(previousOpeningCameraWorks != null) {
			// 先に前回のオブジェクトを削除する
			Debug.LogWarning("以前の [FlightOpeningCamera] オブジェクトが残っているため、削除します");
			GameObject.Destroy(previousOpeningCameraWorks);
		}
		this.flightOpeningCamera = GameObject.Instantiate<GameObject>(
			Resources.Load<GameObject>("Prefabs/FlightOpeningCamera"),
			Vector3.zero,
			Quaternion.identity,
			null
		);
		var openingCameraController = this.flightOpeningCamera.GetComponent<OpeningCameraController>();
		openingCameraController.CameraSwitcher = this.flight3DObjects.transform.Find("MainPlane").gameObject.GetComponent<CameraSwitcher>();
		openingCameraController.StartOpeningCameraWorks();
		openingCameraController.ReadyForStartEvent.AddListener(new UnityEngine.Events.UnityAction(() => {
			// オープニングが終わったら開始する
			this.parent.StartCoroutine(this.readyGo());
		}));
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		// 飛距離の表示
		if(this.isFlightLanding == false) {
			this.setScore(this.flight3DObjects.GetComponent<DistanceCalculator>().Distance, true);
		}

		// 現在地点の表示
		if(this.flight3DObjects.GetComponent<DataContainer>().ControllerBPlane != null) {
			string afterMapAddress = this.getMapAddress(
				this.flight3DObjects.GetComponent<DataContainer>().ControllerBPlane.transform.position
			);
			if(this.mapAddress != afterMapAddress) {
				// 変更があったときに一時的に表示する
				if(this.mapAddressCoroutine != null) {
					this.parent.StopCoroutine(this.mapAddressCoroutine);
				}
				this.mapAddress = afterMapAddress;
				this.mapAddressCoroutine = this.parent.StartCoroutine(this.showMapAddress());
			}
		}

		if(this.isStreamStarted == false) {
			// 実況テキスト送りを開始
			this.isStreamStarted = true;
			this.messageCoroutine = this.parent.StartCoroutine(this.nextStreamTextCharacter());
		}
	}

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public override void Destroy() {
		// 舞台背景カメラワークを復元
		this.backgroundSapporoObjects.ForEach(
			(obj) => obj.SetActive(true)
		);

		// 動的生成したオブジェクトをすべて破棄
		GameObject.Destroy(this.flight3DObjects);
		GameObject.Destroy(this.flightOpeningCamera);

		// [Finish] 表示を消去
		GameObject.Find("FlightPhase").transform.Find("Flight_Finish").gameObject.SetActive(false);

		// コルーチン停止
		if(this.messageCoroutine != null) {
			this.parent.StopCoroutine(this.messageCoroutine);
		}
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
	/// 現在地点名称表示を行うコルーチン
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
				"time", PhaseFlight.MapAddressShowTimeSeconds,
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
				"time", PhaseFlight.MapAddressShowTimeSeconds,
				"easeType", iTween.EaseType.easeOutQuart,
				"isLocal", true
			)
		);
		yield return new WaitForSeconds(3.0f);
	}

	/// <summary>
	/// 座標から大通付近の住所を割り出します。
	/// </summary>
	/// <param name="position">飛行機の座標</param>
	/// <returns>住所テキスト</returns>
	private string getMapAddress(Vector3 position) {
		if(position.x >= -192f) {
			return "札幌市 大通公園";
		} else if(position.x >= -465f) {
			return "大通西５丁目";
		} else if(position.x >= -744f) {
			return "大通西４丁目";
		} else if(position.x >= -1017f) {
			return "大通西３丁目";
		} else if(position.x >= -1300f) {
			return "大通西２丁目";
		} else {
			// 西１丁目だけど、切れてる
			return "テレビ塔がない...";
		}
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

		switch(this.flight3DObjects.GetComponent<StreamTextStepController>().CurrentFlightGameStep) {
			case StreamTextStepController.FlightStep.Opening:
				return @"いよいよフライトの時間がやってまいりました！
このチームは一体どのようなフライトを見せてくれるのでしょうか！";

			case StreamTextStepController.FlightStep.Preparing:
				switch(optionA) {
					case (int)PhaseControllers.OptionA.Bomb:
						return @"爆弾に着火している……！
リスキーな手段ですが……一体どうなってしまうのか！？";

					case (int)PhaseControllers.OptionA.Human:
						return @"ここはオーソドックスな人力加速！
やはり最後に頼れるのは人間の力だということを証明してくれるのかっ！";

					case (int)PhaseControllers.OptionA.Car:
						return @"暴走族のごとくアクセルを吹かして――ぐんっと加速！
さすが車は馬力が違う！";
				}
				return @"";

			case StreamTextStepController.FlightStep.StartFlight:
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

			case StreamTextStepController.FlightStep.Flighting:
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

			case StreamTextStepController.FlightStep.StartSupport:
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

			case StreamTextStepController.FlightStep.EndSupport:
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

			case StreamTextStepController.FlightStep.EndFlight:
				if(optionA == (int)PhaseControllers.OptionA.Bomb && optionC == (int)PhaseControllers.OptionC.Bomb) {
					return @"おおーっと！ 二度目の爆弾は流石に耐えきれなかった！
バカの一つ覚えは行けませんねェ！";
				} else {
					return @"ここで着地、見事なフライトでした！";
				}
		}

		return @"";
	}

	/// <summary>
	/// 開始合図を出し、実際に飛行を開始させるコルーチン
	/// </summary>
	private IEnumerator readyGo() {
		yield return new WaitForSeconds(1.0f);

		// [READY-GO] 表示
		this.parent.SystemSEPlayer.PlaySEWithEcho((int)SystemSEPlayer.SystemSEID.Ready);
		var readyGoContainer = GameObject.Find("FlightPhase").transform.Find("Flight_ReadyGo");
		var animator = readyGoContainer.Find("Background/Flight_ReadyGoText").gameObject.GetComponent<Animator>();
		readyGoContainer.gameObject.SetActive(true);
		animator.enabled = true;
		readyGoContainer.localScale = Vector3.zero;
		iTween.ScaleTo(
			readyGoContainer.gameObject,
			new Vector3(1, 1, 1),
			1.0f
		);
		yield return new WaitForSeconds(1.0f);

		// アニメーションで [GO] に文字を切り替え
		yield return new WaitForSeconds(1.5f);
		this.parent.SystemSEPlayer.PlaySEWithEcho((int)SystemSEPlayer.SystemSEID.Go);
		yield return new WaitForSeconds(1.0f);
		iTween.ScaleTo(
			readyGoContainer.gameObject,
			new Vector3(0, 0, 0),
			1.0f
		);
		yield return new WaitForSeconds(1.0f);
		animator.enabled = false;

		// 飛行開始
		this.flight3DObjects.GetComponent<FlightStarter>().DoFlightStart();
	}

	/// <summary>
	/// 飛行を終了させるコルーチン
	/// </summary>
	private IEnumerator endFlight() {
		yield return new WaitForSeconds(1.0f);
		this.isFlightLanding = true;

		if(int.Parse(this.controllerData[(int)NetworkConnector.RoleIds.C_Assist]["option"]) == (int)PhaseControllers.OptionC.Wairo) {
			// 賄賂演出を行う
			this.parent.SystemSEPlayer.PlaySE((int)SystemSEPlayer.SystemSEID.WairoScoreUp);
			var ratio = this.flight3DObjects.GetComponent<DataContainer>().ParamC / 100f;
			var newScore = this.score * ratio;
			this.setScore(newScore);

			// 実況更新
			this.flight3DObjects.GetComponent<StreamTextStepController>().CurrentFlightGameStep = StreamTextStepController.FlightStep.EndSupport;

			yield return new WaitForSeconds(PhaseFlight.ScoreEaseTimeSeconds);
			yield return new WaitForSeconds(2.0f);
		}

		// 実況更新
		this.flight3DObjects.GetComponent<StreamTextStepController>().CurrentFlightGameStep = StreamTextStepController.FlightStep.EndFlight;

		// [FINISH] 表示
		this.parent.SystemSEPlayer.PlaySE((int)SystemSEPlayer.SystemSEID.FlightEnd);
		var finishContainer = GameObject.Find("FlightPhase").transform.Find("Flight_Finish");
		finishContainer.gameObject.SetActive(true);
		finishContainer.localScale = Vector3.zero;
		iTween.ScaleTo(
			finishContainer.gameObject,
			new Vector3(1, 1, 1),
			1.0f
		);
		yield return new WaitForSeconds(2.0f);

		// スコアに応じた評価アナウンス
		if(this.score < 200) {
			// 低評価
			this.parent.SystemSEPlayer.PlaySEWithEcho((int)SystemSEPlayer.SystemSEID.Evaluation1);
		} else if(this.score < 450) {
			// 中評価
			this.parent.SystemSEPlayer.PlaySEWithEcho((int)SystemSEPlayer.SystemSEID.Evaluation2);
		} else {
			// 高評価
			this.parent.SystemSEPlayer.PlaySEWithEcho((int)SystemSEPlayer.SystemSEID.Evaluation3);
		}
		yield return new WaitForSeconds(3.0f);

		// 次のフェーズへ移行する
		if(this.parent.Phase == this) {
			// 先にEnterキーで進められた場合はスキップ
			this.parent.ChangePhase(this.GetNextPhase());
		}
	}

	/// <summary>
	/// 飛行を終了します。
	/// </summary>
	private void EndFlight() {
		this.parent.StartCoroutine(this.endFlight());
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

	/// <summary>
	/// このフェーズのBGMファイル名を返します。
	/// </summary>
	/// <returns>BGMファイル名</returns>
	public override string GetBGMFileName() {
		return "Sounds/BGM/AC【飛行】shoujokyoku";
	}

}
