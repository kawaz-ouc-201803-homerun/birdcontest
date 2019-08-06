using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// クエリちゃんのボイス再生制御
/// </summary>
public class SapphiartChanVoicePlayer : SEPlayerBase {

	/// <summary>
	/// ボイスID
	/// </summary>
	public enum SEID {
		Cheerup1,       // 応援1
		Cheerup2,       // 応援2
		Cheerup3,       // 応援3
		BombStart,      // 爆弾の掛け声
	}

}
