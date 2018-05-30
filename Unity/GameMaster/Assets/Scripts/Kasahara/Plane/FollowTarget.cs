using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 仕込み役：人力手押し・牽引
/// ＊仕込み物にこのスクリプトをアタッチして下さい。
/// </summary>
public class FollowTarget : PlaneBehaviourParent {

	/// <summary>
	/// 追従状態かどうか
	/// </summary>
	public bool IsFollowed {
		get {
			return this.isFollowed;
		}
		set {
			this.isFollowed = value;
			this.enabled = value;

			if(value == false && this.AutoMoveCar != null) {
				// 牽引終了
				this.AutoMoveCar.Disable();
			}
		}
	}

	/// <summary>
	/// 追従状態かどうかのプロパティの実体
	/// </summary>
	private bool isFollowed;

	/// <summary>
	/// 飛行機の座標X
	/// </summary>
	private float targetX;

	/// <summary>
	/// 飛行機の座標Z
	/// </summary>
	private float targetZ;

	/// <summary>
	/// 自分の座標X
	/// </summary>
	private float ownX;

	/// <summary>
	/// 自分の座標Z
	/// </summary>
	private float ownZ;

	/// <summary>
	/// 初期状態の飛行機と自分の間の距離X
	/// </summary>
	private float distanceX;

	/// <summary>
	/// 初期状態の飛行機と自分の間の距離Z
	/// </summary>
	private float distanceZ;

	/// <summary>
	/// 牽引のみ：追従しながら車と飛行機の力を連動させるオブジェクト
	/// </summary>
	public AutoMoveCar AutoMoveCar;

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

		this.IsFollowed = true;
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
