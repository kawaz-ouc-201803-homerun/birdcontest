using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末A の管理クラス
/// </summary>
public class ControllerA : ControllerBase {

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

		// TODO: データ格納
		dictionary["option"] = "";
		dictionary["param"] = "";

		return dictionary;
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public override string GetResultText() {
		// TODO: 結果テキストを作る
		return "";
	}

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public override void StartNewGame() {
		this.Start();
	}

}
