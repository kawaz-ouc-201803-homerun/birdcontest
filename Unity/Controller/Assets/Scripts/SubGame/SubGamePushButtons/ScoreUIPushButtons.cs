using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubGame {

	/// <summary>
	/// ３ボタン同時押しミニゲーム
	/// ミニゲーム中にスコア表示を行うUI
	/// </summary>
	public class ScoreUIPushButtons : MonoBehaviour {

		/// <summary>
		/// スコア表示を行うテキストUIオブジェクト
		/// </summary>
		public Text ScoreText;

		/// <summary>
		/// スコア値
		/// </summary>
		public static int Score = 0;

		/// <summary>
		/// 初回処理
		/// </summary>
		void Start() {
			ScoreUIPushButtons.Score = 0;
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		void Update() {
			this.ScoreText.text = "獲得スコア ＝ " + Score;
		}

	}

}
