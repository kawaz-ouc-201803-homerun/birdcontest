using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown)
			Debug.Log ("Any");
		//Unity->ProjectSetting->InputManagerからFire0~Fire8を設定する必要がある。
		if (Input.GetAxis("Horizontal") != 0f)
			Debug.Log ("Hol");
		if (Input.GetAxis("Vertical") != 0f)
			Debug.Log ("Ver");
		if (Input.GetButtonDown ("Fire1"))
			Debug.Log ("1");
		if (Input.GetButtonDown ("Fire2"))
			Debug.Log ("2");
		if (Input.GetButtonDown ("Fire3"))
			Debug.Log ("3");
		if (Input.GetButtonDown ("Fire4"))
			Debug.Log ("4");
		if (Input.GetButtonDown ("Fire5"))
			Debug.Log ("5");
		if (Input.GetButtonDown ("Fire6"))
			Debug.Log ("6");
		if (Input.GetButtonDown ("Fire7"))
			Debug.Log ("7");
		if (Input.GetButtonDown ("Fire8"))
			Debug.Log ("8");
		if(Input.GetKeyDown (KeyCode.UpArrow))
			Debug.Log ("UpArrow");
		if(Input.GetKeyDown (KeyCode.DownArrow))
			Debug.Log ("DownArrow");
		if(Input.GetKeyDown (KeyCode.LeftArrow))
			Debug.Log ("LeftArrow");
		if(Input.GetKeyDown (KeyCode.RightArrow))
			Debug.Log ("RightArrow");
	}
}
