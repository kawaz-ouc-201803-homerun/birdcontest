using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TCP/UDPの通信処理を行うラッパークラス
/// </summary>
public class Network {

	/// <summary>
	/// ゲームマスターのIPアドレス（決め打ち）
	/// </summary>
	protected const string GameMasterIPAddress = "192.168.11.2";

	/// <summary>
	/// 各種操作端末のIPアドレス
	/// </summary>
	protected static readonly string[] ControllerIPAddresses = new string[] {
		"192.168.11.10",
		"192.168.11.11",
		"192.168.11.12",
	};

	/// <summary>
	/// 各種操作端末が使用するポート番号
	/// ゲームマスターはこれらすべてを解放する必要があります。
	/// </summary>
	protected static readonly int[] ControllerPorts = new int[] {
		31000,
		31001,
		31002,
	};

	/// <summary>
	/// 共通ポート番号
	/// </summary>
	protected const int GeneralPort = 30000;

	/// <summary>
	/// 端末番号
	/// ゲームマスターは -1 とする。
	/// </summary>
	protected int controllerId = -1;

	/// <summary>
	/// コンストラクター
	/// </summary>
	public Network(int controllerId) {
		if(controllerId == -1) {
			// ゲームマスター
			this.controllerId = -1;
		} else if(controllerId <= 0 && Network.ControllerIPAddresses.Length < controllerId) {
			// 操作端末
			this.controllerId = controllerId;
		} else {
			// 不正な端末ID
			throw new System.ArgumentOutOfRangeException("controllerId");
		}
	}

	/// <summary>
	/// ステップ 1.
	/// TCPでゲームマスターからの開始指示を待機します。
	/// このメソッドを実行している間はブロッキングされます。
	/// </summary>
	/// <returns>役割ID</returns>
	public int WaitForStart() {
		var listener = new System.Net.Sockets.TcpListener(
			System.Net.IPAddress.Parse(Network.GameMasterIPAddress),
			StartingPort
		);

		// 接続待機開始
		listener.Start();

		// ここでブロッキング
		int id = -1;
		using(var client = listener.AcceptTcpClient()) {
			// 開始指示（役割ID）を受け取る
			using(var stream = client.GetStream()) {
				id = stream.ReadByte();
			}
		}

		// データを受け取ったら接続切断
		listener.Stop();

		return id;
	}

	/// <summary>
	/// ステップ 2.
	/// HTTP通信でオーディエンス予想の新しいイベントを生成します。
	/// </summary>
	/// <param name="id">イベントの識別子</param>
	public void StartAudiencePredicts(string id) {

	}

	/// <summary>
	/// ステップ 3.
	/// UDPでゲームマスターに端末の進捗状況を送信します。
	/// UDP通信するため、ブロッキングはありませんが、毎フレームで呼び出すと回線の負荷がワヤになるので一定間隔を置いて呼び出して下さい。
	/// 進捗状況のデータの取り決め（プロトコル）は事前に端末の担当者同士で打ち合わせが必要です。
	/// </summary>
	public void ProgressToGameMaster(string data) {
		// UDPクライアントを生成
		var udp = new System.Net.Sockets.UdpClient(
			new System.Net.IPEndPoint(
				System.Net.IPAddress.Parse(Network.GameMasterIPAddress),
				GeneralPort
			)
		);

		// データの送信
		byte[] databytes = System.Text.Encoding.UTF8.GetBytes(data);
		udp.Send(databytes, databytes.Length);
	}

	/// <summary>
	/// ステップ 4.
	/// UDPによる各端末の進捗報告をゲームマスター側で受け取ります。
	/// UDP通信のため、ブロッキングはありません。
	/// </summary>
	public string LoadBranchProgress() {
		// UDPクライアントを生成（localhost）
		var udp = new System.Net.Sockets.UdpClient(
			new System.Net.IPEndPoint(
				System.Net.IPAddress.Parse("127.0.0.1"),
				GeneralPort
			)
		);

		// データの受信
		System.Net.IPEndPoint remoteEndPoint = null;
		var receivedData = udp.Receive(ref remoteEndPoint);

		return System.Text.Encoding.UTF8.GetString(receivedData);
	}
	/// <summary>
	/// ステップ 5.
	/// TCPでゲームマスターが各端末の操作の終了を待ち受けます。
	/// このメソッドを実行している間はブロッキングされます。
	/// </summary>
	public void WaitForBranch() {

	}

	/// <summary>
	/// ステップ 6.
	/// HTTP通信で指定した識別子のイベントのオーディエンス予想を取り出します。
	/// </summary>
	/// <param name="id">イベントの識別子</param>
	/// <returns>オーディエンス予想のリスト</returns>
	public List<AudiencePredictModel> LoadAudiencePredicts(string id) {
		return null;
	}

}
