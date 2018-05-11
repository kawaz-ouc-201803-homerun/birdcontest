using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGuide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ButtonClick() {
		//https://qiita.com/7of9/items/3b5f6dd0af4f8af353c5
		switch (transform.name) {
		case "Button1":
			Debug.Log ("1");
			break;
		case "Button2":
			Debug.Log("2");
			break;
		case "Button3":
			Debug.Log("3");
			break;
		default:
			break;
		}
	}
}
