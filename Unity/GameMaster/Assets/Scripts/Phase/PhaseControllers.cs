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
	private static readonly string[] ControllerIPAddresses = new string[] {
		"192.168.11.2",
		"192.168.11.3",
		"192.168.11.4",
	};

	/// <summary>
	/// オーディエンス投票を監視する間隔秒数
	/// </summary>
	private const float AudienceWatchSeconds = 5.0f;

	/// <summary>
	/// オーディエンス投票を締め切る直前の待機秒数
	/// </summary>
	private const int ClosingAudienceWaitSeconds = 5;

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
	}

	/// <summary>
	/// ゲームオブジェクトの初期化
	/// </summary>
	public override void Start() {
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");
		closingWindow.gameObject.SetActive(false);

		this.parent.StartCoroutine(this.watcherAudience());

		this.isControllerCompleted = new bool[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			this.watcherControllerProgresses(i);
			this.watcherControllerCompletes(i);
		}
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		GameObject.Find("Controller_AudiencePostInformation").GetComponent<UnityEngine.UI.Text>().text = this.postText;

		// スクロール位置を常に一番上にする
		var scrollView = GameObject.Find("Controller_PostedAudience");
		scrollView.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1f;

		// すべての端末が終了したか判定する
		var isControllerCompleteAll = (this.isControllerCompleted.Where(a => a == false).Count() == 0);
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");

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
	/// 操作端末の進捗報告を受け付けます。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void watcherControllerProgresses(int roleId) {
		this.connector.ReceiveControllerProgress(
			roleId,
			new System.Action<ModelDictionary<string, string>>((result) => {

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
			}
		));
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

}
