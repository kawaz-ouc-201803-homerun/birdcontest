using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingJudge : MonoBehaviour {

	public UnityEngine.Events.UnityEvent LandingEvent;

	void OnTriggerEnter(Collider other){
		
			Debug.Log("着地");
			if (this.LandingEvent != null) {
				this.LandingEvent.Invoke ();
		
		}
	}

}
