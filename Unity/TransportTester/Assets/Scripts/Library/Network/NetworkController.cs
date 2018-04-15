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
	/// 送信用UDPクライアント
	/// </summary>
	private UdpClient udpClient = null;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="gameMasterIPAddress">ゲームマスターのIPアドレス。nullにするとサトの環境デフォルト設定になります。</param>
	public NetworkController(string gameMasterIPAddress) : base(gameMasterIPAddress, null) {
	}

	/// <summary>
	/// TCPでゲームマスターからの開始指示を待機します。
	/// </summary>
	public void ControllerWaitForStart(Action<ModelControllerStart> callback) {
		this.startTCPServer(NetworkConnector.GeneralPort, callback);
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
		this.udpClient = this.startUDPSender(this.udpClient, this.GameMasterIPAddress, NetworkConnector.ControllerPorts[this.RoleId], data, null);
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

		// UDPの送信を止める
		if(this.udpClient != null) {
			this.udpClient = null;
		}

		// TCPで送信
		this.startTCPClient(this.GameMasterIPAddress, NetworkConnector.ControllerPorts[this.RoleId], data, successCallBack, failureCallBack);
	}

}
