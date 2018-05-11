using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ランダムに変動するメーターをタイミングよく止めるミニゲーム
/// </summary>
public class SubGameMeterStop : SubGameBase {

	/// <summary>
	/// メーターを更新する間隔秒数
	/// </summary>
	public const float ChangeMeterSpanSeconds = 0.20f;

	/// <summary>
	/// メーターの最大値
	/// </summary>
	public const float MaxMeterValue = 100f;

	/// <summary>
	/// スコア値
	/// </summary>
	public float Score;

	/// <summary>
	/// 時間計測用
	/// </summary>
	public float TimeCounter;

	/// <summary>
	/// 初回処理
	/// </summary>
	protected override void startSubGame() {
		this.Score = 0;
		this.TimeCounter = 0;

		iTween.Stop(this.gameObject);
		var slider = this.transform.Find("Slider").GetComponent<Slider>();
		slider.value = 0;
		slider.maxValue = SubGameMeterStop.MaxMeterValue;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	protected override void updateSubGame() {
		this.TimeCounter += Time.deltaTime;
		if(this.TimeCounter >= SubGameMeterStop.ChangeMeterSpanSeconds) {
			var score = (int)Random.Range(0f, SubGameMeterStop.MaxMeterValue);
			iTween.ValueTo(
				this.gameObject,
				iTween.Hash(
					"from", this.Score,
					"to", score,
					"time", 0.20f,
					"onupdate", "ValueChange"
				)
			);
			this.Score = score;
			this.TimeCounter = 0f;
		}

		// ボタン押下判定
		if(Input.GetKeyDown(KeyCode.Return) == true) {
			// タイマーを強制的にゼロにする
			this.gameObject.SetActive(false);
			GameObject.Find("Timer").GetComponent<Timer>().StopTimer(true);
		}
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public override string GetResultText() {
		return "アクセル開度 ＝ " + ((int)this.Score);
	}

	/// <summary>
	/// 進捗報告として送るデータを辞書型配列にセットします。
	/// </summary>
	/// <param name="dictionary">セットする対象の辞書型配列</param>
	public override void SetProgressData(ref Dictionary<string, string> dictionary) {
		dictionary["param"] = ((int)this.Score).ToString();
	}

	/// <summary>
	/// iTween：ゲージ更新
	/// </summary>
	/// <param name="Change">現在の値</param>
	void ValueChange(float Change) {
		GameObject.Find("Slider").GetComponent<UnityEngine.UI.Slider>().value = Change;
	}

}
