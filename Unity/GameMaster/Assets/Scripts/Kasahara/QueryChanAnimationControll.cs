using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueryChanAnimationControll : MonoBehaviour {

	public Animator pushplane;		//PushPlaneのアニメータが入っているゲームオブジェクトごとD&D
	public float speed;		//アニメーションのスピード

	//飛行機を押すアニメーションの速度を最初は遅くしておく
	void Start () {
		speed = 0.01f;
		pushplane.speed = speed;
	}
	
	//アニメーションのスピードをだんだん上げていく
	void Update () {

		if (speed <= 8) {
			speed = speed + 2.5f * Time.deltaTime;
			pushplane.speed = speed;

		}
	}
}
