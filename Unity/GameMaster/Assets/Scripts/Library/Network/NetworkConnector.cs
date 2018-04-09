using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// TCP/UDPの通信処理を行う基本クラス
/// </summary>
public abstract class NetworkConnector {

	/// <summary>
	/// 操作端末の役割ID
	/// </summary>
	public enum RoleIds {
		A_Prepare,
		B_Flight,
		C_Assist,
	}

	/// <summary>
	/// 共通ポート番号
	/// </summary>
	protected const int GeneralPort = 30100;

	/// <summary>
	/// ゲームマスターのIPアドレス（決め打ち）
	/// </summary>
	protected const string GameMasterIPAddress = "192.168.11.2";

	/// <summary>
	/// オーディエンス投票システムの基本URL
	/// </summary>
	protected const string AudienceSystemBaseURL = "http://tsownserver.dip.jp:8080/birdman/";

	/// <summary>
	/// 各種操作端末のIPアドレス（決め打ち）
	/// </summary>
	protected static readonly string[] ControllerIPAddresses = new string[] {
		"192.168.11.10",
		"192.168.11.11",
		"192.168.11.12",
	};

	/// <summary>
	/// 各種操作端末が使用するポート番号
	/// ゲームマスターはこれらすべてのポートを開放する必要があります。
	/// </summary>
	protected static readonly int[] ControllerPorts = new int[] {
		30000,
		30001,
		30002,
	};

	/// <summary>
	/// WebAPIにリクエストを送信して任意の型のレスポンスを受信します。
	/// </summary>
	/// <typeparam name="T">受信するレスポンスの型</typeparam>
	/// <param name="url">リクエスト送信先URL</param>
	/// <param name="parameter">リクエストとして送信するオブジェクト</param>
	/// <returns>レスポンスとして受信したオブジェクト。JSON形式からのデシリアライズに失敗した場合は null を返す</returns>
	protected T postRequestWithResponseObject<T>(string url, object parameter) where T : IJSONable<T> {
		var httpRequest = HttpWebRequest.CreateHttp(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = "";
		if(parameter != null) {
			json = JsonUtility.ToJson(parameter);
		}

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		if(parameter != null) {
			using(var W = new System.IO.StreamWriter(httpRequest.GetRequestStream())) {
				W.Write(json);
			}
		}

		// HTTPリクエスト送信、サーバーからレスポンスを受け取る
		json = "";
		using(var httpResponse = httpRequest.GetResponse()) {
			var buf = new System.IO.StringWriter();
			using(var R = new System.IO.StreamReader(httpResponse.GetResponseStream())) {
				buf.Write(R.Read());
			}
			json = buf.ToString();
		}

		// 受信したJSONをデシリアライズして返す
		try {
			return JsonUtility.FromJson<T>(json);
		} catch {
			Debug.LogWarning("HTTPレスポンスのJSONデシリアライズに失敗しました。型が一致していない可能性があります。");
			return default(T);
		}
	}

	/// <summary>
	/// WebAPIにリクエストを送信してステータスコードを確認します。
	/// </summary>
	/// <param name="url">リクエスト送信先URL</param>
	/// <param name="parameter">リクエストとして送信するオブジェクト</param>
	/// <returns>HTTPステータスコード</returns>
	protected HttpStatusCode postRequestWithResponseCode(string url, object parameter) {
		var httpRequest = HttpWebRequest.CreateHttp(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = "";
		if(parameter != null) {
			json = JsonUtility.ToJson(parameter);
		}

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		if(parameter != null) {
			using(var W = new System.IO.StreamWriter(httpRequest.GetRequestStream())) {
				W.Write(json);
			}
		}

		// HTTPリクエスト送信、サーバーからレスポンスを受け取る
		using(var httpResponse = (HttpWebResponse)httpRequest.GetResponse()) {
			return httpResponse.StatusCode;
		}
	}

	/// <summary>
	/// データ受信のみを行うTCPサーバーとして非同期的に接続を待ち受けます。
	/// </summary>
	/// <typeparam name="T">受信するデータの型</typeparam>
	/// <param name="port">使用するポート番号</param>
	/// <param name="callback">データ処理を行うコールバック関数</param>
	protected void startTCPServer<T>(int port, Action<T> callback) where T : IJSONable<T> {
		var tcpListener = new TcpListener(System.Net.IPAddress.Any, port);

		// 非同期で接続待機開始
		Debug.Log("非同期TCP接続待ち (Server): " + "any:" + port);
		tcpListener.Start();
		tcpListener.BeginAcceptSocket(
			new AsyncCallback((async) => {
				var tcp = (TcpListener)async.AsyncState;
				Debug.Log("非同期TCPデータ受信中: " + "any:" + port);

				// データを受け取る
				var receivedData = new System.IO.MemoryStream();
				try {

					using(var client = tcp.AcceptTcpClient()) {
						using(var stream = client.GetStream()) {
							// 先頭４バイトはデータの長さが入っている
							var buf = new byte[4];
							stream.Read(buf, 0, buf.Length);
							int length = BitConverter.ToInt32(buf, 0);

							// 残りをすべてJSONの実データとして読み込む
							int readbyte;
							while((readbyte = stream.ReadByte()) != -1) {
								length -= readbyte;
								receivedData.Write(BitConverter.GetBytes(readbyte), 0, 1);
							}
							if(length != 0) {
								throw new Exception("受信したデータがヘッダーに示された長さと一致しません。過不足＝" + length);
							}
						}
					}

					Debug.Log("非同期TCPデータ受信完了: " + receivedData.Length + " Bytes");

				} catch(Exception e) {
					Debug.LogWarning("非同期TCPデータ受信エラー: " + e.Message);
					return;
				}

				// 受信したJSONをデータとして復元して、呼出元が定義したコールバックを呼び出す
				var obj = JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(receivedData.ToArray()));
				callback?.Invoke(obj);

				// 非同期の通信を切断
				tcp.EndAcceptSocket(async);
			}),
			tcpListener
		);
	}

	/// <summary>
	/// データ送信のみを行うTCPクライアントとして非同期的に接続を試行します。
	/// </summary>
	/// <param name="ipAddress">接続先IPアドレス</param>
	/// <param name="port">接続先ポート番号</param>
	/// <param name="data">送信するデータ（JSONとしてシリアライズ可能なもの）</param>
	/// <param name="callback">送信が完了したときに呼び出されるコールバック関数</param>
	protected void startTCPClient(string ipAddress, int port, object data, Action callback) {
		var tcpClient = new TcpClient();

		// 非同期で接続開始
		Debug.Log("非同期TCP接続待ち (Client): " + ipAddress + ":" + port);
		tcpClient.BeginConnect(
			System.Net.IPAddress.Parse(ipAddress),
			port,
			new AsyncCallback((async) => {
				// 送信準備完了
				var tcp = (TcpClient)async.AsyncState;
				Debug.Log("非同期TCPデータ送信中: " + tcp.Client.RemoteEndPoint.ToString());

				// データを送信
				try {

					// 送信対象データはJSON形式に変換する
					var json = JsonUtility.ToJson(data);
					var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);
					var stream = tcp.GetStream();

					// 先頭４バイトはデータの長さを入れる
					stream.Write(BitConverter.GetBytes(jsonBinary.Length), 0, 4);

					// 残りすべてをJSONの実データとして入れる
					stream.Write(jsonBinary, 0, jsonBinary.Length);

					Debug.Log("非同期TCPデータ送信完了: " + (4 + jsonBinary.Length) + " Bytes");

					// 呼出元が定義したコールバック関数を呼び出す
					callback?.Invoke();

					// 非同期の接続を終了
					tcpClient.EndConnect(async);

				} catch(Exception e) {
					Debug.LogWarning("非同期TCPデータ送信エラー: " + e.Message);
				}
			}),
			tcpClient
		);
	}

	/// <summary>
	/// UDPによるデータ受信を非同期的に行います。
	/// UDPなので必ずしも受信できるとは限りません。
	/// </summary>
	/// <param name="port">使用するポート番号</param>
	/// <param name="callback">データ処理を行うコールバック関数</param>
	protected void startUDPReceiver<T>(int port, Action<T> callback) where T : IJSONable<T> {
		// 受信用のUDPクライアントを生成（localhost）
		var udpClient = new UdpClient(port);

		// 非同期でデータ受信
		Debug.Log("非同期UDPデータ受信待ち:  " + "any:" + port);
		udpClient.BeginReceive(
			new AsyncCallback((async) => {
				var udp = (UdpClient)async.AsyncState;
				Debug.Log("非同期UDPデータ受信中: " + "any:" + port);

				// 実際にデータ受信
				byte[] receivedData = null;
				System.Net.IPEndPoint endPoint = null;
				try {

					// 非同期のデータ受信を終了し、得られたデータを保管する
					receivedData = udp.EndReceive(async, ref endPoint);

					// 先頭４バイトはデータの長さを示す
					var receivedDataCollection = new List<byte>();
					receivedDataCollection.AddRange(receivedData);
					int length = BitConverter.ToInt32(receivedDataCollection.GetRange(0, 4).ToArray(), 0);

					// 指示されたデータの長さと実際の長さを比較する
					if(length != receivedData.Length - 4) {
						throw new Exception("受信したデータがヘッダーに示された長さと一致しません。過不足＝" + (length - (receivedData.Length - 4)));
					}

					Debug.Log("非同期UDPデータ受信完了: " + receivedData.Length + " Bytes");

					// 受信したJSONをデータとして復元して、呼出元が定義したコールバックを呼び出す
					var obj = JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(receivedDataCollection.GetRange(4, receivedData.Length - 4).ToArray()));
					callback?.Invoke(obj);

				} catch(Exception e) {
					Debug.LogWarning("非同期UDPデータ受信エラー: " + e.Message);
				}
			}),
			udpClient
		);

	}

	/// <summary>
	/// UDPによるデータ送信を非同期的に行います。
	/// UDPなので必ずしも相手に届くとは限りません。
	/// </summary>
	/// <param name="ipAddress">接続先IPアドレス</param>
	/// <param name="port">接続先ポート番号</param>
	/// <param name="data">送信するデータ（JSONとしてシリアライズ可能なもの）</param>
	/// <param name="callback">送信が完了したときに呼び出されるコールバック関数</param>
	protected void startUDPSender(string ipAddress, int port, object data, Action callback) {
		// UDPクライアントを生成
		var udpClient = new UdpClient(
			new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ipAddress), port)
		);

		// 送信対象データはJSON形式に変換する
		var json = JsonUtility.ToJson(data);
		var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

		// 非同期でデータ送信
		Debug.Log("非同期UDP接続送信待ち: " + ipAddress + ":" + port);
		udpClient.BeginSend(
			jsonBinary,
			jsonBinary.Length,
			new AsyncCallback((async) => {
				var udp = (UdpClient)async.AsyncState;
				Debug.Log("非同期UDPデータ送信中: " + ipAddress + ":" + port);

				try {
					udp.EndSend(async);
				} catch(Exception e) {
					Debug.LogWarning("非同期UDPデータ送信エラー: " + e.Message);
					return;
				}
				Debug.Log("非同期UDPデータ送信完了: " + jsonBinary.Length + " Bytes");

				// 呼出元が定義したコールバック関数を呼び出す
				callback?.Invoke();
			}),
			udpClient
		);
	}

}
