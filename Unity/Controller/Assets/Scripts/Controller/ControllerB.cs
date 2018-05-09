﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末B の管理クラス
/// </summary>
public class ControllerB : ControllerBase {

	/// <summary>
	/// 連打ミニゲームのオブジェクト
	/// </summary>
	public SubGameButtonRepeat SubGame;

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
	/// 進捗報告として送るデータを生成します。
	/// </summary>
	/// <returns>進捗報告として送る辞書型配列</returns>
	protected override Dictionary<string, string> createProgressData() {
		var dictionary = base.createProgressData();

		// データ格納
		dictionary["param"] = ((int)this.SubGame.Score).ToString();

		return dictionary;
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public override string GetResultText() {
		return "獲得スコア ＝ " + ((int)this.SubGame.Score);
	}

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public override void StartNewGame() {
		this.Start();
		this.SubGame.Start();
	}

}
