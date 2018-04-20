using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// チェックボックスでゲームマスターの切り替えを行います。
/// </summary>
public class GMSwitch : MonoBehaviour {

	/// <summary>
	/// 開始時の処理
	/// </summary>
	public void Start() {
		this.IsGMChanged();
	}

	/// <summary>
	/// ゲームマスターのチェックが切り替わった
	/// </summary>
	public void IsGMChanged() {
		bool isChecked = GameObject.Find("IsGM").GetComponent<UnityEngine.UI.Toggle>().isOn;

		if(isChecked == true) {
			GameObject.Find("GMIPAddress").GetComponent<UnityEngine.UI.InputField>().text = "127.0.0.1";
			GameObject.Find("ControllerIPAddress").GetComponent<UnityEngine.UI.InputField>().text = "";
			GameObject.Find("Label_RoleId").GetComponent<UnityEngine.UI.Text>().text = "接続先端末の役割ID";
		} else {
			GameObject.Find("GMIPAddress").GetComponent<UnityEngine.UI.InputField>().text = "";
			GameObject.Find("ControllerIPAddress").GetComponent<UnityEngine.UI.InputField>().text = "127.0.0.1";
			GameObject.Find("Label_RoleId").GetComponent<UnityEngine.UI.Text>().text = "自分の役割ID";
		}
	}

}
