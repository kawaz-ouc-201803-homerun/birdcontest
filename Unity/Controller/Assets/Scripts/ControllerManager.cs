using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 操作端末の統括クラス
/// </summary>
public class ControllerManager : MonoBehaviour {

	/// <summary>
	/// 操作端末のフェーズ
	/// </summary>
	public enum ControllerPhase {
		Idle,               // アイドル状態
		Playing,            // ミニゲームプレイ中
		Result,             // 結果画面
		RequiredAutoClose,  // 自動で結果画面を送る
	}

	/// <summary>
	/// デフォルトの制限時間秒数
	/// </summary>
	public const int DefaultLimitTimeSeconds = 30;

	/// <summary>
	/// ミニゲーム終了後の結果画面での待機秒数
	/// </summary>
	public const int ResultPhaseLimitTimeSeconds = 30;

	/// <summary>
	/// 完了報告に失敗したときにやり直す回数の上限
	/// </summary>
	public const int ConnectRetryMaxCount = 0;

	/// <summary>
	/// 完了報告に失敗したときのリトライ待機時間
	/// </summary>
	public const float TCPRetryWaitSeconds = 3.0f;

	/// <summary>
	/// 制限時間秒数
	/// </summary>
	public static int LimitTimeSeconds {
		get;
		private set;
	}

	/// <summary>
	/// 完了報告に失敗した回数
	/// </summary>
	private int currentRetryCount;

	/// <summary>
	/// 現在のフェーズ
	/// </summary>
	private ControllerPhase phase;

	/// <summary>
	/// タイマーオブジェクト
	/// </summary>
	public GameObject TimerObject;

	/// <summary>
	/// 開始指示待ち画面
	/// </summary>
	public GameObject IdleScreen;

	/// <summary>
	/// 最初の画面
	/// </summary>
	public GameObject StartScreen;

	/// <summary>
	/// 各種ミニゲーム画面
	/// </summary>
	public GameObject MainScreen;

	/// <summary>
	/// 最後の画面
	/// </summary>
	public GameObject EndScreen;

	/// <summary>
	/// 各種役割に応じた最初の画面
	/// </summary>
	public ControllerBase[] Controllers;

	/// <summary>
	/// 各種役割に対応する背景用カメラ
	/// </summary>
	public GameObject[] Cameras;

	/// <summary>
	/// 初期状態のレンダリングに使われるカメラ
	/// </summary>
	public GameObject DefaultCamera;

	/// <summary>
	/// 通信接続オブジェクト
	/// </summary>
	private NetworkController connector;

	/// <summary>
	/// ゲームマスターからの開始指示を受け取ったかどうか
	/// </summary>
	private bool readyForStart;

	/// <summary>
	/// 端末個別の動作が開始しているかどうか
	/// </summary>
	private bool isControllerStarted;

	/// <summary>
	/// 別スレッドから完了報告のリトライが要求されているかどうか
	/// </summary>
	private bool isTCPRetryRequired;

	/// <summary>
	/// 完了報告として送信したデータ
	/// </summary>
	private Dictionary<string, string> completeProgressData;

	/// <summary>
	/// 完了報告に失敗したときに表示する辞書型配列をテキスト化したデータ
	/// </summary>
	private string emergencyText;

	/// <summary>
	/// SE再生制御オブジェクト
	/// </summary>
	public SEPlayer SEPlayer;

	/// <summary>
	/// 初回処理
	/// </summary>
	public void Start() {
		this.readyForStart = false;
		this.isControllerStarted = false;
		this.isTCPRetryRequired = false;
		this.emergencyText = "";
		this.currentRetryCount = 0;
		this.phase = ControllerPhase.Idle;

		// 役割IDに応じてカメラを切り替える
		this.DefaultCamera.SetActive(false);
		foreach(var camera in this.Cameras) {
			camera.SetActive(false);
		}
		this.Cameras[ControllerSelector.SelectedRoleId].SetActive(true);

		// ゲームマスターからの接続待機
		this.connector = new NetworkController(ControllerSelector.GameMasterIPAddress);
		this.connector.ControllerWaitForStart(ControllerSelector.SelectedRoleId, new Action<ModelControllerStart>((result) => {
			// 開始指示を受け取ったときの処理
			this.connector.CloseConnectionsAll();

			this.readyForStart = true;
			this.isControllerStarted = true;
			this.phase = ControllerPhase.Playing;

			// キャライメージ画像を表示
			GameObject.Find("CharPanels_Start").transform.Find("CharPanel_Role" + ControllerSelector.SelectedRoleId).gameObject.SetActive(true);

			// データ取り出し
			ControllerManager.LimitTimeSeconds = result.LimitTimeSecond;
			if(ControllerSelector.SelectedRoleId != result.RoleId) {
				Debug.LogError("この端末が受け付けている役割ID (" + ControllerSelector.SelectedRoleId + ") と、ゲームマスターが指示した役割ID (" + result.RoleId + ") が一致しません。双方の設定を確認して下さい。");
			}
		}));

		// 開始指示待ち画面へ
		this.IdleScreen.SetActive(true);
		this.StartScreen.SetActive(false);
		this.MainScreen.SetActive(false);
		this.EndScreen.SetActive(false);

		// キャライメージ画像を表示
		GameObject.Find("CharPanels_Idle").transform.Find("CharPanel_Role" + ControllerSelector.SelectedRoleId).gameObject.SetActive(true);
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		if(this.isControllerStarted == false) {
			// 端末固有の動作開始前
			if(Input.GetKeyDown(KeyCode.F10) == true) {
				// 開始指示が受信できない時の緊急対応用：自律的にゲームを開始できるようにする
				this.isControllerStarted = true;
				this.readyForStart = true;
				ControllerManager.LimitTimeSeconds = ControllerManager.DefaultLimitTimeSeconds;
			}
		}

		if(this.readyForStart == true) {
			// ゲームマスターから開始指示を受け取ったときの初回処理
			this.connector.CloseConnectionsAll();
			this.readyForStart = false;
			this.startController();
		}

		if(this.StartScreen.activeInHierarchy == true || this.MainScreen.activeInHierarchy == true) {
			// ミニゲーム画面にいるとき、ユーザー入力（Escapeキーで）でリセット
			if(Input.GetKeyDown(KeyCode.Escape) == true) {
				this.connector.CloseConnectionsAll();
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				return;
			}
		}

		if(this.EndScreen.activeInHierarchy == true) {
			// 終了画面にいるとき、ユーザー入力（Escapeキー）でアイドル画面に戻す
			if(this.phase == ControllerPhase.RequiredAutoClose) {
				// 一定時間経過後に自動でアイドル状態へ戻す
				this.StartCoroutine(this.autoResultCloser());
				this.phase = ControllerPhase.Result;
			}
			if(Input.GetKeyDown(KeyCode.Escape) == true
			&& this.phase == ControllerPhase.Result) {
				// 強制的にアイドル状態へ戻す
				this.closeResult();
				return;
			}
			if(Input.GetKeyDown(KeyCode.Q) == true
			&& this.phase == ControllerPhase.Result) {
				// 強制的に障害発生扱いにする
				this.showEmergencyMessage();
			}

			// 障害発生時のテキスト
			this.EndScreen.transform.Find("EmergencyText").GetComponent<Text>().text = this.emergencyText;
			if(string.IsNullOrEmpty(this.emergencyText) == true) {
				// 平常時
				this.EndScreen.transform.Find("Windows/WindowEnd/Text").GetComponent<Text>().text = "おわり";
			} else {
				// 障害発生時
				this.EndScreen.transform.Find("Windows/WindowEnd/Text").GetComponent<Text>().text = "＜＜通信障害発生＞＞";
			}

			// 障害発生時にリトライする
			if(this.isTCPRetryRequired == true) {
				this.isTCPRetryRequired = false;
				this.StartCoroutine(this.retrySendCompleteProgress());
			}
		}
	}

	/// <summary>
	/// STARTボタン押下時
	/// </summary>
	public void OnStart() {
		this.SEPlayer.PlaySE((int)SEPlayer.SEID.Decision);

		// 各種役割に応じた最初の画面に入る
		this.StartScreen.SetActive(false);
		this.MainScreen.SetActive(true);
		foreach(var controller in this.Controllers) {
			controller.gameObject.SetActive(false);
		}
		this.Controllers[ControllerSelector.SelectedRoleId].gameObject.SetActive(true);
	}

	/// <summary>
	/// ゲームマスターからの指示を受けて操作端末を開始します。
	/// </summary>
	private void startController() {
		this.SEPlayer.PlaySE((int)SEPlayer.SEID.ControllerStart);

		// アイドル画面から共通スタート画面へ
		this.IdleScreen.SetActive(false);
		this.StartScreen.SetActive(true);
		this.StartScreen.transform.Find("Window/Text").GetComponent<Text>().text = this.getStartingMessage();

		// キャライメージ画像を表示
		GameObject.Find("CharPanels_Start").transform.Find("CharPanel_Role" + ControllerSelector.SelectedRoleId).gameObject.SetActive(true);

		// タイマーゼロカウント時の処理を定義
		this.TimerObject.SetActive(false);
		this.TimerObject.GetComponent<Timer>().ZeroTimerEvent.RemoveAllListeners();
		this.TimerObject.GetComponent<Timer>().ZeroTimerEvent.AddListener(new UnityAction(() => {
			this.phase = ControllerPhase.Result;

			// メイン画面を終えて終了画面へ
			this.MainScreen.SetActive(false);
			this.EndScreen.SetActive(true);
			this.EndScreen.transform.Find("Windows/WindowResult/Text").GetComponent<Text>().text = this.Controllers[ControllerSelector.SelectedRoleId].GetResultText();
			this.EndScreen.transform.Find("Windows").localScale = new Vector3(0, 0, 0);
			iTween.ScaleTo(
				this.EndScreen.transform.Find("Windows").gameObject,
				new Vector3(1, 1, 1),
				1.0f
			);

			// ゲームマスターへ完了報告を出す
			Debug.Log("GMへ完了報告を出します...");
			this.sendCompleteProgress();
		}));
	}

	/// <summary>
	/// ゲームマスターへ完了報告を出します。
	/// </summary>
	private void sendCompleteProgress() {
		this.completeProgressData = this.Controllers[ControllerSelector.SelectedRoleId].SendCompleteProgress(
			new System.Action(() => {
				// 送信成功
				Debug.Log("完了報告OK");
				this.emergencyText = "";
				this.connector.CloseConnectionsAll();
				this.phase = ControllerPhase.RequiredAutoClose;
			}),
			new System.Action(() => {
				// 送信失敗
				if(this.phase != ControllerPhase.Result) {
					// 既に結果フェーズから離れている場合は何もしない
					return;
				}

				Debug.LogWarning("完了報告に失敗");
				this.currentRetryCount++;
				if(this.currentRetryCount <= ControllerManager.ConnectRetryMaxCount) {
					// 再試行：リトライ回数が上限に達するまでは障害扱いにしない
					this.isTCPRetryRequired = true;
					return;
				}

				// ゲームマスター側でデータを直接入力できるようにするため、送ろうとしたデータの中身を表示する
				Debug.LogError("完了報告に失敗：リトライ上限に達したため障害扱いとします");
				this.showEmergencyMessage();
			})
		);
	}

	/// <summary>
	/// 一定時間後に、ゲームマスターへの完了報告をやり直します。
	/// </summary>
	private IEnumerator retrySendCompleteProgress() {
		Debug.Log("GM完了報告リトライ待ち [" + this.currentRetryCount + " / " + ControllerManager.ConnectRetryMaxCount + "]");
		yield return new WaitForSeconds(ControllerManager.TCPRetryWaitSeconds);
		this.sendCompleteProgress();
	}

	/// <summary>
	/// 結果画面を閉じます。
	/// </summary>
	private void closeResult() {
		this.phase = ControllerPhase.Idle;
		this.SEPlayer.PlaySE((int)SEPlayer.SEID.Decision);

		// 通信切断
		this.connector.CloseConnectionsAll();

		var fader = GameObject.Find("FadeCanvas").GetComponent<Fade>();
		fader.FadeIn(2.0f, new Action(() => {
			// GC実行
			System.GC.Collect();

			// フェードアウト後、再び開始指示待ち状態へ戻る
			this.connector.CloseConnectionsAll();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}));
	}

	/// <summary>
	/// 結果画面を自動的に閉じるコルーチン
	/// </summary>
	private IEnumerator autoResultCloser() {
		yield return new WaitForSeconds(ControllerManager.ResultPhaseLimitTimeSeconds);

		// 障害が発生しているときは自動で遷移しないようにする
		if(this.phase == ControllerPhase.Result
		&& string.IsNullOrEmpty(this.emergencyText) == true) {
			this.closeResult();
		}
	}

	/// <summary>
	/// 障害発生モードに切り替えます。
	/// </summary>
	private void showEmergencyMessage() {
		using(var buf = new StringWriter()) {
			bool wrote = false;

			foreach(var key in this.completeProgressData) {
				if(wrote == true) {
					// ２回目以降は区切り記号を付ける
					buf.Write(";");
				}
				buf.Write(key.Key + "=" + key.Value);
				wrote = true;
			}

			this.emergencyText = buf.ToString();
			this.connector.CloseConnectionsAll();
		}
	}

	/// <summary>
	/// 最初の画面に表示するメッセージ
	/// </summary>
	/// <returns>メッセージ文字列</returns>
	private string getStartingMessage() {
		switch(ControllerSelector.SelectedRoleId) {
			case (int)ControllerSelector.RoleIds.A_Prepare:
				return @"あなたの役割は飛行機にいいスタートダッシュを切らせることだ。
どれで助走をつけてやるのが一番いいだろうか？
３つの中から選ぼう！";

			case (int)ControllerSelector.RoleIds.B_Flight:
				return @"あなたの役割は飛行機をこぐことだ。
ボタンを連打してメーターの真ん中を保ち続けろ！
たくさん気合をためて、長い距離を飛ぼう！";

			case (int)ControllerSelector.RoleIds.C_Assist:
				return @"あなたの役割は飛行機をサポートすることだ。
どうやって補助をしようか？
３つの中から選ぼう！";
		}

		return "";
	}

}
