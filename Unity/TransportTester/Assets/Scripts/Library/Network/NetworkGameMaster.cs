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
	/// UDPによる操作端末の進捗状況を受け付けるかどうか
	/// </summary>
	protected bool receivableUDPProgress = true;

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
		var result = this.postRequestWithResponseObject<ModelAudienceNewEventResponse>(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "newEvent",
				param = new string[] { },
				id = "1",
			}
		);
		if(result != null) {
			return result.eventId;
		} else {
			return null;
		}
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を取り出します。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <returns>オーディエンス予想のリスト</returns>
	public ModelAudiencePredictsResponse GetAudiencePredicts(string eventId) {
		return this.postRequestWithResponseObject<ModelAudiencePredictsResponse>(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "getPosts",
				param = new string[] { eventId },
				id = "2",
			}
		);
	}

	/// <summary>
	/// HTTP通信で指定したイベントのオーディエンス予想を締め切ります。
	/// </summary>
	/// <param name="eventId">イベントID</param>
	/// <returns>HTTPレスポンスコード</returns>
	public HttpStatusCode CloseAudiencePredicts(string eventId) {
		return this.postRequestWithResponseCode(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "close",
				param = new string[] { eventId },
				id = "3",
			}
		);
	}

	/// <summary>
	/// HTTP通信でオーディエンスの参加延べ人数を取得します。
	/// </summary>
	/// <returns>オーディエンスの参加延べ人数</returns>
	public int GetPeopleCount() {
		var result = this.postRequestWithResponseObject<ModelAudienceGetPeopleCountResponse>(
			NetworkGameMaster.HandlerJson,
			new ModelJsonicRequest() {
				method = "getPeopleCount",
				param = new string[] { },
				id = "4",
			}
		);
		if(result != null) {
			return result.count;
		} else {
			return -1;
		}
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
			this.startUDPReceiver(NetworkConnector.ControllerPorts[i], new Action<ModelControllerProgress>((obj) => {
				// データ受信時のコールバック処理

				if(this.receivableUDPProgress == false) {
					// UDP受信を受け付けていない場合は中止する
					return;
				}

				if(callback != null) {
					callback.Invoke(obj);
				}
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

		// すべての端末から受信待機
		for(int i = 0; i < this.ControllerIPAddresses.Length; i++) {
			this.startTCPServer(NetworkConnector.ControllerPorts[i], callback);
		}
	}

}
