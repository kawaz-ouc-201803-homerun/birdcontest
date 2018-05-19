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
	private GameObject GroundCamera;	//インスペクターで地面からの視点カメラを紐づける

	[SerializeField]
	private DataContainerTest data;		//DataContainerがアタッチされているゲームオブジェクトを代入

	void Start () {
		//DataContainerから変数optionAを取得
			PilotCamera.SetActive(true);
			thirdPersonCamera.SetActive(false);
			SideCamera.SetActive (false);
			GroundCamera.SetActive (false);

		//人が飛行機を押すときだけ最初のカメラをthirdperonにする
		if (data.optionA == 2) {

			PilotCamera.SetActive(false);
			thirdPersonCamera.SetActive(true);
			SideCamera.SetActive (false);
			GroundCamera.SetActive (false);


		}

	}

	public void ChangeCameraAngle(int cameraID){
		PilotCamera.SetActive (cameraID == 0);
		thirdPersonCamera.SetActive (cameraID == 1);
		SideCamera.SetActive (cameraID == 2);
		GroundCamera.SetActive (cameraID == 3);

	}

}
