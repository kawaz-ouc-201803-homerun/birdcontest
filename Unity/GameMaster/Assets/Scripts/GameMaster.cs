using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームマスターとしてのトップ制御クラス
/// 
/// ＊フェーズの管理を行い、その中のロジックは移譲する
/// ＊本シーンの端末操作を行うのは、イベントスタッフのみ
/// 
/// </summary>
public class GameMaster : MonoBehaviour {

	/// <summary>
	/// 現在のフェーズ
	/// </summary>
	private PhaseBase phase;

	/// <summary>
	/// 初期設定
	/// </summary>
	void Start() {
		// フェーズをアイドル状態にセット
		this.phase = new PhaseIdle();

		// TODO: 各種変数を初期化
	}

	/// <summary>
	/// 毎フレームの処理
	/// </summary>
	void Update() {
		// TODO: ゲームマスター共通の処理

		this.phase.Update();
	}

	/// <summary>
	/// 次のフェーズに移行するときのイベントを捕捉したとき
	/// </summary>
	/// <param name="sender">イベント発生源</param>
	/// <param name="e">イベント引数</param>
	private void changePhaseEventHandler(object sender, PhaseBase.NextPhaseEventArgs e) {
		this.phase = e.NextPhase;

		// 次のフェーズがないとき、ゲームの１サイクルが終了したとみなして現在のシーンをリロードする
		if(this.phase == null) {
			SceneManager.LoadScene("GameMaster");
		}
	}

}
