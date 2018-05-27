using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubGame {

	/// <summary>
	/// ボタン連打のミニゲーム
	/// コントローラーを使うため、入力が正しく取れていない時はInputManagerの設定を確認して下さい。
	/// </summary>
	public class SubGameButtonRepeat : SubGameBase {

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
		/// SE再生制御オブジェクト
		/// </summary>
		public SEPlayer SEPlayer;

		/// <summary>
		/// 初回処理
		/// </summary>
		public override void StartSubGame() {
			base.StartSubGame();

			this.IsButtonDown = false;
			this.ButtonDownCount = 0;
			this.TimeCounter = 0;
			this.Score = 0;
			this.LastButtonDownCount = 0;

			iTween.Stop(this.gameObject);
			var slider = this.transform.Find("Slider").GetComponent<Slider>();
			slider.value = 0;
			slider.maxValue = (SubGameButtonRepeat.MaxScoreThreshold % 2 == 0) ? SubGameButtonRepeat.MaxScoreThreshold * 2 : SubGameButtonRepeat.MaxScoreThreshold * 2 - 1;

			this.transform.Find("ScoreWindow/RealTimeScore").GetComponent<Text>().text = "獲得スコア ＝ 0.00";
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		protected override void updateSubGame() {
			// ボタン押下判定
			if(Input.GetButtonDown("Click") == true) {
				if(this.IsButtonDown == false) {
					// ボタン押下開始
					this.IsButtonDown = true;
				} else {
					// ボタン押下状態から離されたときに回数カウント
					this.ButtonDownCount++;
					this.IsButtonDown = false;

					this.SEPlayer.PlaySE((int)SEPlayer.SEID.PushButton);
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

				// Debug.Log("秒間ボタン押下回数 = " + this.ButtonDownCount);
				this.LastButtonDownCount = this.ButtonDownCount;
				this.TimeCounter = 0;
				this.ButtonDownCount = 0;

				// スコア表示更新
				this.transform.Find("ScoreWindow/RealTimeScore").GetComponent<Text>().text = "獲得スコア ＝ " + string.Format("{0:0.00}", this.Score);
			}
		}

		/// <summary>
		/// 画面に表示するミニゲーム結果をテキストで返します。
		/// </summary>
		/// <returns>ミニゲーム結果テキスト</returns>
		public override string GetResultText() {
			return "獲得スコア ＝ " + ((int)this.Score);
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
			this.transform.Find("Slider").GetComponent<Slider>().value = Change;
		}

	}

}
