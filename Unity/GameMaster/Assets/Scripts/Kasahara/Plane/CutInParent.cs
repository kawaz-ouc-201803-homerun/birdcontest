using UnityEngine;

/// <summary>
/// 援護役：カットインを行うための基底クラス
/// </summary>
public class CutInParent : PlaneBehaviourParent {

	/// <summary>
	/// 実況ステップ制御オブジェクト
	/// </summary>
	public StreamTextStepController StreamController;

	/// <summary>
	/// 対象キャラクターの画像が子になっているPanel
	/// </summary>
	public GameObject TargetCharacterPanel;

	/// <summary>
	/// 対象キャラクターの画像が子になっているPanelについているAnimatorコンポーネント
	/// </summary>
	public Animator TargetCharacterAnimator;

	/// <summary>
	/// トリガーによってカットインを行うかどうか
	/// </summary>
	public bool IsCutinEnabled = false;

	/// <summary>
	/// 演出処理時間のタイマー
	/// </summary>
	private float sumTime = 0;

	/// <summary>
	/// カットインに要する時間秒数
	/// </summary>
	public const float CutinTimeSeconds = 3.5f;

	/// <summary>
	/// トリガー対象が接したらこのスクリプトの処理を有効化します。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public virtual void OnTriggerEnter(Collider other) {
		if(this.IsCutinEnabled == false || other.gameObject.tag != "Trigger") {
			return;
		}

		// NOTE: 以後、Update系のメソッドが走るようになる
		this.enabled = true;
		Debug.Log("カットイントリガー発動");

		// 飛行をポーズしてカットイン演出を開始
		this.TargetCharacterPanel.SetActive(true);
		this.TargetCharacterAnimator.SetTrigger("Start");
		Time.timeScale = 0;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// ＊カットイン終了後に適用したい処理をここに定義して下さい。
	/// </summary>
	protected virtual void FixedUpdate() {
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// カットイン演出を行います。
	/// </summary>
	public void Update() {
		if(this.sumTime <= CutInParent.CutinTimeSeconds) {
			// タイムスケールに依存しないリアルの時間で計測する
			this.sumTime += Time.unscaledDeltaTime;
		}
		if(this.sumTime > CutInParent.CutinTimeSeconds && this.IsCutinEnabled == true) {
			// 一定時間経過でアニメーション終了
			this.StopAnimation();
		}
	}

	/// <summary>
	/// アニメーション終了処理
	/// </summary>
	private void StopAnimation() {
		// 飛行のポーズを解除
		this.IsCutinEnabled = false;
		this.TargetCharacterPanel.SetActive(false);
		Time.timeScale = 1;

		// 実況更新は個別に行う
		// this.StreamController.CurrentFlightGameStep = StreamTextStepController.FlightStep.EndSupport;
	}

}
