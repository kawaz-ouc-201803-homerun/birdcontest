using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 操作端末側のTCP/UDP通信処理を行うラッパークラス
/// </summary>
public class NetworkController : NetworkConnector {

	/// <summary>
	/// 操作端末の役割ID
	/// -1 は未定な状態とします。
	/// </summary>
	public int RoleId {
		get; set;
	} = -1;

	/// <summary>
	/// TCPでゲームマスターからの開始指示を待機します。
	/// </summary>
	public void ControllerWaitForStart<T>(Action<T> callback) where T : IJSONable<T> {
		this.startTCPServer(NetworkConnector.GeneralPort, callback);
	}

	/// <summary>
	/// UDPでゲームマスターに端末の進捗状況を送信します。
	/// 毎フレームで呼び出すと回線の負荷がワヤになるので一定間隔を置いて呼び出して下さい。
	/// </summary>
	/// <param name="data">報告内容</param>
	public void ProgressToGameMaster(object data) {
		if(this.RoleId == -1) {
			throw new Exception("操作端末の役割IDが設定されていません。");
		}
		this.startUDPSender(NetworkConnector.GameMasterIPAddress, this.RoleId, data, null);
	}

	/// <summary>
	/// TCPでゲームマスターに完了の報告を送信します。
	/// </summary>
	/// <param name="data">報告内容</param>
	/// <param name="callback">処理が完了したときに呼び出されるコールバック関数</param>
	public void ReportCompleteControllerToGameMaster(object data, Action callback) {
		if(this.RoleId == -1) {
			throw new Exception("操作端末の役割IDが設定されていません。");
		}
		this.startTCPClient(NetworkConnector.GameMasterIPAddress, this.RoleId, data, callback);
	}

}
