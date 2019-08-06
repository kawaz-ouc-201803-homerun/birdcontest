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

		// チェックボックスの状態を復元
		GameObject.Find("Settings_DisableNearpinConditions").GetComponent<Toggle>().isOn = PhaseResult.IsNearpinConditionDisabled;
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

	/// <summary>
	/// オンライン用のテンプレートを適用します。
	/// </summary>
	public void OnLoadOnlineTemplate() {
		this.IPAddresses[(int)NetworkConnector.RoleIds.A_Prepare].text = "192.168.11.10";
		this.IPAddresses[(int)NetworkConnector.RoleIds.B_Flight].text = "192.168.11.11";
		this.IPAddresses[(int)NetworkConnector.RoleIds.C_Assist].text = "192.168.11.12";
	}

	/// <summary>
	/// オンライン用のテンプレートを適用します。
	/// </summary>
	public void OnLoadOfflineTemplate() {
		this.IPAddresses[(int)NetworkConnector.RoleIds.A_Prepare].text = "127.0.0.1";
		this.IPAddresses[(int)NetworkConnector.RoleIds.B_Flight].text = "127.0.0.1";
		this.IPAddresses[(int)NetworkConnector.RoleIds.C_Assist].text = "127.0.0.1";
	}

	/// <summary>
	/// ニアピン賞の条件を取っ払うかどうかのチェック切り替え
	/// </summary>
	public void OnNearpinConditionChecked() {
		PhaseResult.IsNearpinConditionDisabled = GameObject.Find("Settings_DisableNearpinConditions").GetComponent<Toggle>().isOn;
		Debug.Log("ニアピン賞の条件: " + PhaseResult.IsNearpinConditionDisabled);
	}

}
