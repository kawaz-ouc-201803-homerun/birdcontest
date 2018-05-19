using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSwitch : MonoBehaviour {

	[SerializeField]
	public GameObject camera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player")
		{
			camera.GetComponent<CameraSwitching> ().ChangeCameraAngle(3);
		}
	}
}
