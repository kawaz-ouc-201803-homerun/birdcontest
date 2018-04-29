using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームマスターとしてのトップ制御クラス
/// 
/// ＊フェーズの管理を行い、その中のロジックは移譲する
/// ＊本シーンの端末操作を行うのは、イベントスタッフのみ
/// ＊シーンを分けないのは、１つのシーン内で状態遷移を行うため
/// 
/// </summary>
public class PhaseManager : MonoBehaviour {

	/// <summary>
	/// フェーズごとのUI親オブジェクト
	/// </summary>
	public GameObject[] PhaseUIs;

	/// <summary>
	/// 現在のフェーズ
	/// </summary>
	private PhaseBase phase;

	/// <summary>
	/// フェーズのインデックス
	/// </summary>
	private int phaseIndex {
		get {
			return this._phaseIndex;
		}
		set {
			if(value < 0 || this.PhaseUIs.Length <= value) {
				return;
			}

			// 対応するUIブロックに表示を切り替える
			this.PhaseUIs[this._phaseIndex].SetActive(false);
			this._phaseIndex = value;
			this.PhaseUIs[value].SetActive(true);
		}
	}
	private int _phaseIndex;

	/// <summary>
	/// 前のフェーズに戻るキーのフレームカウンター
	/// </summary>
	private int previousKeyCounter;

	/// <summary>
	/// 次のフェーズに進むキーのフレームカウンター
	/// </summary>
	private int nextKeyCounter;

	/// <summary>
	/// 初期設定
	/// </summary>
	void Start() {
		// フェーズをアイドル状態にセット
		this.phase = new PhaseIdle();

		// 各種変数を初期化
		this.phaseIndex = 0;
		this.previousKeyCounter = 0;
		this.nextKeyCounter = 0;
	}

	/// <summary>
	/// 毎フレームの処理
	/// </summary>
	void Update() {
#if UNITY_EDITOR
		// デバッグ時のみ有効な処理

		// 戻るボタン
		if(Input.GetKeyDown(KeyCode.Escape) == true) {
			this.previousKeyCounter++;
			if(this.previousKeyCounter == 1) {
				// 前のフェーズへ戻る
				switch(this.phase.GetType().Name) {
					case "PhaseIdle":
						// これ以上戻れない
						break;

					case "PhaseControllers":
						this.phase = new PhaseIdle();
						this.phaseIndex--;
						break;

					case "PhaseFlight":
						this.phase = new PhaseControllers();
						this.phaseIndex--;
						break;

					case "PhaseResult":
						this.phase = new PhaseFlight(null);
						this.phaseIndex--;
						break;
				}
			}
		} else {
			this.previousKeyCounter = 0;
		}

		// 進むボタン
		if(Input.GetKeyDown(KeyCode.Return) == true) {
			this.nextKeyCounter++;
			if(this.nextKeyCounter == 1) {
				// 前のフェーズへ戻る
				switch(this.phase.GetType().Name) {
					case "PhaseIdle":
						this.phase = new PhaseControllers();
						this.phaseIndex++;
						break;

					case "PhaseControllers":
						this.phase = new PhaseFlight(null);
						this.phaseIndex++;
						break;

					case "PhaseFlight":
						this.phase = new PhaseResult(null);
						this.phaseIndex++;
						break;

					case "PhaseResult":
						// これ以上進めない
						break;
				}
			}
		} else {
			this.nextKeyCounter = 0;
		}
#endif

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
		this.phaseIndex++;

		// 次のフェーズがないとき、ゲームの１サイクルが終了したとみなして現在のシーンをリロードする
		if(this.phase == null) {
			SceneManager.LoadScene("GameMaster");
		}
	}

}
