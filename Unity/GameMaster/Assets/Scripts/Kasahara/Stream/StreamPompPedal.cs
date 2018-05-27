using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛行中の実況更新を行います。
/// </summary>
public class StreamPompPedal : MonoBehaviour {

	/// <summary>
	/// 端末操作結果データオブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// 実況ステップ制御オブジェクト
	/// </summary>
	public StreamTextStepController StreamController;

	/// <summary>
	/// ユニティちゃんのボイス制御オブジェクト
	/// </summary>
	public UnityChanVoicePlayer UnityChanVoice;

	/// <summary>
	/// 機体がこのオブジェクトに触れたときに発動させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			this.StreamController.CurrentFlightGameStep = StreamTextStepController.FlightStep.Flighting;

			if(this.DataContainer.OptionA != (int)PhaseControllers.OptionA.Bomb) {
				// ついでにユニティちゃんのボイスも再生する（爆発時は気絶しているので言葉を発しない）
				this.UnityChanVoice.PlaySE((int)UnityChanVoicePlayer.UnityChanVoiceIndexes.Flying);
			}
		}
	}

}
