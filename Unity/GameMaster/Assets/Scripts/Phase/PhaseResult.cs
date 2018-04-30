using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：結果表示
/// 
/// ＊スコア表示
/// ＊メッセージ表示
/// 
/// </summary>
public class PhaseResult : PhaseBase {

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="param">[0]=イベントID, [1]=飛距離</param>
	public PhaseResult(PhaseManager parent, object[] parameters) : base(parent, parameters) {
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		
	}

}
