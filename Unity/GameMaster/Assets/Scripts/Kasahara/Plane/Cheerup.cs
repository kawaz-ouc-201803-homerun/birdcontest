using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 援護役：応援カットイン
/// </summary>
public class Cheerup : CutInParent {

	/// <summary>
	/// 応援によって機体を上昇させる力の大きさ
	/// ＊ParamCから代入
	/// </summary>
	public float UpperPower;


	/// <summary>
	/// 毎フレーム更新処理
	/// ＊カットイン終了後に適用したい処理をここに定義して下さい。
	/// </summary>
	protected override void FixedUpdate() {
		if(this.UpperPower > 0) {
			// １秒ごとに飛ぶ力を減衰させていく
			this.UpperPower = this.UpperPower - 30f * Time.deltaTime;
			this.PlaneRigidbody.AddForce(this.transform.up * this.UpperPower, ForceMode.Impulse);
		}
	}

}
