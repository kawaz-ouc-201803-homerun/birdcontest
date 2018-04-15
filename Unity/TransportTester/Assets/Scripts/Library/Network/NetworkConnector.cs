using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// HTTP/TCP/UDPの通信処理を行う基本クラス
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
	/// オーディエンス投票システムの基本URL
	/// </summary>
	protected const string AudienceSystemBaseURL = "http://tsownserver.dip.jp:8080/GameJamAudience/";

	/// <summary>
	/// ゲームマスターのIPアドレス
	/// </summary>
	protected string GameMasterIPAddress = "192.168.11.2";

	/// <summary>
	/// 各種操作端末のIPアドレス
	/// </summary>
	protected string[] ControllerIPAddresses = new string[] {
		"192.168.11.10",
		"192.168.11.11",
		"192.168.11.12",
	};

	/// <summary>
	/// ゲームマスターが使用するポート番号
	/// ゲームマスターとなる端末は、これらすべてのポートを開放する必要があります。
	/// ただし、同じLANの中にいる場合はポート開放の必要はありません。
	/// </summary>
	protected static readonly int[] ControllerPorts = new int[] {
		30000,
		30001,
		30002,
	};

	/// <summary>
	/// コネクション確立を待機する秒数
	/// </summary>
	public const int ConnectionWaitSecondsForConnect = 5;

	/// <summary>
	/// 非同期でストリーム受信するときに必要なバッファーオブジェクト
	/// </summary>
	public class AsyncReceiveBuffer {

		/// <summary>
		/// TCPソケット
		/// </summary>
		public Socket TCPSocket;

		/// <summary>
		/// 読み込みバッファー
		/// </summary>
		public byte[] Buffer;

	}

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="gameMasterIPAddress">ゲームマスターのIPアドレス。nullにするとサトの環境デフォルト設定になります。</param>
	/// <param name="controllerIPAddresses">各種操作端末のIPアドレス。nullにするとサトの環境デフォルト設定になります。</param>
	public NetworkConnector(string gameMasterIPAddress, string[] controllerIPAddresses) {
		if(gameMasterIPAddress != null) {
			this.GameMasterIPAddress = gameMasterIPAddress;
		}
		if(controllerIPAddresses != null) {
			this.ControllerIPAddresses = controllerIPAddresses;
		}
	}

	/// <summary>
	/// WebAPIにリクエストを送信して任意の型のレスポンスを受信します。
	/// </summary>
	/// <typeparam name="T">受信するレスポンスの型</typeparam>
	/// <param name="url">リクエスト送信先URL</param>
	/// <param name="parameter">リクエストとして送信するパラメーター</param>
	/// <returns>レスポンスとして受信したオブジェクト。JSON形式からのデシリアライズに失敗した場合は null を返す</returns>
	protected T postRequestWithResponseObject<T>(string url, ModelHttpRequest parameter) where T : IJSONable<T> {
		if(parameter == null) {
			throw new ArgumentNullException("parameter", "WebAPIのリクエストパラメーターは必須です。");
		}

		var httpRequest = HttpWebRequest.Create(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = JsonUtility.ToJson(parameter);

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		using(var W = new System.IO.StreamWriter(httpRequest.GetRequestStream())) {
			W.Write(json);
		}

		// HTTPリクエスト送信、サーバーからレスポンスを受け取る
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
	protected HttpStatusCode postRequestWithResponseCode(string url, ModelHttpRequest parameter) {
		if(parameter == null) {
			throw new ArgumentNullException("parameter", "WebAPIのリクエストパラメーターは必須です。");
		}

		var httpRequest = HttpWebRequest.Create(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = JsonUtility.ToJson(parameter);

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		using(var W = new System.IO.StreamWriter(httpRequest.GetRequestStream())) {
			W.Write(json);
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
		var tcpListener = new TcpListener(IPAddress.Any, port);

		// 非同期で接続待機開始
		Debug.Log("非同期TCP接続待ち (Server): " + "any:" + port);
		tcpListener.Start();
		tcpListener.BeginAcceptSocket(
			new AsyncCallback((asyncSocket) => {
				// 待ち受けを終了する
				var listener = asyncSocket.AsyncState as TcpListener;
				var tcpSocket = listener.EndAcceptSocket(asyncSocket);
				Debug.Log("非同期TCP接続受け入れ: " + tcpSocket.RemoteEndPoint.ToString());

				// 非同期の通信ストリームでデータを受け取る
				Debug.Log("非同期TCPデータ受信中: " + "any:" + port);
				var socketReader = new AsyncReceiveBuffer() {
					TCPSocket = tcpSocket,
					Buffer = new byte[1024],
				};
				tcpSocket.BeginReceive(socketReader.Buffer, 0, socketReader.Buffer.Length, SocketFlags.None, new AsyncCallback((asyncReader) => {
					Debug.Log("非同期TCPデータ受信OK: " + "any:" + port);
					var reader = asyncReader.AsyncState as AsyncReceiveBuffer;
					var buf = reader.Buffer;

					var jsonBinaryData = new byte[1024];
					try {
						int receiveLength = reader.TCPSocket.EndReceive(asyncReader);

						// 先頭４バイトはデータの長さが入っている
						var sizeBuf = new byte[4];
						Array.Copy(buf, 0, sizeBuf, 0, 4);
						int length = BitConverter.ToInt32(sizeBuf, 0);

						// 残りをすべてJSONの実データとして読み込む
						if(receiveLength != length + 4) {
							throw new Exception("受信したデータがヘッダーに示された長さと一致しません。過不足＝" + (receiveLength - (length + 4)));
						}
						Array.Copy(buf, 4, jsonBinaryData, 0, buf.Length - 4);

						Debug.Log("非同期TCPデータ受信完了: " + receiveLength + " Bytes");

					} catch(Exception e) {
						Debug.LogWarning("非同期TCPデータ受信エラー: " + e.Message);
						return;
					}

					// 受信したJSONをデータとして復元して、呼出元が定義したコールバックを呼び出す
					var obj = JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(jsonBinaryData));
					if(callback != null) {
						callback.Invoke(obj);
					}

				}), socketReader);
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
	/// <param name="successCallback">送信が完了したときに呼び出されるコールバック関数</param>
	/// <param name="failureCallBack">通信に失敗したときに呼び出されるコールバック関数</param>
	protected void startTCPClient(string ipAddress, int port, object data, Action successCallback, Action failureCallBack) {
		// 非同期で接続開始
		Debug.Log("非同期TCP接続待ち (Client): " + ipAddress + ":" + port);
		var tcpClient = new TcpClient();

		var result = tcpClient.BeginConnect(
			IPAddress.Parse(ipAddress),
			port,
			new AsyncCallback((async) => {
				// 送信準備完了
				var tcp = async.AsyncState as TcpClient;
				if(tcp.Connected == false) {
					// 接続拒否された等で切断状態になった
					Debug.LogWarning("非同期TCPコネクションの確立に失敗しました - Refused (Client)");
					tcp.Close();

					// 呼出元が定義したコールバック関数を呼び出す
					if(failureCallBack != null) {
						failureCallBack.Invoke();
					}

					return;
				}

				// データを通信用の形式に変換して送信
				Debug.Log("非同期TCPデータ送信中: " + tcp.Client.RemoteEndPoint.ToString());
				var dataBinary = this.createDataForTransport(data);
				try {
					using(var stream = tcp.GetStream()) {
						stream.Write(dataBinary, 0, dataBinary.Length);
						Debug.Log("非同期TCPデータ送信完了: " + (dataBinary.Length) + " Bytes");

						// 呼出元が定義したコールバック関数を呼び出す
						if(successCallback != null) {
							successCallback.Invoke();
						}
					}

					// 非同期の接続を終了
					tcp.EndConnect(async);

				} catch(Exception e) {
					Debug.LogWarning("非同期TCPデータ送信エラー: " + e.Message);

					// 呼出元が定義したコールバック関数を呼び出す
					if(failureCallBack != null) {
						failureCallBack.Invoke();
					}
				}
			}),
			tcpClient
		);

		//if(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(NetworkConnector.ConnectionWaitSecondsForConnect)) == false) {
		//	// タイムアウトにより、コネクション確立に失敗
		//	Debug.LogWarning("非同期TCPコネクション確立に失敗しました - Timeout (Client)");
		//	tcpClient.Close();

		//	// 呼出元が定義したコールバック関数を呼び出す
		//	if(failureCallBack != null) {
		//		failureCallBack.Invoke();
		//	}
		//}
	}

	/// <summary>
	/// UDPによるデータ受信を非同期的に行います。
	/// UDPなので必ずしも受信できるとは限りません。
	/// </summary>
	/// <param name="udpClient">受信用UDPクライアントオブジェクト。初回送信時のみnullでOK</param>
	/// <param name="port">使用するポート番号</param>
	/// <param name="callback">データ処理を行うコールバック関数</param>
	/// <returns>受信用UDPクライアントオブジェクト</returns>
	protected UdpClient startUDPReceiver<T>(UdpClient udpClient, int port, Action<T> callback) where T : IJSONable<T> {
		// 受信用のUDPクライアントを生成（localhost）
		if(udpClient == null) {
			// UDPクライアントが指定されていない場合は新規生成
			udpClient = new UdpClient(port);
		}

		// 非同期でデータ受信
		Debug.Log("非同期UDPデータ受信待ち:  " + "any:" + port);
		udpClient.BeginReceive(
			new AsyncCallback((async) => {
				var udp = async.AsyncState as UdpClient;
				Debug.Log("非同期UDPデータ受信中: " + "any:" + port);

				// 実際にデータ受信
				byte[] receivedData = null;
				IPEndPoint endPoint = null;
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
					if(callback != null) {
						callback.Invoke(obj);
					}

				} catch(Exception e) {
					Debug.LogWarning("非同期UDPデータ受信エラー: " + e.Message);
				}
			}),
			udpClient
		);

		return udpClient;
	}

	/// <summary>
	/// UDPによるデータ送信を非同期的に行います。
	/// UDPなので必ずしも相手に届くとは限りません。
	/// </summary>
	/// <param name="udpClient">送信用UDPクライアントオブジェクト。初回送信時のみnullでOK</param>
	/// <param name="ipAddress">接続先IPアドレス</param>
	/// <param name="port">接続先ポート番号</param>
	/// <param name="data">送信するデータ（JSONとしてシリアライズ可能なもの）</param>
	/// <param name="callback">送信が完了したときに呼び出されるコールバック関数</param>
	/// <returns>送信用UDPクライアントオブジェクト</returns>
	protected UdpClient startUDPSender(UdpClient udpClient, string ipAddress, int port, object data, Action callback) {
		if(udpClient == null) {
			// UDPクライアントが指定されていない場合は新規生成
			var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
			udpClient = new UdpClient();

			// UDPは送信側のみソケット接続が必要となる
			udpClient.Connect(endPoint);
		}

		// データを通信用の形式に変換して送信
		var dataBinary = this.createDataForTransport(data);

		// 非同期でデータ送信
		Debug.Log("非同期UDP接続送信待ち: " + ipAddress + ":" + port);
		udpClient.BeginSend(
			dataBinary,
			dataBinary.Length,
			new AsyncCallback((async) => {
				var udp = async.AsyncState as UdpClient;
				Debug.Log("非同期UDPデータ送信中: " + ipAddress + ":" + port);

				try {
					int length = udp.EndSend(async);
					Debug.Log("非同期UDPデータ送信完了: " + length + " Bytes");
				} catch(Exception e) {
					Debug.LogWarning("非同期UDPデータ送信エラー: " + e.Message);
					return;
				}

				// 呼出元が定義したコールバック関数を呼び出す
				if(callback != null) {
					callback.Invoke();
				}
			}),
			udpClient
		);

		return udpClient;
	}

	/// <summary>
	/// 指定したオブジェクトをもとに、通信用のバイナリデータを作成します。
	/// ここでいう通信用のバイナリデータとは、先頭４バイトが後ろに続くデータの長さを示す形式のデータのことを指します。
	/// </summary>
	/// <param name="obj">対象オブジェクト</param>
	/// <returns>通信用のバイナリデータ</returns>
	protected byte[] createDataForTransport(object obj) {
		var json = JsonUtility.ToJson(obj);
		var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

		using(var stream = new MemoryStream()) {
			// 先頭４バイトはデータの長さを入れる
			stream.Write(BitConverter.GetBytes(jsonBinary.Length), 0, 4);

			// 残りすべてをJSONの実データとして入れる
			stream.Write(jsonBinary, 0, jsonBinary.Length);

			return stream.ToArray();
		}
	}

}
