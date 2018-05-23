using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み役：飛行機に助走をつける（物理）
/// ＊このスクリプトは飛行機に直接アタッチして下さい。
/// </summary>
public class AutoMove : PlaneBehaviourParent {

	/// <summary>
	/// 前に進む力の大きさ
	/// </summary>
	private float movePower;

	/// <summary>
	/// 加える力の種類
	/// </summary>
	public ForceMode PowerForce;

	/// <summary>
	/// 毎フレーム更新処理
	/// 機体に対し、一定の力を加え続けます。
	/// </summary>
	void FixedUpdate() {
		// 推進力を毎秒 10 加える
		this.movePower = this.movePower + 10 * Time.deltaTime;

		// 飛行機にmovepowerを加える
		this.PlaneRigidbody.AddForce(this.transform.forward * this.movePower, this.PowerForce);
	}

	/// <summary>
	/// StopperタグがついているPlaneに触った時に離陸開始となり、このスクリプトを停止させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Stopper") {
			this.PlaneRigidbody.AddForce(this.transform.up * 500, ForceMode.Impulse);
			this.enabled = false;
		}
	}

}
