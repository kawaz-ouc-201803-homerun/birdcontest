using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参加者の延べ人数を集計するAPIのレスポンスクラス
/// </summary>
[Serializable]
public class ModelAudienceGetPeopleCountResponse : IJSONable<ModelAudienceGetPeopleCountResponse> {

	/// <summary>
	/// 参加者の延べ人数
	/// </summary>
	public int count;

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelAudienceGetPeopleCountResponse FromJSON(string json) {
		return JsonUtility.FromJson<ModelAudienceGetPeopleCountResponse>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
