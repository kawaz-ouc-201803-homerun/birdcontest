using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheerup : MonoBehaviour {

	public Rigidbody plane;

	public float upper;		//paramCから代入

	public static bool cheerupOnSwitch = false;		//OnTriggerEnterの実行フラグをデフォルトでオフに


	//TriggerタグがついているPlaneに触ったらこのスクリプトをオンに
	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Trigger") {
			this.enabled = true;
		}
	}
		
	void FixedUpdate () {

		if (upper > 0) {
			upper = upper - 1f * Time.deltaTime;		//飛ぶ力を1秒ずつ減衰
			plane.AddForce (transform.up * upper, ForceMode.VelocityChange);
		}
	}
}
