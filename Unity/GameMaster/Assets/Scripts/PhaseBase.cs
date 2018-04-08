using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各フェーズの基底クラス
/// </summary>
public abstract class PhaseBase : MonoBehaviour {

	/// <summary>
	/// 次のフェーズに移行するときのイベントの引数
	/// </summary>
	public class NextPhaseEventArgs {
		
		/// <summary>
		/// 次のフェーズ名
		/// </summary>
		public PhaseBase NextPhase {
			get; set;
		}
	}

	/// <summary>
	/// 次のフェーズに移行するときのイベントのデリゲート
	/// </summary>
	/// <param name="sender">イベント発生源のオブジェクト</param>
	/// <param name="e">イベント引数</param>
	public delegate void NextPhaseEventDelegate(object sender, NextPhaseEventArgs e);

	/// <summary>
	/// 次のフェーズに移行するときのイベント
	/// </summary>
	public event NextPhaseEventDelegate NextPhaseEvent;

	/// <summary>
	/// 引継用パラメーター
	/// 前のフェーズからのデータを受け取るのに使います。
	/// インターフェースはフェーズ同士で取り決めが必要です。
	/// </summary>
	protected object[] parameters;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="param">引継用パラメーター</param>
	public PhaseBase(object[] parameters) {
		this.parameters = parameters;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public abstract void Update();

	/// <summary>
	/// 次のフェーズに移行します。
	/// </summary>
	/// <param name="nextPhase"></param>
	protected void NextPhase(PhaseBase nextPhase) {
		if(this.NextPhaseEvent != null) {
			// イベント発生：ゲームマスターが捕捉してフェーズを切り替える
			this.NextPhaseEvent.Invoke(
				this,
				new NextPhaseEventArgs {
					NextPhase = nextPhase,
				}
			);
		}
	}
	
}
