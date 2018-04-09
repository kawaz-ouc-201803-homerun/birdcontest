using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投票可能かどうかをチェックするAPIのレスポンスクラス
/// </summary>
[Serializable]
public class ModelAudienceIsPostableResponse : IJSONable<ModelAudienceIsPostableResponse> {

	/// <summary>
	/// 投票可能かどうか: 0=NG、1=OK
	/// </summary>
	public int Result;

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelAudienceIsPostableResponse FromJSON(string json) {
		return JsonUtility.FromJson<ModelAudienceIsPostableResponse>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
