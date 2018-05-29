﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三人称俯瞰視点にカメラを切り替えるトリガー
/// </summary>
public class ThirdPersonSwitchTrigger : MonoBehaviour {

	/// <summary>
	/// 操作対象のカメラオブジェクト
	/// </summary>
	[SerializeField]
	private new GameObject camera;

	/// <summary>
	/// 機体がこのオブジェクトに触れたときに発動させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player" && this.camera != null) {
			this.camera.GetComponent<CameraSwitcher>().ChangeCameraAngle(CameraSwitcher.CameraID.ThirdPerson);
		}
	}

}