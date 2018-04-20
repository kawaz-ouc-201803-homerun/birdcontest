using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オーディエンス予想クラス
/// </summary>
[Serializable]
public class ModelAudiencePredict : IJSONable<ModelAudiencePredict> {

	/// <summary>
	/// 投稿ID
	/// </summary>
	public string id;

	/// <summary>
	/// イベントID
	/// </summary>
	public string eventId;

	/// <summary>
	/// 投稿者名
	/// </summary>
	public string nickname;

	/// <summary>
	/// 予想値
	/// </summary>
	public int predict;

	/// <summary>
	/// 投稿日時
	/// </summary>
	public string receiveTime;

	/// <summary>
	/// ユーザーセッションID
	/// </summary>
	public string userSessionId;

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelAudiencePredict FromJSON(string json) {
		return JsonUtility.FromJson<ModelAudiencePredict>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
