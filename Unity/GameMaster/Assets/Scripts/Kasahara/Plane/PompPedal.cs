using UnityEngine;

/// <summary>
/// 離陸後に飛行機に推進力を与えます。
/// ＊このスクリプトは飛行機に直接アタッチして下さい。
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
		if(other.gameObject.tag == "Stopper") {
			// NOTE: 以後、Update系のメソッドが走るようになる
			Debug.Log("飛行役「ペダル航行」開始");
			this.enabled = true;
		}
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// ＊タイムスケールに依存します。
	/// </summary>
	public void FixedUpdate() {
		if(this.PedalPower > 0) {
			// 毎フレーム減衰させながら、前方に力を加える
			this.PedalPower -= Time.deltaTime * 100f;
			this.PlaneRigidbody.AddForce(this.transform.forward * this.PedalPower, ForceMode.Force);

			// NOTE: バランス調整のため、上方向に力を加えるのはあくまでも仕込み役とサポート役に限定させる
			// this.PlaneRigidbody.AddForce(this.transform.up * this.PedalPower, ForceMode.Force);
		}
	}
}