using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FollowTarget : MonoBehaviour {
	//牽引する車にこのスクリプトをアタッチ

	//牽引される飛行機をtargetに入れる
	public GameObject target;

	//飛行機の座標とZ座標
	float targetX;
	float targetZ;

	//車のX座標とZ座標
	float ownX;
	float ownZ;

	//飛行機と自分の間の距離
	float distanceX;
	float distanceZ;

	//飛行機と自分の間の距離を測る
	void Start () {
		
		ownX = this.transform.position.x;
		ownZ = this.transform.position.z;

		targetX = target.transform.position.x;
		targetZ = target.transform.position.z;

		distanceX = ownX - targetX;
		distanceZ = ownZ - targetZ;

	}
	
	//Start関数で求めた距離を保って飛行機についていく
	void Update () {

		targetX = target.transform.position.x;
		targetZ = target.transform.position.z;

		this.transform.position = new Vector3 (targetX+distanceX, this.transform.position.y, targetZ+distanceZ);

	}
}
