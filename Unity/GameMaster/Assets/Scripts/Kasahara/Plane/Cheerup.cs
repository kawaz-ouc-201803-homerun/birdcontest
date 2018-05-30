using System.Collections;
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
	/// 応援効果を実行したかどうか
	/// </summary>
	private bool isCheerupDone = false;

	/// <summary>
	/// 機体の上昇力の１フレームあたりの減衰量
	/// </summary>
	private float upperPowerDecay = -1;

	/// <summary>
	/// 応援効果の開始時刻
	/// </summary>
	private float upperStartTime = 0;

	/// <summary>
	/// サファイアートちゃんボイス再生制御オブジェクト
	/// </summary>
	public SapphiartChanVoicePlayer VoicePlayer;

	/// <summary>
	/// 毎フレーム更新処理
	/// ＊カットイン終了後に適用したい処理をここに定義して下さい。
	/// </summary>
	protected override void FixedUpdate() {
		if(this.UpperPower > 0 && this.isCheerupDone == false) {
			if(this.upperPowerDecay < 0) {
				// 応援効果の初回処理
				this.upperPowerDecay = this.UpperPower / 1.0f;
				this.upperStartTime = Time.unscaledTime;
			}

			// 毎フレームで力を加え続ける
			this.PlaneRigidbody.AddForce(this.transform.up * this.UpperPower, ForceMode.Acceleration);

			// 減衰タイマー計算
			if(Time.unscaledTime - this.upperStartTime >= 1.0f) {
				// 減衰開始 or 減衰中
				//this.UpperPower -= this.upperPowerDecay * Time.fixedUnscaledDeltaTime;
				this.UpperPower -= this.upperPowerDecay * Time.fixedDeltaTime;
				if(this.UpperPower <= 0) {
					// 上昇完了
					this.isCheerupDone = true;
					Debug.Log("応援終了");

					// 実況更新
					this.StreamController.CurrentFlightGameStep = StreamTextStepController.FlightStep.EndSupport;
				}
			}
		}
	}

	/// <summary>
	/// 応援カットイン開始時にボイスを挟みます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
		if(this.IsCutinEnabled == true && other.gameObject.tag == "Trigger") {
			this.VoicePlayer.PlaySE(Random.Range(
				(int)SapphiartChanVoicePlayer.SEID.Cheerup1,
				(int)SapphiartChanVoicePlayer.SEID.Cheerup3 + 1
			));
		}
	}

}
