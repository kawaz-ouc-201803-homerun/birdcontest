using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PompPedal : ParentClass {

	public float pedalpower;		//DataContainerから代入されるのでこちらでの変更は不要
	public GameObject starter;

	//PedalタグがついているPlaneに触れたらオンに
	void OnTriggerEnter(Collider other){
	
		if (other.gameObject.tag == "Pedal") {
			this.enabled = true;
		}
	
	}



	void FixedUpdate () {
		if (pedalpower > 0) {
			
			pedalpower = pedalpower - Time.deltaTime * 10;
			planerigidbody.AddForce (transform.forward * pedalpower, ForceMode.Force);
			planerigidbody.AddForce (transform.up * pedalpower, ForceMode.Force);

		}
	}
}