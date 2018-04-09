using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操作端末Ｂの進捗報告クラス
/// </summary>
[Serializable]
public class ModelControllerB : IJSONable<ModelControllerB> {

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelControllerB FromJSON(string json) {
		return JsonUtility.FromJson<ModelControllerB>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
