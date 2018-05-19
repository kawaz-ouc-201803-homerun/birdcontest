using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopFollow : MonoBehaviour {
	//板につける
	//牽引する車に"Car"というタグをつける

	public GameObject pullcar;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Car") {
			pullcar.GetComponent<FollowTarget> ().enabled = false;
			Debug.Log("ストップ");
		}
	}
}
