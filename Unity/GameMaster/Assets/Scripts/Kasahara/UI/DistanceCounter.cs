using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//ここ注意
using System;

public class DistanceCounter : MonoBehaviour {
	//変数設定
	//スタート時の座標（DataContainerTestから代入）
	public float startpositionX;
	public float startpositionZ;
	//飛行中の座標
	float currentpositionX;
	float currentpositionZ;
	//飛距離
	float distance;

	//知りたい座標のGaeObjectの設定
	public GameObject target;

	// Update is called once per frame
	void Update () {

		//それぞれに座標を挿入
		currentpositionX = target.transform.position.x;
		currentpositionZ = target.transform.position.z;

		//飛距離の計算　（√（二乗＋二乗））
		distance = Mathf.Sqrt ((currentpositionX - startpositionX) * (currentpositionX - startpositionX) + (currentpositionZ - startpositionZ) * (currentpositionZ - startpositionZ));

		//テキストに表示
		this.GetComponent<Text> ().text = distance.ToString("0.00") + "m";

	}
}