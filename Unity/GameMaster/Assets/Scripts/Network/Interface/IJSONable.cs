using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON形式としてシリアライズ可能なオブジェクトであることを示すインターフェースです。
/// UnityがJSONとしてシリアライズできるのは、プリミティブ型、プリミティブ型の配列、かつ public なメンバー変数のみです。
/// </summary>
public interface IJSONable<T> {

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	string GetJSON();

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	T FromJSON(string json);

}
