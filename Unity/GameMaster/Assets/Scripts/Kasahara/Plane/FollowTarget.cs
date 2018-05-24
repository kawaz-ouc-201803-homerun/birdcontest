using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 仕込み役：牽引
/// ＊牽引する車にこのスクリプトをアタッチして下さい。
/// </summary>
public class FollowTarget : PlaneBehaviourParent {

	/// <summary>
	/// 飛行機の座標X
	/// </summary>
	float targetX;

	/// <summary>
	/// 飛行機の座標Z
	/// </summary>
	float targetZ;

	/// <summary>
	/// 自車の座標X
	/// </summary>
	float ownX;

	/// <summary>
	/// 自車の座標Z
	/// </summary>
	float ownZ;

	/// <summary>
	/// 初期状態の飛行機と自車の間の距離X
	/// </summary>
	float distanceX;

	/// <summary>
	/// 初期状態の飛行機と自車の間の距離Z
	/// </summary>
	float distanceZ;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		// 飛行機と自車の距離を測って記憶する
		this.ownX = this.transform.position.x;
		this.ownZ = this.transform.position.z;

		this.targetX = this.Plane.transform.position.x;
		this.targetZ = this.Plane.transform.position.z;

		this.distanceX = this.ownX - this.targetX;
		this.distanceZ = this.ownZ - this.targetZ;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		// 初期状態の飛行機と自車の距離を保ちながら移動させる
		this.targetX = this.Plane.transform.position.x;
		this.targetZ = this.Plane.transform.position.z;
		this.transform.position = new Vector3(
			this.targetX + this.distanceX, 
			this.transform.position.y,
			this.targetZ + this.distanceZ
		);
	}

}
