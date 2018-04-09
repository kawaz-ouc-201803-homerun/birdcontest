using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新規イベントを作成するAPIのレスポンスクラス
/// </summary>
[Serializable]
public class ModelAudienceNewEventResponse : IJSONable<ModelAudienceNewEventResponse> {

	/// <summary>
	/// イベントID
	/// </summary>
	public string EventId;

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelAudienceNewEventResponse FromJSON(string json) {
		return JsonUtility.FromJson<ModelAudienceNewEventResponse>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
