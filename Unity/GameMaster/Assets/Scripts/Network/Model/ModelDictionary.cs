using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// UnityのJsonUtilityでシリアライズできない辞書型をJSONで読み書きできるようにします。
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
/// <typeparam name="TValue">値の型</typeparam>
[Serializable]
public class ModelDictionary<TKey, TValue> : ISerializationCallbackReceiver, IJSONable<ModelDictionary<TKey, TValue>> {

	/// <summary>
	/// キーのリスト：値と同順
	/// </summary>
	[SerializeField]
	private List<TKey> keys;

	/// <summary>
	/// 値のリスト：キーと同順
	/// </summary>
	[SerializeField]
	private List<TValue> values;

	/// <summary>
	/// 元の辞書型：直接シリアライズできない
	/// </summary>
	private Dictionary<TKey, TValue> target;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="target">元の辞書型オブジェクト</param>
	public ModelDictionary(Dictionary<TKey, TValue> target) {
		this.target = target;
	}

	/// <summary>
	/// 本物の辞書型としてデータを返します。
	/// </summary>
	/// <returns></returns>
	public Dictionary<TKey, TValue> GetDictionary() {
		return this.target;
	}

	/// <summary>
	/// シリアライズ前に行う処理です。
	/// 辞書をキーと値のリストに分解します。
	/// </summary>
	public void OnBeforeSerialize() {
		this.keys = new List<TKey>(this.target.Keys);
		this.values = new List<TValue>(this.target.Values);
	}

	/// <summary>
	/// デシリアライズした後に行う処理です。
	/// キーと値のリストを統合して本物の辞書型を作成します。
	/// </summary>
	public void OnAfterDeserialize() {
		int count = Math.Min(this.keys.Count, this.values.Count);
		this.target = new Dictionary<TKey, TValue>(count);
		for(var i = 0; i < count; i++) {
			this.target.Add(this.keys[i], this.values[i]);
		}
	}

	/// <summary>
	/// JSON文字列からこのインターフェースを実装するオブジェクトにデシリアライズします。
	/// </summary>
	/// <param name="json">シリアライズされたJSON文字列</param>
	/// <returns>復元されたオブジェクト</returns>
	public ModelDictionary<TKey, TValue> FromJSON(string json) {
		return JsonUtility.FromJson<ModelDictionary<TKey, TValue>>(json);
	}

	/// <summary>
	/// このインターフェースを実装するオブジェクトをJSON形式でシリアライズします。
	/// </summary>
	/// <returns>JSONとしてシリアライズされた文字列</returns>
	public string GetJSON() {
		return JsonUtility.ToJson(this);
	}

}
