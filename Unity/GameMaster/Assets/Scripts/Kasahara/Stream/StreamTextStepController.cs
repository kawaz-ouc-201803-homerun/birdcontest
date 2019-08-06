using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛行中の実況ステップを制御します。
/// </summary>
public class StreamTextStepController : MonoBehaviour {

	/// <summary>
	/// 飛行フローのステップ
	/// </summary>
	public enum FlightStep {
		Opening,            // オープニング
		Preparing,          // 仕込み開始後
		StartFlight,        // 飛行開始
		Flighting,          // 飛行中間
		StartSupport,       // 援護開始
		EndSupport,         // 援護終了
		EndFlight,          // 飛行着地
	}

	/// <summary>
	/// 現在の飛行フローのステップ
	/// </summary>
	public FlightStep CurrentFlightGameStep {
		get; set;
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		this.CurrentFlightGameStep = FlightStep.Opening;
	}

}
