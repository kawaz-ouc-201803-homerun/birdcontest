﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 飛距離計算を行います。
/// </summary>
public class DistanceCalculator : MonoBehaviour {

	/// <summary>
	/// 飛距離
	/// </summary>
	public float Distance;

	/// <summary>
	/// 対象オブジェクト
	/// </summary>
	public GameObject Target;

	/// <summary>
	/// 対象オブジェクトの初期位置X
	/// </summary>
	private float startPositionX;

	/// <summary>
	/// 対象オブジェクトの初期位置Z
	/// </summary>
	private float startPositionZ;

	/// <summary>
	/// 対象オブジェクトの現在位置X
	/// </summary>
	private float currentPositionX;

	/// <summary>
	/// 対象オブジェクトの現在位置Z
	/// </summary>
	private float currentPositionZ;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		// 対象オブジェクトの初期座標を保管
		this.startPositionX = this.Target.transform.position.x;
		this.startPositionZ = this.Target.transform.position.z;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		// 最新の座標を取得
		this.currentPositionX = this.Target.transform.position.x;
		this.currentPositionZ = this.Target.transform.position.z;

		// 飛距離の計算　（√（二乗＋二乗））
		// NOTE: マップが拡大されているため、距離の縮尺を適用する
		this.Distance = Mathf.Sqrt(
			(this.currentPositionX - this.startPositionX)
			* (this.currentPositionX - this.startPositionX)
			+ (this.currentPositionZ - this.startPositionZ)
			* (this.currentPositionZ - this.startPositionZ)
		) * 0.5f;
	}

}