using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// システムSEの再生制御を行います。
/// </summary>
public class SystemSEPlayer : SEPlayerBase {

	/// <summary>
	/// システムSEのID
	/// </summary>
	public enum SystemSEID {
		Dicision,           // 決定音
		Cancel,             // キャンセル音
		ControllerEnd,      // 端末の操作が完了したときのチェック音
		WairoScoreUp,       // 賄賂効果でスコアが上昇する音
		FlightEnd,          // フライト終了音
		AnnounceResult,     // アナウンス「結果を発表します」
		AnnounceWait,       // アナウンス「しばらくお待ち下さい」
		Ready,              // アナウンス「レディー？」
		Go,                 // アナウンス「ゴー！」
		Evaluation1,        // グッド！
		Evaluation2,        // エクセレント！
		Evaluation3,        // マーベラス！
	}

	/// <summary>
	/// サウンドのエコーフィルターコンポーネント
	/// </summary>
	public AudioEchoFilter EchoFilter;

	/// <summary>
	/// サウンドのリバーブフィルターコンポーネント
	/// </summary>
	public AudioReverbFilter ReverbFilter;

	/// <summary>
	/// SEを通常再生します。
	/// </summary>
	/// <param name="SEID">SEのID</param>
	public override void PlaySE(int SEID) {
		this.EchoFilter.enabled = false;
		this.ReverbFilter.enabled = false;
		base.PlaySE(SEID);
	}

	/// <summary>
	/// SEをエコー付きで再生します。
	/// ＊アナウンス用
	/// </summary>
	/// <param name="SEID">SEのID</param>
	public void PlaySEWithEcho(int SEID) {
		// this.EchoFilter.enabled = true;
		this.ReverbFilter.enabled = true;
		base.PlaySE(SEID);
	}

}
