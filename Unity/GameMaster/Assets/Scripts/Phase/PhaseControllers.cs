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
	/// オーディエンス投票を締め切る直前に画面に出すテキスト
	/// </summary>
	private string closingText;

	/// <summary>
	/// 各端末の操作が完了したかどうか
	/// </summary>
	private bool[] isControllerCompleted;

	/// <summary>
	/// 各端末の操作状況
	/// </summary>
	private Dictionary<string, string>[] controllerProgresses;

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
		this.connector.StartAudiencePredicts(new System.Action<string>((result) => {
			// 生成されたイベントIDを保管
			this.eventId = result;
			Debug.Log("イベント開始: ID=" + result);
		}));

		// 各端末に開始指示を出す
		this.controllerProgresses = new Dictionary<string, string>[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			this.controllerProgresses[i] = new Dictionary<string, string>();
			this.controllerProgresses[i]["isStarted"] = "false";
			this.startController(i);
		}
	}

	/// <summary>
	/// ゲームオブジェクトの初期化
	/// </summary>
	public override void Start() {
		// ##### オーディエンス投票周り #####
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");
		closingWindow.gameObject.SetActive(false);
		this.parent.StartCoroutine(this.watcherAudience());

		// ##### 操作端末周り #####
		this.isControllerCompleted = new bool[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text = "";
			GameObject.Find("Controller_CompleteCheck" + i).transform.localScale = new Vector3(0, 0, 1);
			GameObject.Find("Controller_CompleteCheck" + i).GetComponent<UnityEngine.UI.Image>().enabled = false;
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

		// 操作状況更新
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text =
				this.conrollerProgressToString(i, this.controllerProgresses[i]);

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
			UnityEngine.UI.Text closingTextUI;

			if(closingWindow.gameObject.activeInHierarchy == false) {
				// オーディエンス投票締切直前カウントダウン開始
				closingWindow.gameObject.SetActive(true);
				this.closingAudienceRemainSeconds = PhaseControllers.ClosingAudienceWaitSeconds + 0.99f;

				// 初期状態のプレースホルダーを含む文字列を記憶する
				closingTextUI = GameObject.Find("Controller_ClosingAudienceText").GetComponent<UnityEngine.UI.Text>();
				this.closingText = closingTextUI.text;

				// ウィンドウオープン
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
					this.parent.ChangePhase(new PhaseFlight(this.parent, null));
					this.IsUpdateEnabled = false;
					return;
				}
			}

			// 表示文字列を更新
			closingTextUI = GameObject.Find("Controller_ClosingAudienceText").GetComponent<UnityEngine.UI.Text>();
			closingTextUI.text = this.closingText
				.Replace("${REMAIN_SECOND}", ((int)this.closingAudienceRemainSeconds).ToString());
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
	/// 操作端末へ開始指示を出します。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void startController(int roleId) {
		this.connector.StartController(
			new ModelControllerStart() {
				RoleId = roleId,
				LimitTimeSecond = PhaseControllers.ControllerLimitSeconds,
			},
			roleId,
			new System.Action(() => {
				Debug.Log("接続開始成功: 端末ID=" + roleId);
				this.controllerProgresses[roleId]["isStarted"] = "true";

				// 進捗報告と完了報告を受け付ける
				this.watcherControllerProgresses(roleId);
				this.watcherControllerCompletes(roleId);
			}),
			new System.Action(() => {
				Debug.LogWarning("接続開始失敗: 端末ID=" + roleId);

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
				this.controllerProgresses[roleId] = result.GetDictionary();

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
				this.controllerProgresses[roleId] = result.GetDictionary();
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
			if(string.IsNullOrEmpty(this.eventId) == true) {
				Debug.LogWarning("イベントIDが無効のため投票情報を取得できません。");
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
			this.controllerProgresses[0],
			this.controllerProgresses[1],
			this.controllerProgresses[2],
		}));
	}

}
