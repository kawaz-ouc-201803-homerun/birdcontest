using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：スタッフクレジット
/// </summary>
public class PhaseCredit : PhaseBase {

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	public PhaseCredit(PhaseManager parent) : base(parent, null) {
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update () {
	}

	/// <summary>
	/// 前のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>前のフェーズのインスタンス</returns>
	public override PhaseBase GetPreviousPhase() {
		// このフェーズでは結果データを持っていないので結果フェーズを復元できない
		return null;
	}

	/// <summary>
	/// 次のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>次のフェーズのインスタンス</returns>
	public override PhaseBase GetNextPhase() {
		return new PhaseIdle(this.parent);
	}

	/// <summary>
	/// このフェーズのBGMファイル名を返します。
	/// </summary>
	/// <returns>BGMファイル名</returns>
	public override string GetBGMFileName() {
		return "Sounds/BGM/AC【エンディング】mist";
	}

}
