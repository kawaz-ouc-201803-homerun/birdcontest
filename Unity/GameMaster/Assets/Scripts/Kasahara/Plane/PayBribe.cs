using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 援護役：賄賂カットイン
/// これが有効である場合、着地後にスコアを調整します。
/// </summary>
public class PayBribe : CutInParent {

	/// <summary>
	/// 賄賂カットインを実行したかどうか
	/// </summary>
	private bool isCutinDone = false;

	/// <summary>
	/// SE再生制御オブジェクト
	/// </summary>
	public SEPlayer SEPlayer;

	/// <summary>
	/// トリガー対象が接したらこのスクリプトの処理を有効化します。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public override void OnTriggerEnter(Collider other) {
		// SE再生
		base.OnTriggerEnter(other);
		if(this.enabled == true && this.isCutinDone == false) {
			// カットインが開始したときにSEを鳴らす
			this.SEPlayer.PlaySE((int)SEPlayer.SEID.WairoCutIn);
		}
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// ＊カットイン終了後に適用したい処理をここに定義して下さい。
	/// </summary>
	protected override void FixedUpdate() {
		if(this.isCutinDone == false) {
			this.isCutinDone = true;

			// 実況更新
			this.StreamController.CurrentFlightGameStep = StreamTextStepController.FlightStep.StartSupport;
		}
	}
}
