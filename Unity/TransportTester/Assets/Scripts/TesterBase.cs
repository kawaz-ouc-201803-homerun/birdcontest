using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 実行可能なテスターの基本クラス
/// </summary>
public abstract class TesterBase : MonoBehaviour {
	
	/// <summary>
	/// 通信機能
	/// </summary>
	protected NetworkConnector connector;

	/// <summary>
	/// テストパラメーター
	/// </summary>
	protected Dictionary<string, object> parameters;

	/// <summary>
	/// テストを実行します。
	/// </summary>
	/// <param name="parameters">テストに必要なパラメーターの連想配列</param>
	public abstract void DoTest(Dictionary<string, object> parameters);

}
