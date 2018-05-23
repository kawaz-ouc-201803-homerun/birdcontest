using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopFollow : MonoBehaviour {

	//車とクエリちゃんを飛行機から切り離すスクリプト

	//板につける
	//牽引する車に"Car"というタグをつける

	public GameObject pullcar;		//牽引する車を代入
	public GameObject pushhuman;		//クエリちゃんを代入

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Car") {
			pullcar.GetComponent<FollowTarget> ().enabled = false;
			pushhuman.GetComponent<FollowTarget> ().enabled = false;
		}
	}
}
