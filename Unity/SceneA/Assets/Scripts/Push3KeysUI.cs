using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Push3KeysUI : MonoBehaviour {
	public Text t;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		t.text =  Push3KeysGame.key[0] + " " + Push3KeysGame.key[1] + " " + Push3KeysGame.key[2];
	}
}