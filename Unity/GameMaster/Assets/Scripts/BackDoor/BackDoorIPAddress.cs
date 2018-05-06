using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作端末のIPアドレス定義を書き換えるバックドア
/// </summary>
public class BackDoorIPAddress : BackDoorBase {

	/// <summary>
	/// 入力されたIPアドレス
	/// </summary>
	public InputField[] IPAddresses;

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public override void Start() {
		// 現在設定されているIPアドレスで復元
		for(int i = 0; i < this.IPAddresses.Length; i++) {
			this.IPAddresses[i].text = PhaseControllers.ControllerIPAddresses[i];
		}
	}

	/// <summary>
	/// 毎フレーム処理
	/// </summary>
	public override void Update() {
	}

	/// <summary>
	/// 変更内容を適用します。
	/// </summary>
	public void OnApply() {
		// 現在入力されているIPアドレスに設定変更
		PhaseControllers.ControllerIPAddresses = new string[this.IPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			var ipAddress = this.IPAddresses[i].text;
			Debug.Log("IPアドレス変更: 端末ID=" + i + ", IPアドレス=" + ipAddress);
			PhaseControllers.ControllerIPAddresses[i] = ipAddress;
		}

		// バックドアを閉じる
		GameObject.Find("BackDoors").GetComponent<BackDoorOpenTrigger>().ChangeBackDoor(-1);
	}

}
