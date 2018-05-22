using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideFixed : MonoBehaviour {



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter (Collider other) {
		
		if (other.gameObject.tag == "Playre") {
			Vector3 sideposition = transform.position;
			transform.position = new Vector3 (sideposition.x, sideposition.y, sideposition.z);
		}

	}
}
