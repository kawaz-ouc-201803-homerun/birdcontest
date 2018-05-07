using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 担当する役割を決定させます。
/// </summary>
public class ControllerSelector : MonoBehaviour {

	/// <summary>
	/// 役割ID
	/// </summary>
	public enum RoleIds {
		A_Prepare,
		B_Flight,
		C_Assist,
	}

	/// <summary>
	/// 選択した役割ID
	/// </summary>
	public static int SelectedRoleId;

	/// <summary>
	/// ゲームマスターのIPアドレス
	/// </summary>
	public static string GameMasterIPAddress;

	/// <summary>
	/// IPアドレス入力欄のオブジェクト
	/// </summary>
	public InputField IPAddressInputField;

	/// <summary>
	/// 端末A を選択
	/// </summary>
	public void OnSelectedRoleA() {
		this.selectRole(RoleIds.A_Prepare);
	}

	/// <summary>
	/// 端末B を選択
	/// </summary>
	public void OnSelectedRoleB() {
		this.selectRole(RoleIds.B_Flight);
	}

	/// <summary>
	/// 端末C を選択
	/// </summary>
	public void OnSelectedRoleC() {
		this.selectRole(RoleIds.C_Assist);
	}

	/// <summary>
	/// 役割を選択してシーン遷移します。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void selectRole(RoleIds roleId) {
		Debug.Log("GMIPアドレス: " + this.IPAddressInputField.text);
		ControllerSelector.GameMasterIPAddress = this.IPAddressInputField.text;

		Debug.Log("シーン開始準備: " + (int)roleId);
		ControllerSelector.SelectedRoleId = (int)roleId;
		SceneManager.LoadScene("Controllers");
	}

}
