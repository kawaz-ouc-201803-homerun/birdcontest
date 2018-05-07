using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタン連打のミニゲーム
/// </summary>
public class SubGameButtonRepeat : MonoBehaviour {

	/// <summary>
	/// 最大スコアにする秒間ボタン押下回数
	/// </summary>
	public const int MaxScoreThreshold = 4;

	/// <summary>
	/// ボタンが押下状態であるかどうか
	/// </summary>
	public bool IsButtonDown;

	/// <summary>
	/// ボタン押下回数
	/// </summary>
	public int ButtonDownCount;

	/// <summary>
	/// 時間計測用
	/// </summary>
	public float TimeCounter;

	/// <summary>
	/// スコア累計
	/// </summary>
	public float Score;

	/// <summary>
	/// 最後に加算された秒間ボタン押下回数
	/// </summary>
	public float LastButtonDownCount;

	/// <summary>
	/// 初回処理
	/// </summary>
	void Start() {
		var slider = GameObject.Find("Slider").GetComponent<Slider>();
		slider.value = 0;
		slider.maxValue = (SubGameButtonRepeat.MaxScoreThreshold % 2 == 0) ? SubGameButtonRepeat.MaxScoreThreshold * 2 : SubGameButtonRepeat.MaxScoreThreshold * 2 - 1;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	void Update() {
		// ボタン押下判定
		if(Input.GetButtonDown("Click") == true) {
			if(this.IsButtonDown == false) {
				// ボタン押下開始
				this.IsButtonDown = true;
			} else {
				// ボタン押下状態から離されたときに回数カウント
				this.ButtonDownCount++;
				this.IsButtonDown = false;
			}
		}

		// 一定間隔で実行する処理
		this.TimeCounter += Time.deltaTime;
		if(this.TimeCounter >= 1.0f) {
			// メーター更新
			iTween.ValueTo(
				this.gameObject,
				iTween.Hash(
					"from", this.LastButtonDownCount,
					"to", this.ButtonDownCount,
					"time", 1.0f,
					"onupdate", "ValueChange"
				)
			);

			// スコア計算＆加算
			var score = (SubGameButtonRepeat.MaxScoreThreshold - Mathf.Abs(this.ButtonDownCount - SubGameButtonRepeat.MaxScoreThreshold)) / (float)SubGameButtonRepeat.MaxScoreThreshold;
			this.Score += score;

			Debug.Log("秒間ボタン押下回数 = " + this.ButtonDownCount);
			this.LastButtonDownCount = this.ButtonDownCount;
			this.TimeCounter = 0;
			this.ButtonDownCount = 0;

			// スコア表示更新
			GameObject.Find("RealTimeScore").GetComponent<UnityEngine.UI.Text>().text = this.Score.ToString();
		}
	}

	/// <summary>
	/// iTween：ゲージ更新
	/// </summary>
	/// <param name="Change">現在の値</param>
	void ValueChange(float Change) {
		GameObject.Find("Slider").GetComponent<UnityEngine.UI.Slider>().value = Change;
	}

}