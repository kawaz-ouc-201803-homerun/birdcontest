﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubGame {

	/// <summary>
	/// ３ボタン同時押しミニゲーム
	/// 入力すべきボタンを表示するUI
	/// </summary>
	public class ButtonUIPushButtons : MonoBehaviour {

		/// <summary>
		/// ボタン表示を隠すかどうか
		/// </summary>
		public static bool IsHidden;

		/// <summary>
		/// 入力すべきボタンを示すテキストUIオブジェクト
		/// </summary>
		public Text[] TextButtons;

		/// <summary>
		/// 初回処理
		/// </summary>
		void Start() {
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		void Update() {
			// 右手のボタン
			this.TextButtons[2].text = ButtonUIPushButtons.IsHidden ? "" : ((int)SubGamePushButtons.AvailableKeys[0] - SubGamePushButtons.KeyCodeBase + 1).ToString();
			if(Input.GetKey(SubGamePushButtons.AvailableKeys[0]) == true) {
				this.TextButtons[2].color = Color.cyan;
			} else {
				this.TextButtons[2].color = Color.white;
			}

			// LRのボタン
			this.TextButtons[1].text = ButtonUIPushButtons.IsHidden ? "" : ((int)SubGamePushButtons.AvailableKeys[1] - SubGamePushButtons.KeyCodeBase + 1).ToString();
			if(Input.GetKey(SubGamePushButtons.AvailableKeys[1]) == true) {
				this.TextButtons[1].color = Color.cyan;
			} else {
				this.TextButtons[1].color = Color.white;
			}

			// 左手のボタン（＝十字キーorスティック）
			this.TextButtons[0].text = ButtonUIPushButtons.IsHidden ? "" : SubGamePushButtons.AxisName;
			if(Input.GetAxis(SubGamePushButtons.AxisCodeName) * SubGamePushButtons.AxisDirection > SubGamePushButtons.StickPowerThreshold) {
				this.TextButtons[0].color = Color.cyan;
			} else {
				this.TextButtons[0].color = Color.white;
			}
		}

	}

}
