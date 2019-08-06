using System.Collections;
using UnityEngine;

/// <summary>
/// ユニティちゃんのボイス再生制御
/// </summary>
public class UnityChanVoicePlayer : SEPlayerBase {

	/// <summary>
	/// ユニティちゃんボイスの配列インデックス
	/// </summary>
	public enum UnityChanVoiceIndexes {
		Starting,       // 発進時
		Flying,         // 飛んでいるとき
	}

}
