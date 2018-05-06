using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作端末の結果を書き換えるバックドア
/// </summary>
public class BackDoorResult : MonoBehaviour {

	/// <summary>
	/// 入力された結果データ
	/// </summary>
	public InputField[] Results;
	
	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public void Start() {
		// 現在の結果データを所定の形式に変換してUIに格納する
		Debug.Log(PhaseControllers.ControllerProgresses.Length);
		for(int i = 0; i < this.Results.Length; i++) {
			this.Results[i].text = this.convertDictionaryToString(PhaseControllers.ControllerProgresses[i]);
		}
	}

	/// <summary>
	/// 毎フレーム処理
	/// </summary>
	public void Update() {
	}

	/// <summary>
	/// 変更内容を適用します。
	/// </summary>
	public void OnApply() {
		// 現在の入力データを辞書型配列に変換して結果データとして格納する
		for(int i = 0; i < this.Results.Length; i++) {
			PhaseControllers.ControllerProgresses[i] = this.convertStringToDictionary(this.Results[i].text);
		}

		PhaseControllers.BackDoorOperated = true;

		// バックドアを閉じる
		GameObject.Find("BackDoors").GetComponent<BackDoorOpenTrigger>().ChangeBackDoor(-1);
	}

	/// <summary>
	/// 辞書型配列をUI用の文字列に変換します。
	/// </summary>
	/// <returns>UI用の文字列</returns>
	private string convertDictionaryToString(Dictionary<string, string> dictionary) {
		bool wrote = false;

		using(var buf = new StringWriter()) {
			foreach(var key in dictionary.Keys) {
				if(wrote == true) {
					// ２個目以降には ; を付けて区切る
					buf.Write(";");
				}

				// データを文字列に形式変換
				buf.Write(key);
				buf.Write("=");
				buf.Write(dictionary[key]);

				wrote = true;
			}
			return buf.ToString();
		}
	}

	/// <summary>
	/// UI用の文字列を辞書型配列に変換します。
	/// </summary>
	/// <param name="data">UI用の文字列</param>
	/// <returns>辞書型配列</returns>
	private Dictionary<string, string> convertStringToDictionary(string data) {
		// 一つのデータごとに分離
		var split = data.Split(';');
		var dictionary = new Dictionary<string, string>();

		foreach(var oneData in split) {
			var keyValuePair = oneData.Split('=');
			dictionary[keyValuePair[0]] = keyValuePair[1];
		}

		return dictionary;
	}

}
