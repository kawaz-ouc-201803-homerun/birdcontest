using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : ParentClass {

	//飛行機に助走をつけるスクリプト

	//飛行機にアタッチ

	[HideInInspector]
	public float movepower;	//前に進む力

	public ForceMode powerforce;

	void FixedUpdate () {

		movepower = movepower + 10 * Time.deltaTime;	//推進力を毎秒10加える

		planerigidbody.AddForce (transform.forward * movepower, powerforce);		//飛行機にmovepowerを加える

		}

	//StopperタグがついているPlaneに触った時に離陸
	public void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Stopper"){
			
			planerigidbody.AddForce (transform.up * 500, ForceMode.Impulse);

			enabled = false;

	}
}
}
