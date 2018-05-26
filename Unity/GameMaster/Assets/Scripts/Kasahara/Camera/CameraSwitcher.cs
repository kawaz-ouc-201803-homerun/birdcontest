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
	public int CurrentCameraId {
		get;
		private set;
	}

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
	/// カメラアングルを切り替えます。
	/// </summary>
	/// <param name="cameraID">カメラ視点ID</param>
	public void ChangeCameraAngle(CameraID cameraID) {
		Debug.Log("カメラ有効: " + cameraID);
		foreach(var camera in this.cameras) {
			camera.gameObject.GetComponent<Camera>().enabled = false;
			camera.gameObject.SetActive(false);
		}
		this.cameras[(int)cameraID].gameObject.SetActive(true);
		this.cameras[(int)cameraID].enabled = true;
		this.CurrentCameraId = (int)cameraID;
	}

	/// <summary>
	/// すべてのカメラアングルを無効化します。
	/// ＊これは一時的に管轄外の別のカメラに切り替えるときに使用して下さい。
	/// </summary>
	public void DisenabledCameraAll() {
		Debug.Log("カメラ無効状態");
		foreach(var camera in this.cameras) {
			camera.gameObject.GetComponent<Camera>().enabled = false;
			camera.gameObject.SetActive(false);
		}
		this.CurrentCameraId = -1;
	}

	/// <summary>
	/// 現在有効なカメラを返します。
	/// </summary>
	/// <returns>現在有効なカメラ。すべて無効になっているときはnull</returns>
	public Camera GetCurrentCamera() {
		if(this.CurrentCameraId < 0 || this.CurrentCameraId <= this.cameras.Length) {
			return null;
		} else {
			return this.cameras[this.CurrentCameraId];
		}
	}

}
