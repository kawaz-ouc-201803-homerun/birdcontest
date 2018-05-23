using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControl : MonoBehaviour {

	public GameObject player;
	float rotationX;
	float rotationY;
	float rotationZ;

	// Use this for initialization
	void Start () {

		rotationX = transform.localEulerAngles.x;
		rotationY = transform.localEulerAngles.y;
		rotationZ = transform.localEulerAngles.z;
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.rotation = Quaternion.Euler (rotationX, rotationY, rotationZ);
	}
}
