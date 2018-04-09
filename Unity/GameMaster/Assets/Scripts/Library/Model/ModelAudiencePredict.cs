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
	public string Id;

	/// <summary>
	/// イベントID
	/// </summary>
	public string EventId;

	/// <summary>
	/// 投稿者名
	/// </summary>
	public string AudienceName;

	/// <summary>
	/// 予想値
	/// </summary>
	public int Predict;

	/// <summary>
	/// 投稿日時
	/// </summary>
	public Time ReceiveDate;

	/// <summary>
	/// 送信元IPアドレス
	/// </summary>
	public string IPAddress;

	/// <summary>
	/// 送信元の機種・OS名など
	/// </summary>
	public string OSEnv;

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
