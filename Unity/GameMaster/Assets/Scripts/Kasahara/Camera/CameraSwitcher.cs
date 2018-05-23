using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ切り替え制御
/// </summary>
public class CameraSwitcher : MonoBehaviour {

	/// <summary>
	/// カメラ視点ID
	/// </summary>
	public enum CameraID {
		Pilot,          // 一人称
		ThirdPerson,    // 三人称俯瞰
		Side,           // 横視点
		Ground,         // 地上視点
	}

	/// <summary>
	/// インスペクターでパイロット視点カメラを紐づける
	/// </summary>
	[SerializeField]
	private GameObject pilotCamera;

	/// <summary>
	/// インスペクターで第三者視点カメラを紐づける
	/// </summary>
	[SerializeField]
	private GameObject thirdPersonCamera;

	/// <summary>
	/// インスペクターで側面視点カメラを紐づける
	/// </summary>
	[SerializeField]
	private GameObject sideCamera;

	/// <summary>
	/// インスペクターで地面からの視点カメラを紐づける
	/// </summary>
	[SerializeField]
	private GameObject groundCamera;

	/// <summary>
	/// 初期化処理
	/// </summary>
	void Start() {
		if(DataContainer.OptionA == (int)PhaseControllers.OptionA.Human) {
			// 人が飛行機を押すときだけ、後方俯瞰にする
			this.ChangeCameraAngle(CameraID.ThirdPerson);
		} else {
			// カメラ視点の初期状態：一人称
			this.ChangeCameraAngle(CameraID.Pilot);
		}
	}

	/// <summary>
	/// カメラアングルを切り替えます。
	/// </summary>
	/// <param name="cameraID">カメラ視点ID</param>
	public void ChangeCameraAngle(CameraID cameraID) {
		this.pilotCamera.SetActive(cameraID == CameraID.Pilot);
		this.thirdPersonCamera.SetActive(cameraID == CameraID.ThirdPerson);
		this.sideCamera.SetActive(cameraID == CameraID.Side);
		this.groundCamera.SetActive(cameraID == CameraID.Ground);
	}

}
