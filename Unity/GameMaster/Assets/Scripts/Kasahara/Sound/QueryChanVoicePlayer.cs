using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// クエリちゃんのボイス再生制御
/// </summary>
public class QueryChanVoicePlayer : SEPlayerBase {

	/// <summary>
	/// クエリちゃん専用のボイス制御コンポーネント
	/// </summary>
	public QuerySoundController VoicePlayer;

	/// <summary>
	/// ボイスID
	/// </summary>
	public enum SEID {
		BombStart = QuerySoundController.QueryChanSoundType.ONE_TWO,      // せーの！
		HumanStart = QuerySoundController.QueryChanSoundType.GO_AHEAD,    // いきますよー！
		CarStart = QuerySoundController.QueryChanSoundType.FOLLOW_ME,     // ついてきてくださいねー！
		CarRacing = QuerySoundController.QueryChanSoundType.HAWAWAWA,	  // はわわわわ
		Ending = QuerySoundController.QueryChanSoundType.SEE_YOU,         // いってらっしゃーい！
	}

	/// <summary>
	/// SEを通常再生します。
	/// </summary>
	/// <param name="SEID">SEのID</param>
	public override void PlaySE(int SEID) {
		this.VoicePlayer.PlaySoundByType((QuerySoundController.QueryChanSoundType)SEID);
	}

}
