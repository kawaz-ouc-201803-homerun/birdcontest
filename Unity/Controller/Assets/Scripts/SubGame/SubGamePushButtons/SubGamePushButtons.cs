﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ３ボタン同時押しミニゲーム
/// コントローラーを使うため、入力が正しく取れていない時はInputManagerの設定を確認して下さい。
/// </summary>
public class SubGamePushButtons : SubGameBase {

	/// <summary>
	/// 同時に押させるボタンの数
	/// </summary>
	public const int KeyCount = 3;

	/// <summary>
	/// 押すべきでないボタンを判定する数
	/// </summary>
	public const int DummyKeyCount = 5;

	/// <summary>
	/// ボタン番号の基点
	/// </summary>
	public const int KeyCodeBase = (int)KeyCode.Joystick1Button0;

	/// <summary>
	/// スティック方向のパターン数
	/// </summary>
	public const int StickPatternCount = 4;

	/// <summary>
	/// スティック入力の判定閾値
	/// </summary>
	public const float StickPowerThreshold = 0.8f;

	/// <summary>
	/// 入力可能なボタン一覧
	/// </summary>
	public static KeyCode[] AvailableKeys = new KeyCode[SubGamePushButtons.KeyCount + SubGamePushButtons.DummyKeyCount];

	/// <summary>
	/// スティックのコードネーム
	/// </summary>
	private string axisCodeName;

	/// <summary>
	/// スティックの倒している方向軸名
	/// Vertical=縦倒し / Horizontal=横倒し
	/// </summary>
	public static string AxisName;

	/// <summary>
	/// スティックの軸基準で倒している方向
	/// 正方向=プラス / 逆方向=マイナス
	/// </summary>
	private int axisDirection;

	/// <summary>
	/// 初回処理
	/// </summary>
	protected override void startSubGame() {
		// スコア初期化
		ScoreUIPushButtons.Score = 0;
		ButtonUIPushButtons.IsHidden = true;

		// 最初の入力ボタンを決定する
		this.SetRandomKey();
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	protected override void updateSubGame() {
		ButtonUIPushButtons.IsHidden = false;

		// デバッグ用: 入力ボタンのチェック
		/*
		for(int keyCode = (int)KeyCode.Joystick1Button0; keyCode <= (int)KeyCode.Joystick1Button8; keyCode++) {
			if(Input.GetKeyDown((KeyCode)keyCode) == true) {
				Debug.Log(keyCode + " 押下");
			}
		}
		if(Input.GetAxis("Vertical") != 0) {
			Debug.Log("スティック V=" + Input.GetAxis("Vertical"));
		}
		if(Input.GetAxis("Horizontal") != 0) {
			Debug.Log("スティック H=" + Input.GetAxis("Horizontal"));
		}
		*/

		// 入力すべきでないダミーボタンの押下判定
		for(int i = SubGamePushButtons.KeyCount; i < SubGamePushButtons.AvailableKeys.Length; i++) {
			if(Input.GetKey(SubGamePushButtons.AvailableKeys[i]) == true) {
				// 入力されていたら判定中止
				Debug.Log("入力すべきでないボタンが押されています: KeyCode=" + i);
				return;
			}
		}

		// 入力すべきボタンの押下判定
		// NOTE: キーボードでは３キーを同時に入力できない環境が多いため、ゲームパッドによる入力に変更しました。
		if(Input.GetKey(SubGamePushButtons.AvailableKeys[0]) == true
		&& Input.GetKey(SubGamePushButtons.AvailableKeys[1]) == true
		&& Input.GetAxis(this.axisCodeName) * this.axisDirection > SubGamePushButtons.StickPowerThreshold) {

			// 点数加算
			ScoreUIPushButtons.Score++;

			// 次の入力ボタンを決定する
			this.SetRandomKey();

		}
	}

	/// <summary>
	/// 進捗報告として送るデータを辞書型配列にセットします。
	/// </summary>
	/// <param name="dictionary">セットする対象の辞書型配列</param>
	public override void SetProgressData(ref Dictionary<string, string> dictionary) {
		dictionary["option"] = ((int)ControllerA.Option.Bomb).ToString();
		dictionary["param"] = ScoreUIPushButtons.Score.ToString();
	}

	/// <summary>
	/// 画面に表示するミニゲーム結果をテキストで返します。
	/// </summary>
	/// <returns>ミニゲーム結果テキスト</returns>
	public override string GetResultText() {
		return "獲得スコア ＝ " + ScoreUIPushButtons.Score;
	}

	/// <summary>
	/// ランダムな２つのボタン＋スティック方向の設定
	/// 入力不要なダミーボタンの設定
	/// </summary>
	private void SetRandomKey() {
		// ゲームパッドの右手: 1~4 ボタンの中から選ぶ
		SubGamePushButtons.AvailableKeys[0] = (KeyCode)Random.Range((int)KeyCode.Joystick1Button0, (int)KeyCode.Joystick1Button4);

		// ゲームパッドのLR: 5~8 ボタンの中から選ぶ
		SubGamePushButtons.AvailableKeys[1] = (KeyCode)Random.Range((int)KeyCode.Joystick1Button4, (int)KeyCode.Joystick1Button7 + 1);

		// ゲームパッドの左手: スティックの方向を決める
		int i = Random.Range(0, SubGamePushButtons.StickPatternCount);
		switch(i) {
			case 0:
				this.axisCodeName = "Horizontal";
				this.axisDirection = 1;
				SubGamePushButtons.AxisName = "→";
				break;

			case 1:
				this.axisCodeName = "Horizontal";
				this.axisDirection = -1;
				SubGamePushButtons.AxisName = "←";
				break;

			case 2:
				this.axisCodeName = "Vertical";
				this.axisDirection = -1;
				SubGamePushButtons.AxisName = "↑";
				break;

			case 3:
				this.axisCodeName = "Vertical";
				this.axisDirection = 1;
				SubGamePushButtons.AxisName = "↓";
				break;
		}

		// 入力すべきボタン以外の、入力不要なダミーボタンを設定する
		// NOTE: 右手ボタンとLRボタンのみ
		for(int j = SubGamePushButtons.KeyCount; j < SubGamePushButtons.AvailableKeys.Length; j++) {
			// ランダム決定
			SubGamePushButtons.AvailableKeys[j] = (KeyCode)Random.Range((int)KeyCode.Joystick1Button0, (int)KeyCode.Joystick1Button7 + 1);

			// 入力すべきボタンとぶつかっていたらやり直し
			if(SubGamePushButtons.AvailableKeys[j] == SubGamePushButtons.AvailableKeys[0]
			|| SubGamePushButtons.AvailableKeys[j] == SubGamePushButtons.AvailableKeys[1]) {
				j--;
				continue;
			}
		}

		// 入力すべきボタンの表示
		Debug.Log(
			"左手＝" + AxisName + ", " +
			"右手＝" + ((int)SubGamePushButtons.AvailableKeys[0] - SubGamePushButtons.KeyCodeBase + 1) + ", " +
			"ＬＲ＝" + ((int)SubGamePushButtons.AvailableKeys[1] - SubGamePushButtons.KeyCodeBase + 1)
		);
	}

}
