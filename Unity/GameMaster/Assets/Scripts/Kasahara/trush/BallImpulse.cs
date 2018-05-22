using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallImpulse : MonoBehaviour {

	public float thrustX;
	public float thrustZ;
	public float thrustY;
	public Rigidbody rb;

	void Start() 
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate() 
	{
		if (Input.GetKey (KeyCode.LeftShift)) {
			rb.AddForce (thrustX, thrustY, thrustZ, ForceMode.Impulse);
		}
	}
	}
