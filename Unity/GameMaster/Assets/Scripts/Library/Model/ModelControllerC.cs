using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 操作端末Ｃの進捗報告クラス
/// </summary>
[Serializable]
public class ModelControllerC : IJSONable<ModelControllerC> {

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelControllerC FromJSON(string json) {
		return JsonUtility.FromJson<ModelControllerC>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
