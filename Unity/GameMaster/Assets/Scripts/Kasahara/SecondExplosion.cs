using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondExplosion : ParentClass {

	//端末Cの選択によって出現する爆弾の処理を行うスクリプト

	public GameObject secondbomb;		//インスペクター上でSecondBombを代入
	public GameObject smoke;		//インスペクター上でSmokeを代入
	public GameObject dummyplane;		//インスペクター上で二連続爆発時に出現させる飛行機を代入

	//二連続爆発だった場合の処理に使う変数
	public GameObject counter;		//DistanceCounterがアタッチされているObjectを代入
	float breakflugA;		//optionAの値を代入
	float breakflugC;		//optionCの値を代入

	public static bool enabledOnTrigegrEnter = false;		//OnTriggerEnterの実行のフラグをデフォルトではオフに（フラグの切り替えはDataContainerTestから）

	void Start(){

	}

	void OnTriggerEnter (Collider other) {

		//フラグがオフなら何もしない
		if (enabledOnTrigegrEnter == false) {
			return;
		}

		breakflugA = plane.GetComponent<DataContainerTest> ().optionA;
		breakflugC = plane.GetComponent<DataContainerTest> ().optionC;

		if (other.gameObject.tag == "Trigger") {

			//AとCどちらも爆弾だった場合、DummyPlaneを出現させ、煙を出し、上方向に飛ばす
			if (breakflugA == 0 && breakflugC == 0) {
				
				dummyplane.transform.position = plane.transform.position;		//DummyPlaneの位置をPlaneの接地点に合わせる

				counter.GetComponent<DistanceCounter> ().enabled = false;		//元の飛行機を消すとエラー吐きまくるのであらかじめ機能を停止させておく
				Destroy (plane);		//元の飛行機を消す

				smoke.SetActive (true);		//煙を出す

				dummyplane.SetActive (true);		//DummyPlaneを出現させる
				dummyplane.GetComponent<Rigidbody>().AddForce (transform.up * 20,ForceMode.VelocityChange);		//DummyPlaneを上方向に飛ばす

			}

			secondbomb.transform.position = plane.transform.position;		//SecoundBombの位置をPlaneに合わせる

			secondbomb.SetActive (true);		//SecondBombの爆破

			enabledOnTrigegrEnter = false;		//処理が繰り返されないためにフラグをfalseに切り替え
		}
	}
}