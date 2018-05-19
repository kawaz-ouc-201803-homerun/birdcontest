using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour {
	
	public Rigidbody plane;
	public ForceMode powerforce;
	public ForceMode liftforce;

	public float power;
	public float lift;

	// Use this for initialization
	void Start () {
		plane = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftShift)) {
			plane.AddForce (transform.forward * power, powerforce);
		}
		if (Input.GetKey (KeyCode.Space)) {
			plane.AddForce (transform.up * lift, liftforce);
		}
	}
}
