using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 援護役：爆弾
/// 仕込み役と援護役が両方とも爆弾だったときは墜落します。
/// </summary>
public class SecondExplosion : PlaneBehaviourParent {

	/// <summary>
	/// 爆弾オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject SecondBomb;

	/// <summary>
	/// 黒煙オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject Smoke;

	/// <summary>
	/// 墜落時に差し替えるダミーの飛行機オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject DummyPlane;

	//二連続爆発だった場合の処理に使う変数
	public GameObject counter;      //DistanceCounterがアタッチされているObjectを代入
	float breakflugA;       //optionAの値を代入
	float breakflugC;       //optionCの値を代入

	public static bool enabledOnTrigegrEnter = false;       //OnTriggerEnterの実行のフラグをデフォルトではオフに（フラグの切り替えはDataContainerTestから）

	void OnTriggerEnter(Collider other) {
		//フラグがオフなら何もしない
		if(enabledOnTrigegrEnter == false) {
			return;
		}

		breakflugA = Plane.GetComponent<DataContainerTest>().optionA;
		breakflugC = Plane.GetComponent<DataContainerTest>().optionC;

		if(other.gameObject.tag == "Trigger") {
			//AとCどちらも爆弾だった場合、DummyPlaneを出現させ、煙を出し、上方向に飛ばす
			if(breakflugA == 0 && breakflugC == 0) {

				DummyPlane.transform.position = Plane.transform.position;       //DummyPlaneの位置をPlaneの接地点に合わせる

				counter.GetComponent<DistanceCounter>().enabled = false;        //元の飛行機を消すとエラー吐きまくるのであらかじめ機能を停止させておく
				Destroy(Plane);     //元の飛行機を消す

				Smoke.SetActive(true);      //煙を出す

				DummyPlane.SetActive(true);     //DummyPlaneを出現させる
				DummyPlane.GetComponent<Rigidbody>().AddForce(transform.up * 20, ForceMode.VelocityChange);     //DummyPlaneを上方向に飛ばす

			}

			SecondBomb.transform.position = Plane.transform.position;       //SecoundBombの位置をPlaneに合わせる
			SecondBomb.SetActive(true);     //SecondBombの爆破
			enabledOnTrigegrEnter = false;      //処理が繰り返されないためにフラグをfalseに切り替え
		}
	}

}