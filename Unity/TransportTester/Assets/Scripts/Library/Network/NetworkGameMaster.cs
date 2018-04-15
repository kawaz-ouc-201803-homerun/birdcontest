using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ModelControllerProgress = ModelDictionary<string, string>;

/// <summary>
/// ゲームマスター側のHTTP/TCP/UDP通信処理を行うラッパークラス
/// </summary>
public class NetworkGameMaster : NetworkConnector {

	/// <summary>
	/// WebAPIのハンドラー名
	/// </summary>
	protected const string HandlerJson = "handler/GameMasterHandler.json";

	/// <summary>
	/// UDPによる操作端末の進捗状況を受け付けるかどうか
	/// </summary>
	protected bool receivableUDPProgress = true;

	/// <summary>
	/// 受信用UDPクライアント
	/// </summary>
	private UdpClient udpClient = null;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="controllerIPAddresses">各種操作端末のIPアドレス</param>
	public NetworkGameMaster(string[] controllerIPAddresses) : base("localhost", controllerIPAddresses) {
	}

	/// <summary>
	/// HTTP通信でオーディエンス予想の新しいイベントを生成します。
	/// 既存のイベントは自動的にすべて締め切られます。
	/// </summary>
	/// <returns>イベントID</returns>
	public string StartAudiencePredicts() {
		return this.postRequestWithResponseObject<ModelAudienceNewEventResponse>(
			NetworkConnector.AudienceSystemBaseURL + NetworkGameMaster.HandlerJson,
			new ModelHttpRequest() {
				method = "newEvent",
				param = new object[] { },
				id = 1,
			}
		).eventId;
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を取り出します。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <returns>オーディエンス予想のリスト</returns>
	public List<ModelAudiencePredict> LoadAudiencePredicts(string eventId) {
		return this.postRequestWithResponseObject<ModelAudiencePredictList>(
			NetworkConnector.AudienceSystemBaseURL + NetworkGameMaster.HandlerJson,
			new ModelHttpRequest() {
				method = "getPosts",
				param = new object[] { eventId },
				id = 2,
			}
		).audiencePredicts;
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を締め切ります。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <returns>HTTPレスポンスコード</returns>
	public HttpStatusCode CloseAudiencePredicts(string eventId) {
		return this.postRequestWithResponseCode(
			NetworkConnector.AudienceSystemBaseURL + NetworkGameMaster.HandlerJson,
			new ModelHttpRequest() {
				method = "close",
				param = new object[] { eventId },
				id = 3,
			}
		);
	}

	/// <summary>
	/// HTTP通信でオーディエンスの参加延べ人数を取得します。
	/// </summary>
	/// <returns>オーディエンスの参加延べ人数</returns>
	public int GetPeopleCount() {
		return this.postRequestWithResponseObject<ModelAudienceGetPeopleCountResponse>(
			NetworkConnector.AudienceSystemBaseURL + NetworkGameMaster.HandlerJson,
			new ModelHttpRequest() {
				method = "getPeopleCount",
				param = new object[] { },
				id = 4,
			}
		).count;
	}

	/// <summary>
	/// TCPで指定した端末に開始指示を送信します。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="data">送信する任意のデータ</param>
	/// <param name="roleId">役割ID</param>
	/// <param name="successCallBack">通信が完了したときに呼び出されるコールバック関数</param>
	/// <param name="failureCallBack">通信に失敗したときに呼び出されるコールバック関数</param>
	public void StartController(ModelControllerStart data, int roleId, Action successCallBack, Action failureCallBack) {
		this.startTCPClient(this.ControllerIPAddresses[roleId], NetworkConnector.GeneralPort, data, successCallBack, failureCallBack);
	}

	/// <summary>
	/// UDPによる各端末の進捗報告をゲームマスター側で受け取ります。
	/// ポートは共通のものを使います。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="callback">データを受信したときに呼び出されるコールバック関数</param>
	public void ReceiveControllerProgress(Action<ModelControllerProgress> callback) {
		this.receivableUDPProgress = true;

		// すべての端末から受信
		for(int i = 0; i < this.ControllerIPAddresses.Length; i++) {
			this.udpClient = this.startUDPReceiver(this.udpClient, NetworkConnector.ControllerPorts[i], new Action<ModelControllerProgress>((obj) => {
				if(this.receivableUDPProgress == false) {
					// UDP受信を受け付けていない場合は中止する
					return;
				}

				callback.Invoke(obj);
				this.ReceiveControllerProgress(callback);
			}));
		}
	}

	/// <summary>
	/// TCPでゲームマスターが各端末の操作の終了を待ち受けます。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="callback">受信が完了したときに呼び出されるコールバック関数</param>
	public void WaitForControllers(Action<ModelControllerProgress> callback) {
		// UDPでの受信を止める
		this.receivableUDPProgress = false;
		this.udpClient = null;

		// すべての端末から受信待機
		for(int i = 0; i < this.ControllerIPAddresses.Length; i++) {
			this.startTCPServer(NetworkConnector.ControllerPorts[i], callback);
		}
	}

}
