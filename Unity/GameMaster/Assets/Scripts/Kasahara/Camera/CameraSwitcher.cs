using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ切り替え制御
/// </summary>
public class CameraSwitcher : MonoBehaviour {

	/// <summary>
	/// 端末操作結果データ
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// 切り替え対象のカメラ
	/// ＊インスペクターでそれぞれの視点カメラを紐づけて下さい。
	/// </summary>
	[SerializeField]
	private Camera[] cameras;

	/// <summary>
	/// 現在有効なカメラ視点のID
	/// </summary>
	private CameraID currentCameraId;

	/// <summary>
	/// カメラ視点ID
	/// </summary>
	public enum CameraID {
		Pilot,          // 一人称視点
		ThirdPerson,    // 後方俯瞰視点
		Side,           // 横視点
		Ground,         // 地上視点
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		if(this.DataContainer.OptionA == (int)PhaseControllers.OptionA.Human) {
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
		foreach(var camera in this.cameras) {
			camera.gameObject.SetActive(false);
		}
		this.cameras[(int)cameraID].gameObject.SetActive(true);
		this.currentCameraId = cameraID;
	}

	/// <summary>
	/// 現在有効なカメラを返します。
	/// </summary>
	/// <returns>現在有効なカメラ</returns>
	public Camera GetCurrentCamera() {
		return this.cameras[(int)this.currentCameraId];
	}

}
