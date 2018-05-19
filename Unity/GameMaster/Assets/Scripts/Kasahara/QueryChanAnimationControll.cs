using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueryChanAnimationControll : MonoBehaviour {

	public Animator pushplane;
	public float speed;

	// Use this for initialization
	void Start () {
		speed = 0.01f;
		pushplane.speed = speed;
	}
	
	// Update is called once per frame
	void Update () {

		if (speed <= 8) {
			speed = speed + 2.5f * Time.deltaTime;
			pushplane.speed = speed;
		}
	}
}
