using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFlight : MonoBehaviour {

	Rigidbody plane;
	public float power;
	public float rotationX;
	float rotationY;
	float rotationZ;


	public float torque;

	// Use this for initialization
	void Start () {
		plane = GetComponent<Rigidbody> ();
		rotationX = transform.localEulerAngles.x;

		rotationY = transform.localEulerAngles.y;
		rotationZ = transform.localEulerAngles.z;

	}

	// Update is called once per frame
	void Update () {

		rotationX = transform.eulerAngles.x;
		if (power > 0) {

			power = power - Time.deltaTime * 10;

		

			plane.AddForce (transform.forward * power, ForceMode.Acceleration);

			if (rotationX > 360 - 30 || rotationX == 0) {
			
				plane.AddTorque (transform.right * -1, ForceMode.Acceleration);
				rotationX = transform.localEulerAngles.x;

			}

			if (rotationX <= 360 - 30) {
			
				transform.rotation = Quaternion.Euler (360 - 30, rotationY, rotationZ);

			}
		}

		if (power <= 0) {

			Debug.Log ("0だよ");
		}
	}
}
