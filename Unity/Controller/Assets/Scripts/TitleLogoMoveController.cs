using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミニゲームの妨げになる場合にタイトルロゴを別の場所に退避させるためのコントローラー
/// </summary>
public class TitleLogoMoveController : MonoBehaviour {

	/// <summary>
	/// 初回処理
	/// </summary>
	public void Start() {
		switch(ControllerSelector.SelectedRoleId) {
			case (int)ControllerSelector.RoleIds.A_Prepare:
			case (int)ControllerSelector.RoleIds.C_Assist:
				GameObject.Find("TitleLogo").transform.localPosition = new Vector3(433, 300, 0);
				break;
		}
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
	}

}
