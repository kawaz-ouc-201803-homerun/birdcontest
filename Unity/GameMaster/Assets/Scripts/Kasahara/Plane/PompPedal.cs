using UnityEngine;

/// <summary>
/// 離陸後に飛行機に推進力を与えます。
/// </summary>
public class PompPedal : PlaneBehaviourParent {

	/// <summary>
	/// ペダルを漕ぐ力の大きさ
	/// ＊端末操作の結果に依存します。
	/// </summary>
	public float PedalPower;

	/// <summary>
	/// トリガー対象に接触したら開始します。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Pedal") {
			// NOTE: 以後、Update系のメソッドが走るようになる
			this.enabled = true;
		}
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// ＊タイムスケールに依存します。
	/// </summary>
	public void FixedUpdate() {
		if(this.PedalPower > 0) {
			// 斜め上＋前方方向に力を加える
			this.PedalPower = this.PedalPower - Time.deltaTime * 10;
			this.PlaneRigidbody.AddForce(this.transform.forward * this.PedalPower, ForceMode.Force);
			this.PlaneRigidbody.AddForce(this.transform.up * this.PedalPower, ForceMode.Force);
		}
	}
}