using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各フェーズの基底クラス
/// </summary>
public abstract class PhaseBase {

	/// <summary>
	/// フェーズ管理クラスのインスタンス
	/// </summary>
	protected PhaseManager parent;

	/// <summary>
	/// 引継用パラメーター
	/// 前のフェーズからのデータを受け取るのに使います。
	/// インターフェースはフェーズ同士で取り決めが必要です。
	/// </summary>
	protected object[] parameters;

	/// <summary>
	/// 毎フレーム呼び出すUpdate関数が有効であるかどうか
	/// </summary>
	public bool IsUpdateEnabled {
		get;
		protected set;
	}

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="param">引継用パラメーター</param>
	public PhaseBase(PhaseManager parent, object[] parameters) {
		this.parent = parent;
		this.parameters = parameters;
		this.IsUpdateEnabled = true;
	}

	/// <summary>
	/// このフェーズが有効になったときに最初に実行される処理
	/// 明転前に呼び出されます。
	/// </summary>
	public virtual void Start() {
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public abstract void Update();

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public virtual void Destroy() {
	}

	/// <summary>
	/// 前のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>前のフェーズのインスタンス</returns>
	public abstract PhaseBase GetPreviousPhase();

	/// <summary>
	/// 次のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>次のフェーズのインスタンス</returns>
	public abstract PhaseBase GetNextPhase();
	
}
