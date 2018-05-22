using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutIn : MonoBehaviour {

	public Animator cutin;
	public Animation cutinanimation;
	public GameObject image;
	public bool stopflug = false;
	public float time;
	public float sumtime;

	void OnTriggerEnter(Collider other){
		
		if (other.gameObject.tag == "Player") {
			
			stopflug = true;
			image.SetActive (true);
			Time.timeScale = 0;
			cutin.SetTrigger ("Start");

		}
	}

	void Update(){

		if (stopflug == false) {

			return;

		}

		if (stopflug == true) {
			time = Time.unscaledDeltaTime;
			sumtime = sumtime + time;

			if (sumtime >= 3.5f) {
				StopAnimation ();
			}
		}

	}

	void StopAnimation(){

		image.SetActive (false);
		Time.timeScale = 1;

	}
}
