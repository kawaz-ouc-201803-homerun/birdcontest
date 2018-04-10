using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// ゲームマスター側のHTTP/TCP/UDP通信処理を行うラッパークラス
/// </summary>
public class NetworkGameMaster : NetworkConnector {

	/// <summary>
	/// WebAPIのハンドラー名
	/// </summary>
	protected const string HandlerJson = "handler/GameMasterHandler.json";

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
				param = new object[] {},
				id = 1,
			}
		)?.eventId;
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
				param = new object[] {},
				id = 4,
			}
		).count;
	}

	/// <summary>
	/// TCPで各端末に開始指示を送信します。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="data">送信する任意のデータ</param>
	/// <param name="callback">通信が完了したときに呼び出されるコールバック関数</param>
	public void StartControllers(object data, Action callback) {
		// すべての端末に送信
		foreach(var ipAddress in NetworkConnector.ControllerIPAddresses) {
			this.startTCPClient(ipAddress, NetworkConnector.GeneralPort, data, callback);
		}
	}

	/// <summary>
	/// UDPによる各端末の進捗報告をゲームマスター側で受け取ります。
	/// ポートは共通のものを使います。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="callback">データを受信したときに呼び出されるコールバック関数</param>
	public void ReceiveControllerProgress<T>(Action<T> callback) where T : IJSONable<T> {
		this.startUDPReceiver<T>(NetworkConnector.GeneralPort, callback);
	}

	/// <summary>
	/// TCPでゲームマスターが各端末の操作の終了を待ち受けます。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="callback">受信が完了したときに呼び出されるコールバック関数</param>
	public void WaitForControllers<T>(Action<T> callback) where T : IJSONable<T> {
		// すべての端末に送信
		for(int i = 0; i < NetworkConnector.ControllerIPAddresses.Length; i++) {
			this.startTCPServer(NetworkConnector.ControllerPorts[i], callback);
		}
	}

}
