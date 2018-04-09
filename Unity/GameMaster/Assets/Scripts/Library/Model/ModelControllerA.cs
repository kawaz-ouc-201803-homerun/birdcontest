using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操作端末Ａの進捗報告クラス
/// </summary>
[Serializable]
public class ModelControllerA : IJSONable<ModelControllerA> {

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelControllerA FromJSON(string json) {
		return JsonUtility.FromJson<ModelControllerA>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
