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

			if (rotationX > 360 - 45 || rotationX == 0) {

				plane.AddTorque (transform.right * -1, ForceMode.Acceleration);
				rotationX = transform.localEulerAngles.x;

			}

			if (rotationX <= 360 - 45) {

				transform.rotation = Quaternion.Euler (360 - 45, rotationY, rotationZ);

			}
		}

		if (power <= 0) {

			if (rotationX <= 360 || rotationX <= 45) {

				rotationX = rotationX + 10 * Time.deltaTime;

				plane.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

			}

			if (rotationX >= 45 && rotationX < 360 - 45) {
				plane.transform.rotation = Quaternion.Euler (45, rotationY, rotationZ);
			}

			if (rotationX >= 0 && rotationX <= 1) {
				plane.transform.rotation = Quaternion.Euler (0, rotationY, rotationZ);
			}

			Debug.Log ("0だよ");
		}
	}
}
