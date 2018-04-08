using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGaugeGame : MonoBehaviour {
	private int power = 0;
	bool chack = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!chack){
			power++;
			if (Input.GetKeyDown(KeyCode.K) && power<=100) {
				chack = true;
				Debug.Log (power);
			}	
		}
	}
}
