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
	protected const string HandlerJson = "handler/GameMaster.json";

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
	/// <param name="callback">処理完了後に呼び出されるコールバック関数。イベントIDを受け取るのに使用する</param>
	public void StartAudiencePredicts(Action<string> callback) {
		this.postRequestWithResponseObject(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "newEvent",
				param = new string[] { },
				id = "1",
			},
			new Action<ModelAudienceNewEventResponse>((result) => {
				if(callback == null) {
					return;
				}

				// コールバック関数を呼び出してイベントIDを渡す
				if(result != null) {
					callback.Invoke(result.eventId);
				} else {
					callback.Invoke(null);
				}
			})
		);
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を取り出します。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <param name="callback">処理完了後に呼び出されるコールバック関数。オーディエンス予想データを受け取るのに使用する</param>
	public void GetAudiencePredicts(string eventId, Action<ModelAudiencePredictsResponse> callback) {
		this.postRequestWithResponseObject(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "getPosts",
				param = new string[] { eventId },
				id = "2",
			},
			new Action<ModelAudiencePredictsResponse>((result) => {
				if(callback == null) {
					return;
				}

				// コールバック関数を呼び出してオーディエンス予想データを渡す
				if(result != null) {
					callback.Invoke(result);
				} else {
					callback.Invoke(null);
				}
			})
		);
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を締め切ります。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <param name="callback">処理完了後に呼び出されるコールバック関数。HTTPレスポンスコードを受け取るのに使用する</param>
	public void CloseAudiencePredicts(string eventId, Action<HttpStatusCode> callback) {
		this.postRequestWithResponseCode(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "close",
				param = new string[] { eventId },
				id = "3",
			},
			new Action<HttpStatusCode>((result) => {
				if(callback != null) {
					// コールバック関数を呼び出してステータスコードを渡す
					callback.Invoke(result);
				}
			})
		);
	}

	/// <summary>
	/// HTTP通信でオーディエンスの参加延べ人数を取得します。
	/// </summary>
	/// <param name="callback">処理完了後に呼び出されるコールバック関数。参加延べ人数を受け取るのに使用する</param>
	public void GetPeopleCount(Action<int> callback) {
		this.postRequestWithResponseObject(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "getPeopleCount",
				param = new string[] { },
				id = "4",
			},
			new Action<ModelAudienceGetPeopleCountResponse>((result) => {
				if(callback == null) {
					return;
				}

				// コールバック関数を呼び出して参加延べ人数を渡す
				if(result != null) {
					callback.Invoke(result.count);
				} else {
					callback.Invoke(-1);
				}
			})
		);
	}

	/// <summary>
	/// TCPで指定した端末に開始指示を送信します。
	/// ポートは端末個別のものを使います。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="data">送信する任意のデータ</param>
	/// <param name="roleId">役割ID</param>
	/// <param name="successCallBack">通信が完了したときに呼び出されるコールバック関数</param>
	/// <param name="failureCallBack">通信に失敗したときに呼び出されるコールバック関数</param>
	public void StartController(ModelControllerStart data, int roleId, Action successCallBack, Action failureCallBack) {
		this.startTCPClient(this.ControllerIPAddresses[roleId], NetworkConnector.StartingControllerPorts[roleId], data, successCallBack, failureCallBack);
	}

	/// <summary>
	/// UDPによる端末の進捗報告をゲームマスター側で受け取ります。
	/// ポートは端末個別のものを使います。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="roleId">対象端末の役割ID</param>
	/// <param name="callback">データを受信したときに呼び出されるコールバック関数</param>
	public void ReceiveControllerProgress(int roleId, Action<ModelControllerProgress> callback) {
		this.startUDPReceiver(NetworkConnector.ProgressToGameMasterPorts[roleId], callback);
	}

	/// <summary>
	/// TCPでゲームマスターが端末の操作の終了を待ち受けます。
	/// ポートは端末個別のものを使います。
	/// このメソッドはノンブロッキングで通過するため、通信が完了したときに呼び出される処理をコールバック関数で指定する必要があります。
	/// </summary>
	/// <param name="roleId">対象端末の役割ID</param>
	/// <param name="callback">受信が完了したときに呼び出されるコールバック関数</param>
	public void WaitForControllers(int roleId, Action<ModelControllerProgress> callback) {
		this.startTCPServer(NetworkConnector.ProgressToGameMasterPorts[roleId], callback);
	}

}
