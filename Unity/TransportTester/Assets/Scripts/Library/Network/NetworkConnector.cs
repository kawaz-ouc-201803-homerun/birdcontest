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
	/// コネクション確立を待機する秒数
	/// </summary>
	public const int ConnectionWaitSecondsForConnect = 5;

	/// <summary>
	/// 共通ポート番号
	/// </summary>
	protected const int GeneralPort = 30100;

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
	/// 受信用のUDPクライアント
	/// キーはポート番号
	/// 受信は必ずしも成功しないため、前回実行時のオブジェクトを保持しておく必要があります。
	/// </summary>
	protected Dictionary<int, AsyncResource<UdpClient>> udpReceiveClients = new Dictionary<int, AsyncResource<UdpClient>>();

	/// <summary>
	/// 受信用のTCPリスナー
	/// キーはポート番号
	/// 受信は必ずしも成功しないため、前回実行時のオブジェクトを保持しておく必要があります。
	/// </summary>
	protected Dictionary<int, AsyncResource<TcpListener>> tcpReceiveListeners = new Dictionary<int, AsyncResource<TcpListener>>();

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
	protected T postRequestWithResponseObject<T>(string url, ModelJsonicRequest parameter) where T : IJSONable<T> {
		if(parameter == null) {
			throw new ArgumentNullException("parameter", "WebAPIのリクエストパラメーターは必須です。");
		}

		var httpRequest = HttpWebRequest.Create(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = JsonUtility.ToJson(parameter);

		// JSONのうち、厳密には param -> params (C#の予約語につき変数名にできない) なので直す
		json = json.Replace("\"param\":", "\"params\":");

		// JSONをバイト配列化
		var bytes = System.Text.Encoding.UTF8.GetBytes(json);

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		httpRequest.ContentLength = bytes.Length;
		httpRequest.GetRequestStream().Write(bytes, 0, bytes.Length);

		// HTTPリクエスト送信、サーバーからレスポンスを受け取る
		using(var httpResponse = (HttpWebResponse)httpRequest.GetResponse()) {
			var buf = "";
			using(var R = httpResponse.GetResponseStream()) {
				var readBuffer = new byte[1024];
				int length = 0;
				while((length = R.Read(readBuffer, 0, readBuffer.Length)) > 0) {
					buf += System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
				}
			}
			json = buf;
		}

		try {

			// 受信したJSONレスポンスをデシリアライズ
			var jsonicResponse = JsonUtility.FromJson<ModelJsonicResponse<T>>(json);

			// JSONICエラーチェック
			if(jsonicResponse.error != null
			&& (jsonicResponse.error.code != 0 || string.IsNullOrEmpty(jsonicResponse.error.message) == false)) {
				// エラーがある場合は戻り値を返さない
				throw new Exception("JSONICエラー情報: [" + jsonicResponse.error.code + "] " + jsonicResponse.error.message);
			}

			// メソッドの戻り値を得る
			return jsonicResponse.result;

		} catch(Exception e) {
			Debug.LogWarning("HTTPレスポンスエラー: " + e.Message);
			return default(T);
		}
	}

	/// <summary>
	/// WebAPIにリクエストを送信してステータスコードを確認します。
	/// </summary>
	/// <param name="url">リクエスト送信先URL</param>
	/// <param name="parameter">リクエストとして送信するオブジェクト</param>
	/// <returns>HTTPステータスコード</returns>
	protected HttpStatusCode postRequestWithResponseCode(string url, ModelJsonicRequest parameter) {
		if(parameter == null) {
			throw new ArgumentNullException("parameter", "WebAPIのリクエストパラメーターは必須です。");
		}

		var httpRequest = HttpWebRequest.Create(NetworkController.AudienceSystemBaseURL + url);

		// 送信するデータはJSON化する
		string json = JsonUtility.ToJson(parameter);

		// JSONのうち、厳密には param -> params (C#の予約語につき変数名にできない) なので直す
		json = json.Replace("\"param\":", "\"params\":");

		// JSONをバイト配列化
		var bytes = System.Text.Encoding.UTF8.GetBytes(json);

		// HTTPリクエストを作成
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/json";
		httpRequest.ContentLength = bytes.Length;
		httpRequest.GetRequestStream().Write(bytes, 0, bytes.Length);

		// HTTPリクエスト送信、サーバーからレスポンスを受け取る
		using(var httpResponse = (HttpWebResponse)httpRequest.GetResponse()) {
			var buf = new StringWriter();
			using(var R = httpResponse.GetResponseStream()) {
				var readBuffer = new byte[1024];
				int length = 0;
				while((length = R.Read(readBuffer, 0, readBuffer.Length)) > 0) {
					buf.Write(System.Text.Encoding.UTF8.GetString(readBuffer, 0, length));
				}
			}
			json = buf.ToString();

			try {

				// 受信したJSONレスポンスをデシリアライズ
				var jsonicResponse = JsonUtility.FromJson<ModelJsonicResponse<object>>(json);

				// JSONICエラーチェック
				if(jsonicResponse.error != null
				&& (jsonicResponse.error.code != 0 || string.IsNullOrEmpty(jsonicResponse.error.message) == false)) {
					throw new Exception("JSONICエラー情報: [" + jsonicResponse.error.code + "] " + jsonicResponse.error.message);
				}

			} catch(Exception e) {
				Debug.LogWarning("HTTPレスポンスエラー: " + e.Message);
			}
			return httpResponse.StatusCode;
		}
	}

	/// <summary>
	/// すべてのTCP/UDPコネクションを破棄します。
	/// </summary>
	public void CloseConnectionsAll() {
		// 受信用TCPリスナー
		foreach(var tcp in this.tcpReceiveListeners) {
			tcp.Value.Resource.Server.Close();
		}
		this.tcpReceiveListeners.Clear();

		// 受信用UDPクライアント
		foreach(var udp in this.udpReceiveClients) {
			udp.Value.Resource.Close();
		}
		this.udpReceiveClients.Clear();
	}

	/// <summary>
	/// データ受信のみを行うTCPサーバーとして非同期的に接続を待ち受けます。
	/// </summary>
	/// <typeparam name="T">受信するデータの型</typeparam>
	/// <param name="port">使用するポート番号</param>
	/// <param name="callback">データ処理を行うコールバック関数</param>
	protected void startTCPServer<T>(int port, Action<T> callback) where T : IJSONable<T> {
		if(this.tcpReceiveListeners.ContainsKey(port) == true) {
			// 既に同じポートが使われている場合はスキップ
			Debug.Log("既にTCPポート #" + port + " が使用中です。");
			return;
		}
		this.tcpReceiveListeners[port] = new AsyncResource<TcpListener>() {
			Resource = new TcpListener(IPAddress.Any, port)
		};

		// 非同期で接続待機開始
		Debug.Log("非同期TCP接続待ち (Server): " + "any:" + port);
		this.tcpReceiveListeners[port].Resource.Start();
		this.tcpReceiveListeners[port].AsyncResult = this.tcpReceiveListeners[port].Resource.BeginAcceptSocket(
			new AsyncCallback((asyncSocket) => {
				// 待ち受けを終了する
				this.tcpReceiveListeners.Remove(port);
				var listener = (asyncSocket.AsyncState as AsyncResource<TcpListener>).Resource;
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

					using(reader.TCPSocket) {
						try {
							// 受信を終了し、受信したデータの実際の長さを取得
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
						} finally {
							// ソケットを切断
							listener.Server.Close();
						}
					}

					// 受信したJSONをデータとして復元して、呼出元が定義したコールバックを呼び出す
					var obj = JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(jsonBinaryData));
					if(callback != null) {
						try {
							callback.Invoke(obj);
						} catch(Exception e) {
							Debug.LogWarning("コールバック関数内の例外: " + e.Message);
						}
					}

				}), socketReader);
			}),
			this.tcpReceiveListeners[port]
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

		tcpClient.BeginConnect(
			IPAddress.Parse(ipAddress),
			port,
			new AsyncCallback((async) => {
				// 送信準備完了
				var tcp = async.AsyncState as TcpClient;

				// 通信エラーチェック
				if(tcp.Connected == false) {
					// 接続拒否された等で切断状態になった
					Debug.LogWarning("非同期TCPコネクションの確立に失敗しました - Refused (Client)");
					tcp.Close();

					// 失敗：呼出元が定義したコールバック関数を呼び出す
					if(failureCallBack != null) {
						try {
							failureCallBack.Invoke();
						} catch(Exception e) {
							Debug.LogWarning("コールバック関数内の例外: " + e.Message);
						}
					}

					return;
				}

				// データを通信用の形式に変換して送信
				Debug.Log("非同期TCPデータ送信中: " + tcp.Client.RemoteEndPoint.ToString());
				var dataBinary = this.createDataForTransport(data);
				try {
					using(tcp) {
						using(var stream = tcp.GetStream()) {
							stream.Write(dataBinary, 0, dataBinary.Length);
							Debug.Log("非同期TCPデータ送信完了: " + (dataBinary.Length) + " Bytes");
						}
					}

					// 成功：呼出元が定義したコールバック関数を呼び出す
					if(successCallback != null) {
						try {
							successCallback.Invoke();
						} catch(Exception e) {
							Debug.LogWarning("コールバック関数内の例外: " + e.Message);
						}
					}
				} catch(Exception e) {
					Debug.LogWarning("非同期TCPデータ送信エラー: " + e.Message);

					// 失敗：呼出元が定義したコールバック関数を呼び出す
					if(failureCallBack != null) {
						try {
							failureCallBack.Invoke();
						} catch(Exception e2) {
							Debug.LogWarning("コールバック関数内の例外: " + e2.Message);
						}
					}
				}
			}),
			tcpClient
		);
	}

	/// <summary>
	/// UDPによるデータ受信を非同期的に行います。
	/// UDPなので必ずしも受信できるとは限りません。
	/// </summary>
	/// <param name="udpClient">受信用UDPクライアントオブジェクト。初回送信時のみnullでOK</param>
	/// <param name="port">使用するポート番号</param>
	/// <param name="callback">データ処理を行うコールバック関数</param>
	protected void startUDPReceiver<T>(int port, Action<T> callback) where T : IJSONable<T> {
		// 受信用のUDPクライアントを生成（localhost）
		if(this.udpReceiveClients.ContainsKey(port) == true) {
			// 前回のUDPクライアントが残っている場合は処理をスキップ
			Debug.Log("非同期UDPクライアント #" + port + " が使用中です。");
			return;
		}
		this.udpReceiveClients[port] = new AsyncResource<UdpClient>() {
			Resource = new UdpClient(port)
		};

		// 非同期でデータ受信
		Debug.Log("非同期UDPデータ受信待ち:  " + "any:" + port);
		this.udpReceiveClients[port].AsyncResult = this.udpReceiveClients[port].Resource.BeginReceive(
			new AsyncCallback((async) => {
				this.udpReceiveClients.Remove(port);
				byte[] receivedData = null;
				var receivedDataCollection = new List<byte>();

				using(var udp = (async.AsyncState as AsyncResource<UdpClient>).Resource) {
					// 受信準備完了

					// 通信エラーチェック
					if(udp.Client.Connected == false) {
						Debug.LogWarning("非同期UDPの受信に失敗しました - Packet Loss");
						udp.Close();
						return;
					}

					Debug.Log("非同期UDPデータ受信中: " + "any:" + port);

					// 実際にデータ受信
					IPEndPoint endPoint = null;
					try {
						// 非同期のデータ受信を終了し、得られたデータを保管する
						receivedData = udp.EndReceive(async, ref endPoint);

						// 先頭４バイトはデータの長さを示す
						receivedDataCollection.AddRange(receivedData);
						int length = BitConverter.ToInt32(receivedDataCollection.GetRange(0, 4).ToArray(), 0);

						// 指示されたデータの長さと実際の長さを比較する
						if(length != receivedData.Length - 4) {
							throw new Exception("受信したデータがヘッダーに示された長さと一致しません。過不足＝" + (length - (receivedData.Length - 4)));
						}

						Debug.Log("非同期UDPデータ受信完了: " + receivedData.Length + " Bytes");

					} catch(Exception e) {
						Debug.LogWarning("非同期UDPデータ受信エラー: " + e.Message);
					}
				}

				// 受信したJSONをデータとして復元して、呼出元が定義したコールバックを呼び出す
				var obj = JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(receivedDataCollection.GetRange(4, receivedData.Length - 4).ToArray()));
				if(callback != null) {
					try {
						callback.Invoke(obj);
					} catch(Exception e) {
						Debug.LogWarning("コールバック関数内の例外: " + e.Message);
					}
				}
			}),
			this.udpReceiveClients[port]
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
		// 送信用のUDPクライアントを生成
		var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
		var udpClient = new UdpClient();

		// UDPは送信側のみソケット接続が必要となる
		udpClient.Connect(endPoint);

		// データを通信用の形式に変換して送信
		var dataBinary = this.createDataForTransport(data);

		// 非同期でデータ送信
		Debug.Log("非同期UDP接続送信待ち: " + ipAddress + ":" + port);
		udpClient.BeginSend(
			dataBinary,
			dataBinary.Length,
			new AsyncCallback((async) => {
				using(var udp = async.AsyncState as UdpClient) {
					Debug.Log("非同期UDPデータ送信中: " + ipAddress + ":" + port);

					try {
						int length = udp.EndSend(async);
						Debug.Log("非同期UDPデータ送信完了: " + length + " Bytes");
					} catch(Exception e) {
						Debug.LogWarning("非同期UDPデータ送信エラー: " + e.Message);
						return;
					}
				}

				// 呼出元が定義したコールバック関数を呼び出す
				if(callback != null) {
					try {
						callback.Invoke();
					} catch(Exception e) {
						Debug.LogWarning("コールバック関数内の例外: " + e.Message);
					}
				}
			}),
			udpClient
		);
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
