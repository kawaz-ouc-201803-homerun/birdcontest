using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour {
	// 変数を定義する（データを入れる箱を作る）
	[SerializeField]
	private GameObject PilotCamera;   // インスペクターでパイロット視点カメラを紐づける
	[SerializeField]
	private GameObject thirdPersonCamera;   // インスペクターで第三者視点カメラを紐づける
	[SerializeField]
	private GameObject SideCamera;	//インスペクターで側面視点カメラを紐づける
	[SerializeField]
	private GameObject GroundCamera;	//インスペクターで地面からの視点カメラをひもづえｋ


	//時間
	//public float ;

	void Start () {

			PilotCamera.SetActive(true);
			thirdPersonCamera.SetActive(false);
			SideCamera.SetActive (false);
			GroundCamera.SetActive (false);

	}

	public void ChangeCameraAngle(int cameraID){
		PilotCamera.SetActive (cameraID == 0);
		thirdPersonCamera.SetActive (cameraID == 1);
		SideCamera.SetActive (cameraID == 2);
		GroundCamera.SetActive (cameraID == 3);

	}

	void Update () {
	// ↓現在のactive状態から反転 
	/*PilotCamera.SetActive(!PilotCamera.activeInHierarchy);
	thirdPersonCamera.SetActive(!thirdPersonCamera.activeInHierarchy);*/
		// カメラを切り替える
		/*if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			
			

			PilotCamera.SetActive(true);
			thirdPersonCamera.SetActive(false);
			SideCamera.SetActive (false);
			GroundCamera.SetActive (false);

		}*/
	}
}
