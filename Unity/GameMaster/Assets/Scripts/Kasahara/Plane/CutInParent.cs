using UnityEngine;

/// <summary>
/// 援護役：カットインを行うための基底クラス
/// </summary>
public class CutInParent : PlaneBehaviourParent {

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
	void OnTriggerEnter(Collider other) {
		if(this.IsCutinEnabled == false || other.gameObject.tag != "Trigger") {
			return;
		}

		// NOTE: 以後、Update系のメソッドが走るようになる
		this.enabled = true;

		// カットイン演出を開始
		this.TargetCharacterPanel.SetActive(true);
		this.TargetCharacterAnimator.SetTrigger("Start");

		// ポーズ開始
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
	void Update() {
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
		this.IsCutinEnabled = false;
		this.TargetCharacterPanel.SetActive(false);

		// ポーズ解除
		Time.timeScale = 1;
	}

}
