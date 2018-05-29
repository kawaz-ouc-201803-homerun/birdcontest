using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Accelerator : MonoBehaviour {

	/// <summary>
	/// タイヤが空転する回転数の閾値
	/// </summary>
	public const float RacingThreshold = 0.65f;

	/// <summary>
	/// タイヤが空転する時間秒数
	/// </summary>
	public const float RacingTimeSeconds = 1.5f;

	/// <summary>
	/// タイヤが空転しているときのエンジン回転数の減衰量
	/// </summary>
	public const float RacingDecelFactor = 0.05f;

	/// <summary>
	/// クルマの操作状態
	/// </summary>
	public enum State {
		BeforeStart,    // 開始前のアクセル吹かし
		Racing,         // 発進に失敗してタイヤ空転中
		Ready,          // 発進準備完了
		Starting,		// 発進直後に牽引している飛行機の負荷がかかり回転数が落ちる状態
		Running,        // 発進
		Braking,        // ブレーキ
	}

	/// <summary>
	/// １～４キー操作でデモ操作を行えるようにするかどうか
	/// </summary>
	public bool DemoMode = true;

	/// <summary>
	/// エンジン回転数 0~1
	/// </summary>
	public float EngineRevs;

	/// <summary>
	/// クラッチを切った状態で上げるエンジン回転数の目標値
	/// </summary>
	public float TargetRevs = 0.5f;

	/// <summary>
	/// クラッチを切った状態でアクセルを踏んだときのエンジン回転数の加算量
	/// </summary>
	public const float DetachedClutchAccelFactor = 0.01f;

	/// <summary>
	/// 走行中にアクセルを踏んだときのエンジン回転数の加算量
	/// </summary>
	public const float RunningAccelFactor = 0.002f;

	/// <summary>
	/// 走行中にアクセルを離したときのエンジン回転数の減衰量
	/// </summary>
	public const float RunningDecelFactor = 0.01f;

	/// <summary>
	/// 発進直後に牽引している飛行機の負荷がかかって落ちる回転数差分値
	/// </summary>
	public const float StatringDownRevFactor = 0.1f;

	/// <summary>
	/// 現在の操作状態
	/// </summary>
	public State CurrentState;

	/// <summary>
	/// エンジン音の管理オブジェクト
	/// </summary>
	private CarAudio engineAudio;

	/// <summary>
	/// 空転タイマー
	/// </summary>
	private float racingTimer;

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public void Start() {
		this.engineAudio = this.GetComponent<CarAudio>();
		this.CurrentState = State.BeforeStart;
	}

	/// <summary>
	/// 毎フレーム更新
	/// </summary>
	public void Update() {
		if(this.CurrentState == State.Racing) {
			// 空転処理
			this.racingTimer -= Time.deltaTime;

			// 回転数を自動で落とす
			this.EngineRevs -= Accelerator.RacingDecelFactor;
			if(this.EngineRevs < 0) {
				this.EngineRevs = 0;
			}

			if(this.racingTimer < 0) {
				// タイマー終わったら空転解除
				this.GetComponent<AudioSource>().Stop();
				this.CurrentState = State.Running;
			} else {
				return;
			}
		}

		if(this.CurrentState == State.BeforeStart && this.EngineRevs < this.TargetRevs) {
			// クラッチを切っているとき、自動的に目標の回転数まで上げる
			this.ReadyForStart();
		}

		if(this.CurrentState == State.Ready) {
			// 発進するとき、飛行機の重みで回転数が一瞬落ちる演出を行う
			this.CurrentState = State.Starting;
			this.StartCoroutine(this.startingDownEngine());
		}

		// デモンストレーションモード
		if(this.DemoMode == true) {
			// 1キーでクラッチ切る（初期状態へ）
			if(Input.GetKey(KeyCode.Alpha1) == true) {
				this.CurrentState = State.BeforeStart;
			}

			// 2キーでクラッチ繋ぐ（発進）
			if(Input.GetKey(KeyCode.Alpha2) == true && this.CurrentState == State.BeforeStart) {
				this.Run();
			}

			// クラッチ繋いでいるとき
			if(this.CurrentState != State.BeforeStart) {
				// 3キーでアクセル吹かす
				if(Input.GetKey(KeyCode.Alpha3) == true && this.CurrentState == State.Running) {
					this.AccelUpDown(false);
				} else {
					this.AccelUpDown(true);
				}

				// 4キーでブレーキ（スリップ音出す）
				if(Input.GetKey(KeyCode.Alpha4) == true && this.CurrentState == State.Running) {
					this.Brake();
				}
			}
		}

		this.EngineRevs = this.engineAudio.Revs;
	}

	/// <summary>
	/// 発進前のアクセル吹かしを行います。
	/// </summary>
	public void ReadyForStart() {
		// クラッチを切る
		this.CurrentState = State.BeforeStart;

		this.engineAudio.Revs += Accelerator.DetachedClutchAccelFactor;
		if(this.engineAudio.Revs > 1) {
			this.engineAudio.Revs = 1;
		}

		if(this.engineAudio.Revs > this.TargetRevs) {
			this.engineAudio.Revs = this.TargetRevs;
		}
	}

	/// <summary>
	/// クラッチを繋いで発進します。
	/// このとき、一定の回転数を超えていたらタイヤが空転します。
	/// </summary>
	public void Run() {
		this.CurrentState = State.Running;

		// 一定の回転数以上のとき、空転させる
		if(this.EngineRevs > Accelerator.RacingThreshold) {
			this.CurrentState = State.Racing;
			this.racingTimer = Accelerator.RacingTimeSeconds;
			this.engineAudio.Revs = 0;
			this.GetComponent<AudioSource>().Play();
			return;
		}
	}

	/// <summary>
	/// 発進するとき、飛行機の重みで回転数が一瞬落ちる演出を行います。
	/// </summary>
	private IEnumerator startingDownEngine() {
		iTween.ValueTo(
			this.gameObject,
			iTween.Hash(
				"from", this.EngineRevs,
				"to", this.EngineRevs - Accelerator.StatringDownRevFactor,
				"time", 0.5f,
				"onupdate", new Action<object>((value) => {
					this.engineAudio.Revs = (float)value;
				})
			)
		);

		yield return new WaitForSeconds(0.5f);

		// 回転数を上げていくフェーズへ移行する
		this.CurrentState = State.Running;
	}

	/// <summary>
	/// 走行中にアクセルを踏んだり離したりします。
	/// </summary>
	/// <param name="leave">アクセルを離しているかどうか</param>
	public void AccelUpDown(bool leave = false) {
		if(leave == false) {
			// 加速
			this.CurrentState = State.Running;

			this.engineAudio.Revs += Accelerator.RunningAccelFactor;
			if(this.engineAudio.Revs > 1) {
				this.engineAudio.Revs = 1;
			}
		} else {
			// 減速
			this.engineAudio.Revs -= Accelerator.RunningDecelFactor;
			if(this.engineAudio.Revs < 0) {
				this.engineAudio.Revs = 0;

				// 回転数ゼロになったらブレーキ解除
				this.GetComponent<AudioSource>().Stop();
				this.CurrentState = State.Running;
			}
		}
	}

	/// <summary>
	/// ブレーキで停止します。
	/// </summary>
	public void Brake() {
		if(this.engineAudio.Revs > 0 && this.CurrentState != State.Braking) {
			this.CurrentState = State.Braking;
			this.GetComponent<AudioSource>().Play();
		}
	}

}
