using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainB : MonoBehaviour {
	
	public bool flag;

	public int count;

	// Use this for initialization
	void Start () {
		Debug.Log ("MainB");	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			this.flag = true;
		}
		if (Input.GetMouseButtonUp (0) && this.flag==true) {
			this.count++;
			this.flag = false;
			GameObject.Find ("Slider").GetComponent<UnityEngine.UI.Slider> ().value = this.count;

		}
	}
}
