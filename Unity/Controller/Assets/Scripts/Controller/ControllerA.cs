﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末A の管理クラス
/// </summary>
public class ControllerA : ControllerBase {

	/// <summary>
	/// 選択肢
	/// </summary>
	public enum Option {
		Car,		// 自動車による牽引
		Human,		// 人力手押し
		Bomb,		// 爆弾
	}

	/// <summary>
	/// 開始準備完了したかどうか
	/// </summary>
	private bool isReadyForStart = false;

	/// <summary>
	/// 初回処理
	/// </summary>
	public override void Start() {
		base.Start();
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		base.Update();

		if(this.isReadyForStart == true && this.doneReadyGo == false) {
			// Ready-Go 表示してミニゲーム開始へ
			if(this.activeSubGame != null) {
				this.activeSubGame.gameObject.SetActive(true);
			}
			this.doReadyGo();
		}
	}

	/// <summary>
	/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
	/// </summary>
	public override void StartNewGame() {
		// 画面初期化
		this.transform.Find("Options").gameObject.SetActive(true);
		foreach(var subGame in this.SubGames) {
			subGame.gameObject.SetActive(false);
		}
		this.Start();
		this.isReadyForStart = false;
	}

	/// <summary>
	/// 「メータータイミング押し」を開始
	/// </summary>
	public void StartSubGame_MeterStop() {
		this.transform.Find("Options").gameObject.SetActive(false);
		this.activeSubGame = this.SubGames[(int)Option.Car];
		this.activeSubGame.Start();
		this.isReadyForStart = true;
	}

	/// <summary>
	/// 「ボタン連打」を開始
	/// </summary>
	public void StartSubGame_ButtonRepeat() {
		this.transform.Find("Options").gameObject.SetActive(false);
		this.activeSubGame = this.SubGames[(int)Option.Human];
		this.activeSubGame.Start();
		this.isReadyForStart = true;
	}

	/// <summary>
	/// 「３ボタン連続押し」を開始
	/// </summary>
	public void StartSubGame_PushButtons() {
		this.transform.Find("Options").gameObject.SetActive(false);
		this.activeSubGame = this.SubGames[(int)Option.Bomb];
		this.activeSubGame.Start();
		this.isReadyForStart = true;
	}

	/// <summary>
	/// Ready-Go完了後にゲームを開始させます。
	/// </summary>
	protected override void AfterHideGo() {
		base.AfterHideGo();
		this.activeSubGame.IsUpdateEnabled = true;
	}

}
