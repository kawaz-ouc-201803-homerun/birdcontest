using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGaugeGame : MonoBehaviour {
	private bool chack = false;
	private int dpower=1;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(!chack){
			PowerUI.power += dpower;
			if (PowerUI.power <= 0 || 100 <= PowerUI.power)
				dpower *= -1;
			if (Input.GetKeyDown(KeyCode.K)) {
				chack = true;
				Debug.Log (PowerUI.power);
			}	
		}
	}
}
