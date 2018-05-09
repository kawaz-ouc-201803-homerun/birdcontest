using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	/// 初回処理
	/// </summary>
	public virtual void Start() {
		// 開始指示の受信は別のオブジェクトで行うため、ここでは直接役割IDを入力してインスタンス化する
		this.connector = new NetworkController(ControllerSelector.GameMasterIPAddress) {
			RoleId = ControllerSelector.SelectedRoleId,
		};
		this.progressSenderTimer = 0;
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
		return new Dictionary<string, string>() {
			{ "roleId", ControllerSelector.SelectedRoleId.ToString() },
		};
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public abstract string GetResultText();

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public abstract void StartNewGame();

}
