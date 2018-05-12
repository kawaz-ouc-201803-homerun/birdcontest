using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushKeyArrayUI : MonoBehaviour {
	public Text t;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < PushKeyArrayGame.length; i++) {
			t.text = PushKeyArrayGame.keys [i] + " ";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (true) {
			t.text = "";
			for (int i = PushKeyArrayGame.key_num; i < PushKeyArrayGame.length; i++) {
				t.text += PushKeyArrayGame.keys [i] + " ";
			}
		}
	}
}
