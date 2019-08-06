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
	}

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="gameMasterIPAddress">ゲームマスターのIPアドレス。nullにするとサトの環境デフォルト設定になります。</param>
	public NetworkController(string gameMasterIPAddress) : base(gameMasterIPAddress, null) {
	}

	/// <summary>
	/// TCPでゲームマスターからの開始指示を待機します。
	/// 待ち受けポートは役割個別のものを使います。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	/// <param name="callback">処理が完了したときに呼び出されるコールバック関数</param>
	public void ControllerWaitForStart(int roleId, Action<ModelControllerStart> callback) {
		this.RoleId = roleId;
		this.startTCPServer(NetworkConnector.StartingControllerPorts[this.RoleId], callback);
	}

	/// <summary>
	/// UDPでゲームマスターに端末の進捗状況を送信します。
	/// 毎フレームで呼び出すと回線の負荷がワヤになるので一定間隔を置いて呼び出して下さい。
	/// </summary>
	/// <param name="data">報告内容</param>
	public void ReportProgressToGameMaster(object data) {
		if(this.RoleId == -1) {
			throw new Exception("操作端末の役割IDが設定されていません。");
		}
		this.startUDPSender(this.GameMasterIPAddress, NetworkConnector.ProgressToGameMasterPorts[this.RoleId], data, null);
	}

	/// <summary>
	/// TCPでゲームマスターに完了の報告を送信します。
	/// </summary>
	/// <param name="data">報告内容</param>
	/// <param name="successCallBack">処理が完了したときに呼び出されるコールバック関数</param>
	/// <param name="failureCallBack">処理が完了したときに呼び出されるコールバック関数</param>
	public void ReportCompleteToGameMaster(object data, Action successCallBack, Action failureCallBack) {
		if(this.RoleId == -1) {
			throw new Exception("操作端末の役割IDが設定されていません。");
		}

		// TCPで送信
		this.startTCPClient(this.GameMasterIPAddress, NetworkConnector.ProgressToGameMasterPorts[this.RoleId], data, successCallBack, failureCallBack);
	}

}
