using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 操作端末の統括クラス
/// </summary>
public class ControllerManager : MonoBehaviour {

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
	/// 制限時間秒数
	/// </summary>
	private int limitTimeSecond;

	/// <summary>
	/// 完了報告として送信したデータ
	/// </summary>
	private Dictionary<string, string> completeProgressData;

	/// <summary>
	/// 完了報告に失敗したときに表示する辞書型配列をテキスト化したデータ
	/// </summary>
	private string emergencyText;

	/// <summary>
	/// 初回処理
	/// </summary>
	public void Start() {
		// ゲームマスターからの接続待機
		this.readyForStart = false;
		this.connector = new NetworkController(ControllerSelector.GameMasterIPAddress);
		this.connector.ControllerWaitForStart(ControllerSelector.SelectedRoleId, new System.Action<ModelControllerStart>((result) => {
			// 開始指示を受け取ったときの処理
			this.readyForStart = true;
			this.isControllerStarted = true;

			// データ取り出し
			this.limitTimeSecond = result.LimitTimeSecond;
			if(ControllerSelector.SelectedRoleId != result.RoleId) {
				Debug.LogError("この端末が受け付けている役割ID (" + ControllerSelector.SelectedRoleId + ") と、ゲームマスターが指示した役割ID (" + result.RoleId + ") が一致しません。双方の設定を確認して下さい。");
			}
		}));

		// 開始指示待ち画面へ
		this.IdleScreen.SetActive(true);
		this.StartScreen.SetActive(false);
		this.MainScreen.SetActive(false);
		this.EndScreen.SetActive(false);
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
				this.limitTimeSecond = 30;
			}
		}

		if(this.readyForStart == true) {
			// ゲームマスターから開始指示を受け取ったときの初回処理
			this.connector.CloseConnectionsAll();
			this.readyForStart = false;
			this.startController();
		}

		if(this.EndScreen.activeInHierarchy == true) {
			// 終了画面にいるとき、ユーザー入力（Enterキー）でアイドル画面に戻す
			if(Input.GetKeyDown(KeyCode.Return) == true) {
				this.connector.CloseConnectionsAll();
				this.TimerObject.SetActive(false);
				this.EndScreen.SetActive(false);
				this.IdleScreen.SetActive(true);
				this.emergencyText = "";
			}

			// 障害発生時のテキスト
			this.EndScreen.transform.Find("EmergencyText").GetComponent<Text>().text = this.emergencyText;
			if(string.IsNullOrEmpty(this.emergencyText) == false) {
				// 障害発生時
				this.EndScreen.transform.Find("Text").GetComponent<Text>().text = "＜＜通信障害発生＞＞";
			}
		}
	}

	/// <summary>
	/// STARTボタン押下時
	/// </summary>
	public void OnStart() {
		// 各種役割に応じた最初の画面に入る
		this.StartScreen.SetActive(false);
		this.MainScreen.SetActive(true);
		foreach(var controller in this.Controllers) {
			controller.gameObject.SetActive(false);
		}
		this.Controllers[ControllerSelector.SelectedRoleId].gameObject.SetActive(true);

		// タイマー開始
		this.TimerObject.transform.Find("TimerManager").GetComponent<Timer>().TimeSeconds = this.limitTimeSecond;
		this.TimerObject.SetActive(true);
	}

	/// <summary>
	/// ゲームマスターからの指示を受けて操作端末を開始します。
	/// </summary>
	private void startController() {
		// アイドル画面から共通スタート画面へ
		this.IdleScreen.SetActive(false);
		this.StartScreen.SetActive(true);
		this.StartScreen.transform.Find("Window/Text").GetComponent<Text>().text = this.getStartingMessage();

		// タイマーゼロカウント時の処理を定義
		this.TimerObject.SetActive(false);
		this.TimerObject.transform.Find("TimerManager").GetComponent<Timer>().ZeroTimerEvent.AddListener(new UnityAction(() => {
			// メイン画面を終えて終了画面へ
			this.MainScreen.SetActive(false);
			this.EndScreen.SetActive(true);
			this.EndScreen.transform.Find("Windows/WindowResult/Text").GetComponent<Text>().text = this.Controllers[ControllerSelector.SelectedRoleId].GetResultText();
			iTween.ScaleTo(
				this.EndScreen.transform.Find("Windows").gameObject,
				new Vector3(1, 1, 1),
				1.0f
			);

			// ゲームマスターへ完了報告を出す
			this.completeProgressData = this.Controllers[ControllerSelector.SelectedRoleId].SendCompleteProgress(
				new System.Action(() => {
					// 送信成功
					Debug.Log("完了報告OK");
					this.emergencyText = "";
				}),
				new System.Action(() => {
					// 送信失敗
					Debug.LogError("完了報告に失敗");

					// ゲームマスター側でデータを直接入力できるようにするため、送ろうとしたデータの中身を表示する
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
					}
				})
			);
		}));
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
