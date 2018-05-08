using System;
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
	/// ゲームタイトル
	/// </summary>
	public const string GameTitle = "鳥人コンテスト";

	/// <summary>
	/// RPG風メッセージ送りで、メッセージを１文字進めるまでに待機する秒数
	/// </summary>
	public const float MessageSpeed = 0.03f;

	/// <summary>
	/// 各種フェーズクラスに対応するインデックスの定義
	/// </summary>
	public static readonly Dictionary<Type, int> PhaseIndexMap = new Dictionary<Type, int>() {
		{ typeof(PhaseIdle), 0 },
		{ typeof(PhaseControllers), 1 },
		{ typeof(PhaseFlight), 2 },
		{ typeof(PhaseResult), 3 },
		{ typeof(PhaseCredit), 4 },
	};

	/// <summary>
	/// トランジションにかける秒数
	/// </summary>
	private const float TransitionTimeSecond = 2.0f;

	/// <summary>
	/// フェーズごとのUI親オブジェクト
	/// </summary>
	public GameObject[] PhaseUIs;

	/// <summary>
	/// バックドアの管理オブジェクト
	/// </summary>
	public BackDoorOpenTrigger BackDoorManager;

	/// <summary>
	/// 現在のフェーズ
	/// </summary>
	public PhaseBase Phase;

	/// <summary>
	/// フェーズのインデックス
	/// </summary>
	public int PhaseIndex {
		get;
		private set;
	}

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
	public void Start() {
		// フェーズをアイドル状態にセット
		this.ChangePhase(new PhaseIdle(this));

		// IPアドレスのバックドアを開く
		this.BackDoorManager.GetComponent<BackDoorOpenTrigger>().ChangeBackDoor((int)BackDoorOpenTrigger.BackDoorUIIndex.IPAddress);

		// 各種変数を初期化
		this.previousKeyCounter = 0;
		this.nextKeyCounter = 0;
	}

	/// <summary>
	/// 毎フレームの処理
	/// </summary>
	public void Update() {
		if(BackDoorOpenTrigger.IsBackDoorOpened == true) {
			// バックドアが開いているときは操作を無効にする
			return;
		}

		// 戻るボタン
		if(Input.GetKeyDown(KeyCode.Escape) == true) {
			this.previousKeyCounter++;

			if(this.previousKeyCounter == 1) {
				// 前のフェーズへ戻る
				switch(this.Phase.GetType().Name) {
					case "PhaseIdle":
						// これ以上戻れない
						break;

					case "PhaseControllers":
						this.ChangePhase(new PhaseIdle(this));
						break;

					case "PhaseFlight":
						this.ChangePhase(new PhaseControllers(this));
						break;

					case "PhaseResult":
						this.ChangePhase(new PhaseFlight(this, null));
						break;

					case "PhaseCredit":
						this.ChangePhase(new PhaseResult(this, null));
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
				switch(this.Phase.GetType().Name) {
					case "PhaseIdle":
						this.ChangePhase(new PhaseControllers(this));
						break;

					case "PhaseControllers":
						// this.ChangePhase(new PhaseFlight(this, null));
						((PhaseControllers)this.Phase).ChangeToFlightPhase();
						break;

					case "PhaseFlight":
						// this.ChangePhase(new PhaseResult(this, null));
						((PhaseFlight)this.Phase).ChangeToResultPhase();
						break;

					case "PhaseResult":
						this.ChangePhase(new PhaseCredit(this));
						break;

					case "PhaseCredit":
						// 最初のフェーズに戻す
						this.ChangePhase(new PhaseIdle(this));
						break;
				}
			}
		} else {
			this.nextKeyCounter = 0;
		}

		if(this.Phase != null
		&& this.Phase.IsUpdateEnabled == true) {
			// 現在のフェーズの毎フレーム更新処理
			this.Phase.Update();
		}
	}

	/// <summary>
	/// フェーズを変更します。
	/// </summary>
	/// <param name="phase">フェーズオブジェクト</param>
	public void ChangePhase(PhaseBase phase) {
		this.PhaseIndex = PhaseManager.PhaseIndexMap[phase.GetType()];
		var nextPhaseCallback = new Action(() => {
			// シーン移行前にすべてのコルーチンを停止しておく
			this.StopAllCoroutines();
			iTween.Stop();

			// フェーズ破棄処理
			var hasBeforePhase = (this.Phase != null);
			if(this.Phase != null) {
				this.Phase.Destroy();
			}

			// 暗転後にフェーズ切り替え、対応するUIブロックに表示を切り替える
			for(int i = 0; i < PhaseManager.PhaseIndexMap.Count; i++) {
				this.PhaseUIs[i].SetActive(false);
			}
			this.PhaseUIs[this.PhaseIndex].SetActive(true);
			this.Phase = phase;

			// 初回処理をここで実行
			this.Phase.Start();

			if(hasBeforePhase == true) {
				// 明転開始
				this.DoTransitionIn(PhaseManager.TransitionTimeSecond);
			}
		});

		if(this.Phase == null) {
			// 前のシーンがない場合はすぐにシーンを開始する
			nextPhaseCallback();
		} else {
			// 前のシーンがある場合は暗転を挟む
			this.DoTransitionOut(1.0f, nextPhaseCallback);
		}
	}

	/// <summary>
	/// 画面を表示するトランジションを実行します。
	/// </summary>
	/// <param name="time">処理時間秒数</param>
	/// <param name="callback">処理完了後に呼び出されるコールバック関数</param>
	public void DoTransitionIn(float time, Action callback = null) {
		GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeOut(time, callback);
	}

	/// <summary>
	/// 画面を消去するトランジションを実行します。
	/// </summary>
	/// <param name="time">処理時間秒数</param>
	/// <param name="callback">処理完了後に呼び出されるコールバック関数</param>
	public void DoTransitionOut(float time, Action callback = null) {
		GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeIn(time, callback);
	}

}
