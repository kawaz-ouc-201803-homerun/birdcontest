using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作端末の基底クラス
/// </summary>
public abstract class ControllerBase : MonoBehaviour {

	/// <summary>
	/// 進捗状況の定期報告を行う間隔秒数
	/// </summary>
	public const float ProgressSendTimeSeconds = 1.0f;

	/// <summary>
	/// 進捗状況の定期報告/UDPのタイマー
	/// </summary>
	private float progressSenderTimer;

	/// <summary>
	/// 通信接続オブジェクト
	/// </summary>
	protected NetworkController connector;

	/// <summary>
	/// Ready-Goオブジェクト
	/// </summary>
	public GameObject ReadyGo;

	/// <summary>
	/// タイマーオブジェクト
	/// </summary>
	public GameObject TimerObject;

	/// <summary>
	/// Ready-Go表示を行ったかどうか
	/// </summary>
	protected bool doneReadyGo;

	/// <summary>
	/// ミニゲームオブジェクト
	/// </summary>
	public SubGameBase[] SubGames;

	/// <summary>
	/// アクティブなミニゲーム
	/// </summary>
	protected SubGameBase activeSubGame;

	/// <summary>
	/// 初回処理
	/// </summary>
	public virtual void Start() {
		// 開始指示の受信は別のオブジェクトで行うため、ここでは直接役割IDを入力してインスタンス化する
		this.connector = new NetworkController(ControllerSelector.GameMasterIPAddress) {
			RoleId = ControllerSelector.SelectedRoleId,
		};
		this.progressSenderTimer = 0;
		this.doneReadyGo = false;
		this.ReadyGo.transform.localScale = Vector3.zero;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public virtual void Update() {
		// 進捗状況の定期報告タイマー処理
		this.progressSenderTimer += Time.deltaTime;
		if(this.progressSenderTimer >= ControllerBase.ProgressSendTimeSeconds) {
			this.progressSenderTimer = 0;

			// 進捗報告を行う
			this.connector.ReportProgressToGameMaster(
				new ModelDictionary<string, string>(this.createProgressData())
			);
		}
	}

	/// <summary>
	/// ゲームマスターに完了報告を送信します。
	/// </summary>
	/// <returns>報告内容として送信した辞書型配列</returns>
	public Dictionary<string, string> SendCompleteProgress(Action successCallback, Action failureCallback) {
		var dictionary = this.createProgressData();
		this.connector.ReportCompleteToGameMaster(
			new ModelDictionary<string, string>(this.createProgressData()),
			successCallback,
			failureCallback
		);
		return dictionary;
	}

	/// <summary>
	/// 進捗報告として送るデータを生成します。
	/// </summary>
	/// <returns>進捗報告として送る辞書型配列</returns>
	protected virtual Dictionary<string, string> createProgressData() {
		var dictionary = new Dictionary<string, string>();
		dictionary["roleId"] = ControllerSelector.SelectedRoleId.ToString();

		if(this.activeSubGame != null) {
			this.activeSubGame.SetProgressData(ref dictionary);
		}

		return dictionary;
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public virtual string GetResultText() {
		if(this.activeSubGame != null) {
			return this.activeSubGame.GetResultText();
		}
		return "";
	}

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public virtual void StartNewGame() {
		// すべての通信を切断
		this.connector.CloseConnectionsAll();
		this.doneReadyGo = false;
		this.activeSubGame = null;
	}

	/// <summary>
	/// READY? GO! 表示を行い、タイマーを開始します。
	/// </summary>
	protected void doReadyGo() {
		this.doneReadyGo = true;
		this.ReadyGo.transform.Find("Text").GetComponent<Text>().text = "READY…？";
		iTween.ScaleTo(
			this.ReadyGo,
			iTween.Hash(
				"x", 1,
				"y", 1,
				"z", 1,
				"time", 0.5f,
				"delay", 0.01f,
				"easeType", iTween.EaseType.easeOutQuint,
				"oncomplete", "AfterShowReady",
				"oncompletetarget", this.gameObject
			)
		);
	}

	/// <summary>
	/// [READY?] iTween表示完了後の処理
	/// </summary>
	protected void AfterShowReady() {
		iTween.ScaleTo(
			this.ReadyGo,
			iTween.Hash(
				"x", 0,
				"y", 0,
				"z", 0,
				"time", 0.3f,
				"delay", 1.0f,
				"easeType", iTween.EaseType.easeOutQuint,
				"oncomplete", "AfterHideReady",
				"oncompletetarget", this.gameObject
			)
		);
	}

	/// <summary>
	/// [REDY?] iTween消去完了後の処理
	/// </summary>
	protected void AfterHideReady() {
		this.ReadyGo.transform.localScale = Vector3.zero;
		this.ReadyGo.transform.Find("Text").GetComponent<Text>().text = "GO!!!";
		iTween.ScaleTo(
			this.ReadyGo,
			iTween.Hash(
				"x", 1,
				"y", 1,
				"z", 1,
				"time", 0.3f,
				"delay", 0.01f,
				"easeType", iTween.EaseType.easeOutQuint,
				"oncomplete", "AfterShowGo",
				"oncompletetarget", this.gameObject
			)
		);
	}

	/// <summary>
	/// [GO!!!] iTween表示完了後の処理
	/// </summary>
	protected void AfterShowGo() {
		iTween.ScaleTo(
			this.ReadyGo,
			iTween.Hash(
				"x", 0,
				"y", 0,
				"z", 0,
				"easeType", iTween.EaseType.easeOutQuint,
				"time", 0.3f,
				"delay", 1.0f,
				"oncomplete", "AfterHideGo",
				"oncompletetarget", this.gameObject
			)
		);
	}

	/// <summary>
	/// [GO!!!] iTween消去完了後の処理
	/// </summary>
	protected virtual void AfterHideGo() {
		// ゲーム開始
		this.gameObject.SetActive(true);

		// タイマー開始
		this.TimerObject.SetActive(true);
		this.TimerObject.GetComponent<Timer>().StartTimer(ControllerManager.LimitTimeSeconds);
	}

}
