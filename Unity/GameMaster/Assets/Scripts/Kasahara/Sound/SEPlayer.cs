using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛行フェーズ中に使うSEの再生制御を行います。
/// </summary>
public class SEPlayer : SEPlayerBase {

	/// <summary>
	/// SEのID
	/// </summary>
	public enum SEID {
		HumanStep,          // 端末Aの手押し走行音
		Explosion,          // 爆発音
		WairoCutIn,         // 賄賂カットインのイメージ音
	}

}
