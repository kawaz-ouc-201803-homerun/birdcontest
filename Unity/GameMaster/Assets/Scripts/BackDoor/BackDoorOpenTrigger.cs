using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バックドアを開くためのトリガー
/// </summary>
public class BackDoorOpenTrigger : MonoBehaviour {

	/// <summary>
	/// バックドアのUI親オブジェクトのインデックス
	/// </summary>
	public enum BackDoorUIIndex {
		IPAddress,
		Result,
	}
	
	/// <summary>
	/// バックドアのUI親オブジェクトのインデックスとキーコードを紐づける定義
	/// </summary>
	public static readonly Dictionary<KeyCode, int> UIIndexMap = new Dictionary<KeyCode, int>() {
		{ KeyCode.F10, (int)BackDoorUIIndex.IPAddress },
		{ KeyCode.F11, (int)BackDoorUIIndex.Result },
	};

	/// <summary>
	/// バックドアのUI親オブジェクト
	/// </summary>
	public GameObject[] BackDoorUIs;

	/// <summary>
	/// 現在開いているバックドアのインデックス
	/// </summary>
	private static int currentBackDoorIndex;

	/// <summary>
	/// 現在バックドアが開かれているかどうか
	/// </summary>
	/// <returns></returns>
	public static bool IsBackDoorOpened {
		get {
			return BackDoorOpenTrigger.currentBackDoorIndex != -1;
		}
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public void Start() {
		BackDoorOpenTrigger.currentBackDoorIndex = -1;
	}

	/// <summary>
	/// 毎フレーム処理
	/// </summary>
	public void Update() {
		foreach(var map in BackDoorOpenTrigger.UIIndexMap) {
			if(Input.GetKeyDown(map.Key) == true && BackDoorOpenTrigger.currentBackDoorIndex != map.Value) {
				this.ChangeBackDoor(map.Value);
			}
		}
	}

	/// <summary>
	/// バックドアの表示切替
	/// </summary>
	/// <param name="index">表示するバックドアのインデックス。すべて無効にするときは -1 を指定する</param>
	public void ChangeBackDoor(int index) {
		BackDoorOpenTrigger.currentBackDoorIndex = index;

		// すべて無効化
		foreach(var obj in this.BackDoorUIs) {
			obj.SetActive(false);
		}

		// 指定したバックドアを有効にする
		if(0 <= index && index < this.BackDoorUIs.Length) {
			this.BackDoorUIs[index].SetActive(true);
			this.BackDoorUIs[index].GetComponentInChildren<BackDoorBase>().Start();
		}
	}

}
