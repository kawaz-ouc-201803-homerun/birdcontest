using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModelControllerProgress = ModelDictionary<string, string>;

/// <summary>
/// テストの管理・制御を行います。
/// </summary>
public class TesterManager : MonoBehaviour {

	/// <summary>
	/// 自分がGM？
	/// </summary>
	public bool IsGM;

	/// <summary>
	/// GM/IPアドレス
	/// </summary>
	public string GMIPAddress;

	/// <summary>
	/// 操作端末/IPアドレス
	/// </summary>
	public string ControllerIPAddress;

	/// <summary>
	/// 役割ID
	/// </summary>
	public int RoleId;

	/// <summary>
	/// インスタンス：GM
	/// </summary>
	public GameObject ObjectTesterGM;

	/// <summary>
	/// インスタンス：操作端末
	/// </summary>
	public GameObject ObjectTesterController;

	/// <summary>
	/// 開始時の処理
	/// </summary>
	public void Start() {
	}

	/// <summary>
	/// テスト実行ボタン押下時
	/// </summary>
	public void onStart() {
		this.IsGM = GameObject.Find("IsGM").GetComponent<Toggle>().isOn;
		this.GMIPAddress = GameObject.Find("GMIPAddress").GetComponent<InputField>().text;
		this.ControllerIPAddress = GameObject.Find("ControllerIPAddress").GetComponent<InputField>().text;
		this.RoleId = GameObject.Find("RoleId").GetComponent<Dropdown>().value;

		// バリデーション
		string errorMessage = "";
		if(string.IsNullOrEmpty(this.GMIPAddress) == true) {
			errorMessage += "GMのIPアドレスを入力して下さい。\n";
		}
		if(string.IsNullOrEmpty(this.ControllerIPAddress) == true) {
			errorMessage += "操作端末のIPアドレスを入力して下さい。\n";
		}
		if(string.IsNullOrEmpty(errorMessage) == false) {
			Logger.LogProcess(errorMessage);
			return;
		}

		// テスト開始
		GameObject.Find("DoTest").GetComponent<Button>().interactable = false;
		Logger.LogProcess("==============================");
		Logger.LogProcess(
			"テストを開始します...IsGM=" + this.IsGM + ", GMIP=" + this.GMIPAddress + ", CtrlIP=" + this.ControllerIPAddress + ", RoleID=" + this.RoleId
		);

		TesterBase tester;
		if(this.IsGM == true) {
			this.ObjectTesterGM.SetActive(true);
			tester = this.ObjectTesterGM.GetComponent<TesterGM>();
		} else {
			this.ObjectTesterController.SetActive(true);
			tester = this.ObjectTesterController.GetComponent<TesterController>();
		}
		tester.DoTest(new Dictionary<string, object>() {
			{"GMIP", this.GMIPAddress },
			{"CtrlIP", this.ControllerIPAddress },
			{"RoleID", this.RoleId },
		});
	}

}
