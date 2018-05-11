using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末C の管理クラス
/// </summary>
public class ControllerC : ControllerBase {

	/// <summary>
	/// 初回処理
	/// </summary>
	public override void Start() {
		base.Start();
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		base.Update();
	}

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public override void StartNewGame() {
		this.Start();
	}

}
