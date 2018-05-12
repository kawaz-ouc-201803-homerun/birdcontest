using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Push3KeyPadGameUI : MonoBehaviour {
	public Text t;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		t.text =  ((int)Push3KeyPadGame.key [0] - 350 + 1) + 
			" " + ((int)Push3KeyPadGame.key [1] - 350 + 1) + 
			" " + Push3KeyPadGame.axisname;
	}
}