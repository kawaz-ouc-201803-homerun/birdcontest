using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// ゲームマスター側のTCP/UDP通信処理を行うラッパークラス
/// </summary>
public class NetworkGameMaster : NetworkConnector {

	/// <summary>
	/// HTTP通信でオーディエンス予想の新しいイベントを生成します。
	/// </summary>
	/// <returns>イベントID</returns>
	public string StartAudiencePredicts() {
		return this.postRequestWithResponseObject<ModelAudienceNewEventResponse>("new_event", null)?.EventId;
	}

	/// <summary>
	/// HTTP通信で現在有効なイベントのオーディエンス予想を取り出します。
	/// </summary>
	/// <returns>オーディエンス予想のリスト</returns>
	public ModelAudiencePredictList LoadAudiencePredicts() {
		return this.postRequestWithResponseObject<ModelAudiencePredictList>("get_posts", null);
	}

	/// <summary>
	/// HTTP通信で現在のイベントのオーディエンス予想を締め切ります。
	/// </summary>
	public System.Net.HttpStatusCode CloseAudiencePredicts() {
		return this.postRequestWithResponseCode("close", null);
	}

	/// <summary>
	/// HTTP通信でオーディエンスの参加延べ人数を取得します。
	/// </summary>
	public ModelAudienceGetPeopleCountResponse GetPeopleCount() {
		return this.postRequestWithResponseObject<ModelAudienceGetPeopleCountResponse>("get_people_count", null);
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
