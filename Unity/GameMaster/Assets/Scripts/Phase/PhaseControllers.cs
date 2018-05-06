using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// フェーズ：各端末操作
/// 
/// ＊スクリーン上で実況する（実況メッセージは余力あれば考えてもらう）
/// ＊各端末を操作する人は、スクリーンを見えないように配置する
/// ＊選んだ選択肢と、アクション内容の進捗を定期的にゲームマスターへ報告してもらう
/// 
/// </summary>
public class PhaseControllers : PhaseBase {

	/// <summary>
	/// メーター順序
	/// </summary>
	public enum PowerMeter {
		StartPower,
		FlightPower,
		LackPower,
		Count,
	}

	/// <summary>
	/// 操作端末のIPアドレス
	/// </summary>
	public static string[] ControllerIPAddresses = new string[] {
#if UNITY_EDITOR
		"127.0.0.1",
		"127.0.0.1",
		"127.0.0.1",
#else
		// TODO: GM開始時に設定を動的に変えられるようにしたい
		"192.168.11.11",
		"192.168.11.12",
		"192.168.11.13",
#endif
	};

	/// <summary>
	/// オーディエンス投票を監視する間隔秒数
	/// </summary>
	private const float AudienceWatchSeconds = 5.0f;

	/// <summary>
	/// オーディエンス投票を締め切る直前の待機秒数
	/// </summary>
	private const int ClosingAudienceWaitSeconds = 30;

	/// <summary>
	/// 操作端末に与えられるゲーム時間
	/// </summary>
	private const int ControllerLimitSeconds = 60;

	/// <summary>
	/// 各端末の進捗状況を更新する間隔
	/// </summary>
	private const float ControllerProgressRefreshSeconds = 1.0f;

	/// <summary>
	/// オーディエンス投票を締め切るまでの残り秒数
	/// </summary>
	private float closingAudienceRemainSeconds;

	/// <summary>
	/// 通信システム
	/// </summary>
	private NetworkGameMaster connector;

	/// <summary>
	/// イベントID
	/// </summary>
	private string eventId;

	/// <summary>
	/// オーディエンス投票情報として画面に出すテキスト
	/// </summary>
	private string postText;

	/// <summary>
	/// オーディエンス投票が通信障害によって利用できない状態になっているかどうか
	/// </summary>
	private bool isAudienceEventError;

	/// <summary>
	/// オーディエンス投票を締め切る直前に画面に出すテキスト
	/// </summary>
	private const string ClosingTextSource = @"${REMAIN_SECOND} 秒後に投票を締め切ります...
まだ投票できていない人は今のうちに済ませてね！";

	/// <summary>
	/// 画面上部のスクロール文字列の元のテキスト（平常運行時）
	/// </summary>
	private const string TopDescriptionSourceNormal = @"ただいま３名のプレイヤーが飛行の準備をしています。プレイ希望の方は次のゲームが始まるまでお待ち下さい（待ち時間はおよそ５分です）。観客の皆さんも結果を予想してみましょう！ スマホから画面のＱＲコードを読み込んで投票してみて下さい♪";

	/// <summary>
	/// 画面上部のスクロール文字列の元のテキスト（オーディエンス投票障害発生時）
	/// </summary>
	private const string TopDescriptionSourceError = @"ただいま３名のプレイヤーが飛行の準備をしています。プレイ希望の方は次のゲームが始まるまでお待ち下さい（待ち時間はおよそ５分です）。観客の皆さんも結果を予想をして頂きたいところですが、あいにくただいま障害発生中のため投票できません。";

	/// <summary>
	/// 各端末の操作が完了したかどうか
	/// </summary>
	private bool[] isControllerCompleted;

	/// <summary>
	/// 各端末の通信でエラーが発生しているかどうか
	/// </summary>
	private bool[] isControllerError;

	/// <summary>
	/// バックドアによって端末の操作内容が変更されたかどうか
	/// </summary>
	public static bool BackDoorOperated = false;

	/// <summary>
	/// 各端末の操作状況
	/// </summary>
	public static Dictionary<string, string>[] ControllerProgresses;

	/// <summary>
	/// 各端末の操作状況を更新するための Timer.deltaTime 加算用
	/// </summary>
	private float controllerProgressRefreshTimer;

	/// <summary>
	/// 実況テキストエリアの元文章
	/// </summary>
	private string textSource;

	/// <summary>
	/// 実況テキストエリアの現在表示している文字列インデックス
	/// </summary>
	private int textCursorIndex;

	/// <summary>
	/// 実況メッセージを格納するゲームオブジェクトのコンポーネント
	/// </summary>
	private UnityEngine.UI.Text textArea;

	/// <summary>
	/// 実行中のメッセージ送りコルーチン
	/// </summary>
	private Coroutine messageCoroutine;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	public PhaseControllers(PhaseManager parent) : base(parent, null) {
		this.connector = new NetworkGameMaster(PhaseControllers.ControllerIPAddresses);
	}

	/// <summary>
	/// ゲームオブジェクトの初期化
	/// </summary>
	public override void Start() {
		// ##### オーディエンス投票周り #####
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");
		closingWindow.gameObject.SetActive(false);
		this.parent.StartCoroutine(this.watcherAudience());

		// 新規投票イベント生成
		this.startAudienceEvent();

		// ##### 操作端末周り #####
		this.isControllerCompleted = new bool[PhaseControllers.ControllerIPAddresses.Length];
		this.isControllerError = new bool[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text = "";
			GameObject.Find("Controller_CompleteCheck" + i).transform.localScale = new Vector3(0, 0, 1);
			GameObject.Find("Controller_CompleteCheck" + i).GetComponent<UnityEngine.UI.Image>().enabled = false;
		}

		// 各端末に開始指示を出す
		PhaseControllers.ControllerProgresses = new Dictionary<string, string>[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			PhaseControllers.ControllerProgresses[i] = new Dictionary<string, string>();
			PhaseControllers.ControllerProgresses[i]["isStarted"] = "false";
			this.startController(i);
		}

		// 実況テキスト
		this.textArea = GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>();
		this.textArea.text = "";
		this.textSource = "";
		this.textCursorIndex = 0;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		// ##### 操作端末周り #####
		// すべての端末が終了したか判定する
		var isControllerCompleteAll = (this.isControllerCompleted.Where(a => a == false).Count() == 0);
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");

		// 端末の進捗状況
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			// 毎秒１回ずつ処理する
			this.controllerProgressRefreshTimer += Time.deltaTime;
			if(this.controllerProgressRefreshTimer >= PhaseControllers.ControllerProgressRefreshSeconds) {
				this.controllerProgressRefreshTimer = 0;

				// 操作状況を文字列化して表示
				if(PhaseControllers.BackDoorOperated == true || this.isControllerError[i] == false) {
					// 平常運転 or バックドアによって強制的に書き換えられた

					// 進捗状況のテキスト更新
					GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text =
						this.conrollerProgressToString(i, PhaseControllers.ControllerProgresses[i]);

					// 各種メーター更新
					this.setMetersByConrollerProgress(i, PhaseControllers.ControllerProgresses[i]);
				} else {
					// 障害発生中
					GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text = "＜＜障害発生中＞＞";
				}
			}

			// 完了したときのマーク表示
			var checkBox = GameObject.Find("Controller_CompleteCheck" + i);
			var checkBoxImage = checkBox.GetComponent<UnityEngine.UI.Image>();
			if(checkBoxImage.enabled != this.isControllerCompleted[i]) {
				checkBoxImage.enabled = this.isControllerCompleted[i];

				// チェックマークをiTween表示
				iTween.ScaleTo(checkBox, new Vector3(1, 1, 1), 1.0f);
			}
		}

		// 実況更新
		var afterText = this.getStreamText();
		var beforeText = this.textSource;
		this.textSource = afterText;
		if(afterText != beforeText) {
			// テキストが変更になったときにメッセージ送りを開始する
			GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>().text = "";
			if(this.messageCoroutine != null) {
				// 既に実行中の場合は中止する
				this.parent.StopCoroutine(this.messageCoroutine);
			}
			this.messageCoroutine = this.parent.StartCoroutine(this.nextMessageCharacter());
		} else {
			// 以前のテキストと同じときは引き続きメッセージ送りを行う
			GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>().text = this.textSource.Substring(0, this.textCursorIndex);
		}

		// ##### オーディエンス投票周り #####
		// 投票状況更新
		GameObject.Find("Controller_AudiencePostInformation").GetComponent<UnityEngine.UI.Text>().text = this.postText;

		// スクロール位置を常に一番上にする
		var scrollView = GameObject.Find("Controller_PostedAudience");
		scrollView.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1f;

		// オーディエンス投票締切直前カウントダウン表示
		if(isControllerCompleteAll == true) {
			if(this.isAudienceEventError == true) {

				// オーディエンス投票が障害発生中のときはすぐに次のフェーズへ移行する
				this.ChangeToFlightPhase();
				this.IsUpdateEnabled = false;

			} else {

				UnityEngine.UI.Text closingTextUI;

				if(closingWindow.gameObject.activeInHierarchy == false) {
					// オーディエンス投票締切直前カウントダウン開始
					closingWindow.gameObject.SetActive(true);
					this.closingAudienceRemainSeconds = PhaseControllers.ClosingAudienceWaitSeconds + 0.99f;

					// ウィンドウオープン
					closingWindow.localScale = Vector3.zero;
					iTween.ScaleTo(
						closingWindow.gameObject,
						new Vector3(1, 1, 1),
						1.0f
					);
				} else {
					// オーディエンス投票締切直前カウントダウン中
					this.closingAudienceRemainSeconds -= Time.deltaTime;
					if(this.closingAudienceRemainSeconds < 0) {
						// タイムアップ：次のフェーズへ移行
						this.ChangeToFlightPhase();
						this.IsUpdateEnabled = false;
						return;
					}
				}

				// 表示文字列を更新
				closingTextUI = GameObject.Find("Controller_ClosingAudienceText").GetComponent<UnityEngine.UI.Text>();
				closingTextUI.text = PhaseControllers.ClosingTextSource
					.Replace("${REMAIN_SECOND}", ((int)this.closingAudienceRemainSeconds).ToString());

			}
		}

		// ##### その他 #####
		// オーディエンス投票をやっているかどうかで上部スクロール文字列の中身を変える
		if(this.isAudienceEventError == false) {
			GameObject.Find("Controller_TopDescriptionText").GetComponent<UnityEngine.UI.Text>().text = PhaseControllers.TopDescriptionSourceNormal;
		} else {
			GameObject.Find("Controller_TopDescriptionText").GetComponent<UnityEngine.UI.Text>().text = PhaseControllers.TopDescriptionSourceError;
		}

#if UNITY_EDITOR
		// デバッグ時のみ有効な処理

		// 端末の操作を強制終了する
		if(Input.GetKeyDown(KeyCode.Alpha1) == true) {
			this.isControllerCompleted[0] = true;
			Debug.Log("端末A 終了");
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) == true) {
			this.isControllerCompleted[1] = true;
			Debug.Log("端末B 終了");
		}
		if(Input.GetKeyDown(KeyCode.Alpha3) == true) {
			this.isControllerCompleted[2] = true;
			Debug.Log("端末C 終了");
		}

#endif
	}

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public override void Destroy() {
		this.connector.CloseConnectionsAll();
		this.connector.CloseAudiencePredicts(this.eventId, null);
	}

	/// <summary>
	/// 新規オーディエンス投票イベントを生成します。
	/// </summary>
	private void startAudienceEvent() {
		this.connector.StartAudiencePredicts(new System.Action<string>((result) => {
			if(string.IsNullOrEmpty(result) == false) {
				// 生成されたイベントIDを保管
				this.eventId = result;
				Debug.Log("イベント開始: ID=" + result);
				this.isAudienceEventError = false;
			} else {
				// エラー発生時
				Debug.LogError("イベント開始失敗");
				this.isAudienceEventError = true;
			}
		}));
	}

	/// <summary>
	/// 操作端末へ開始指示を出します。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void startController(int roleId) {
		Debug.Log("開始指示: 端末ID=" + roleId + ", IPアドレス=" + PhaseControllers.ControllerIPAddresses[roleId]);

		this.connector.StartController(
			new ModelControllerStart() {
				RoleId = roleId,
				LimitTimeSecond = PhaseControllers.ControllerLimitSeconds,
			},
			roleId,
			new System.Action(() => {
				Debug.Log("接続開始成功: 端末ID=" + roleId);
				PhaseControllers.ControllerProgresses[roleId]["isStarted"] = "true";
				this.isControllerError[roleId] = false;

				// 進捗報告と完了報告を受け付ける
				this.watcherControllerProgresses(roleId);
				this.watcherControllerCompletes(roleId);
			}),
			new System.Action(() => {
				Debug.LogWarning("接続開始失敗: 端末ID=" + roleId);

				// 当該端末をエラー状態に変更
				this.isControllerError[roleId] = true;

				if(this.parent.PhaseIndex == PhaseManager.PhaseIndexMap[this.GetType()]) {
					// 現在のフェーズであるうちは自動でリトライを続ける
					this.startController(roleId);
				}
			})
		);
	}

	/// <summary>
	/// 操作端末の進捗報告を受け付けます。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void watcherControllerProgresses(int roleId) {
		this.connector.ReceiveControllerProgress(
			roleId,
			new System.Action<ModelDictionary<string, string>>((result) => {
				PhaseControllers.ControllerProgresses[roleId] = result.GetDictionary();

				if(this.isControllerCompleted[roleId] == false) {
					// 完了報告が届いていないうちは引き続き受け付ける
					this.watcherControllerProgresses(roleId);
				}
			}
		));
	}

	/// <summary>
	/// 操作端末の完了報告を受け付けます。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void watcherControllerCompletes(int roleId) {
		this.connector.WaitForControllers(
			roleId,
			new System.Action<ModelDictionary<string, string>>((result) => {
				this.isControllerCompleted[roleId] = true;
				PhaseControllers.ControllerProgresses[roleId] = result.GetDictionary();
			}
		));
	}

	/// <summary>
	/// メッセージの文字を１文字進めるコルーチン
	/// </summary>
	private IEnumerator nextMessageCharacter() {
		this.textCursorIndex = 0;

		while(true) {
			if(this.textCursorIndex + 1 > this.textSource.Length) {
				// 最後まで達したとき終了
				yield break;
			} else {
				// 先に遅延
				yield return new WaitForSeconds(PhaseManager.MessageSpeed);
			}

			// １文字進める
			this.textCursorIndex++;
			this.textArea.text = this.textSource.Substring(0, this.textCursorIndex);
		}
	}

	/// <summary>
	/// オーディエンス投票を監視するコルーチン
	/// </summary>
	private IEnumerator watcherAudience() {
		while(true) {
			yield return new WaitForSeconds(PhaseControllers.AudienceWatchSeconds);
			if(this.isAudienceEventError == true) {
				Debug.LogWarning("イベントIDが無効のため投票情報を取得できません。");
				this.postText = "＜＜障害発生中＞＞";
				continue;
			}

			// オーディエンス投票データを受け取る
			this.connector.GetAudiencePredicts(this.eventId, new System.Action<ModelAudiencePredictsResponse>((result) => {
				if(result == null) {
					// 取得失敗
					return;
				}
				Debug.Log("オーディエンス投票データ: レスポンス取得OK");

				// 取得時点では昇順だが、表示領域の関係で逆順に直す場合は以下をコメントアウト解除すること
				result.audiencePredicts.Reverse();

				// 表示用文字列を生成
				using(var str = new System.IO.StringWriter()) {
					foreach(var post in result.audiencePredicts) {
						str.WriteLine(post.receiveTime.Substring(11) + "  " + post.nickname + " さんが投票しました");
					}
					this.postText = str.ToString();
				}
			}));
		}
	}

	/// <summary>
	/// 操作端末の進捗情報を文字列にします。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	/// <param name="progress">操作端末の進捗情報</param>
	/// <returns>文字列化された進捗情報</returns>
	private string conrollerProgressToString(int roleId, Dictionary<string, string> progress) {
		if(progress == null) {
			return "";
		}

		// TODO: それぞれのI/F仕様に応じた表示
		var isStarted = (progress.ContainsKey("isStarted") == false || bool.Parse(progress["isStarted"]));
		using(var buf = new System.IO.StringWriter()) {
			// バリデーション
			if(progress.ContainsKey("roleId") == true && int.Parse(progress["roleId"]) != roleId) {
				Debug.LogError("GMで管理している役割IDと操作端末が申告してきた役割IDが一致しません。");
			}

			switch(roleId) {
				case (int)NetworkConnector.RoleIds.A_Prepare:
					buf.WriteLine("【助走役：○○ちゃん】");
					if(isStarted == true) {
						buf.WriteLine("　助走方法：");
						buf.WriteLine("　ＰＯＷＥＲ：");
					}
					break;

				case (int)NetworkConnector.RoleIds.B_Flight:
					buf.WriteLine("【飛行役：○○ちゃん】");
					if(isStarted == true) {
						buf.WriteLine("　ＰＯＷＥＲ：");
					}
					break;

				case (int)NetworkConnector.RoleIds.C_Assist:
					buf.WriteLine("【サポート役：○○ちゃん】");
					if(isStarted == true) {
						buf.WriteLine("　サポート方法：");
						buf.WriteLine("　？？？：");
					}
					break;
			}

			if(isStarted == false) {
				// まだ開始指示を受け取っていない端末
				buf.WriteLine("　開始準備中...");
			}

#if UNITY_EDITOR
			// デバッグ用
			foreach(var item in progress) {
				buf.Write(item.Key + "=" + item.Value + ", ");
			}
#endif

			return buf.ToString();
		}
	}

	/// <summary>
	/// 操作端末の進捗情報を基に性能バランス値を各種メーターにセットします。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	/// <param name="progress">操作端末の進捗情報</param>
	private void setMetersByConrollerProgress(int roleId, Dictionary<string, string> progress) {
		int option;
		var values = new float[(int)PowerMeter.Count];

		var isStarted = (progress.ContainsKey("isStarted") == false || bool.Parse(progress["isStarted"]));
		if(isStarted == false) {
			// まだ始まっていないときは初期状態を維持する
			values[(int)PowerMeter.StartPower] = 0;
			values[(int)PowerMeter.FlightPower] = 0;
			values[(int)PowerMeter.LackPower] = 0;
			return;
		}

		switch(roleId) {
			case (int)NetworkConnector.RoleIds.A_Prepare:
				// 選択肢に応じて基本バランスが変わる
				option = int.Parse(progress["option"]);
				switch(option) {
					case 0:
						// 人力
						values[(int)PowerMeter.StartPower] = 0.1f + int.Parse(progress["param"]) / 0.4f;
						values[(int)PowerMeter.FlightPower] = 0;
						values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / 0.5f;
						break;

					case 1:
						// 牽引
						values[(int)PowerMeter.StartPower] = 0.4f + int.Parse(progress["param"]) / 0.4f;
						values[(int)PowerMeter.FlightPower] = 0;
						values[(int)PowerMeter.LackPower] = 0.1f + int.Parse(progress["param"]) / 0.4f;
						break;

					case 2:
						// 爆弾
						values[(int)PowerMeter.StartPower] = 1.0f;
						values[(int)PowerMeter.FlightPower] = 0.2f + int.Parse(progress["param"]) / 0.8f;
						values[(int)PowerMeter.LackPower] = 0;
						break;
				}
				break;

			case (int)NetworkConnector.RoleIds.B_Flight:
				// 基本バランスは固定で、スコアに応じて増減する
				values[(int)PowerMeter.StartPower] = 0;
				values[(int)PowerMeter.FlightPower] = int.Parse(progress["param"]) / 1.0f;
				values[(int)PowerMeter.LackPower] = 0;
				break;

			case (int)NetworkConnector.RoleIds.C_Assist:
				// 選択肢に応じて基本バランスが変わる
				option = int.Parse(progress["option"]);
				switch(option) {
					case 0:
						// 応援
						values[(int)PowerMeter.StartPower] = 0;
						values[(int)PowerMeter.FlightPower] = 0.1f + int.Parse(progress["param"]) / 0.25f;
						values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / 0.3f;
						break;

					case 1:
						// 爆弾
						values[(int)PowerMeter.StartPower] = 0;
						values[(int)PowerMeter.FlightPower] = 0.2f + int.Parse(progress["param"]) / 0.5f;
						values[(int)PowerMeter.LackPower] = 0;
						break;

					case 2:
						// 賄賂
						values[(int)PowerMeter.StartPower] = 0;
						values[(int)PowerMeter.FlightPower] = 0;
						values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / 0.8f;
						break;
				}
				break;
		}

		// メーターにセット
		var meters = GameObject.Find("Controller_Meters" + roleId).transform;
		for(int i = 0; i < meters.childCount; i++) {
			meters.GetChild(i).GetComponent<Meter>().SetValue(values[i]);
		}
	}

	/// <summary>
	/// 現在の端末操作状況を基に、実況テキストを返します。
	/// </summary>
	/// <returns>実況テキスト</returns>
	private string getStreamText() {
		string buf = "";

		// TODO: 状況に応じた分岐
		buf = "実況テキスト";

		return buf;
	}

	/// <summary>
	/// 端末操作フェーズを終えて飛行フェーズへ移行します。
	/// </summary>
	public void ChangeToFlightPhase() {
		this.parent.ChangePhase(new PhaseFlight(this.parent, new object[] {
			this.eventId,
			PhaseControllers.ControllerProgresses[0],
			PhaseControllers.ControllerProgresses[1],
			PhaseControllers.ControllerProgresses[2],
		}));
	}

}
