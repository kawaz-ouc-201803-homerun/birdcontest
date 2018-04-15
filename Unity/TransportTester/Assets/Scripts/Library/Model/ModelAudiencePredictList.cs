using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オーディエンス予想クラスのリストクラス
/// </summary>
[Serializable]
public class ModelAudiencePredictList : IJSONable<ModelAudiencePredictList> {

	/// <summary>
	/// オーディエンス予想リスト
	/// </summary>
	public List<ModelAudiencePredict> audiencePredicts;

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelAudiencePredictList FromJSON(string json) {
		return JsonUtility.FromJson<ModelAudiencePredictList>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
