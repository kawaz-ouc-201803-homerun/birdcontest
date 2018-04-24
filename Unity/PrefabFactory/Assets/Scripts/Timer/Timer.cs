using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 秒数カウントダウンタイマー制御
/// </summary>
public class Timer : MonoBehaviour {

	/// <summary>
	/// タイマー残り秒数
	/// 初期化時にタイマーの開始秒数に合わせられます。
	/// </summary>
	private float remainTimeSeconds;

	/// <summary>
	/// タイマー開始秒数
	/// 初期値はインスペクターで設定して下さい。
	/// </summary>
	public int TimeSeconds = 60;

	/// <summary>
	/// タイマーを注意レベルにする割合
	/// 初期値はインスペクターで設定して下さい。
	/// </summary>
	public float OrangeRate = 0.35f;

	/// <summary>
	/// タイマーを警告レベルにする割合
	/// 初期値はインスペクターで設定して下さい。
	/// </summary>
	public float RedRate = 0.25f;

	/// <summary>
	/// カウントダウン中であるかどうか
	/// </summary>
	public bool Counting;

	/// <summary>
	/// タイマーがゼロになったときに発動するイベント
	/// イベントハンドラーはインスペクターで設定して下さい。
	/// </summary>
	public UnityEvent ZeroTimerEvent;

	/// <summary>
	/// ゲームオブジェクト：タイマー音源
	/// </summary>
	public GameObject GameObject_TimerSound;

	/// <summary>
	/// ゲームオブジェクト：タイマーフレーム
	/// </summary>
	public GameObject GameObject_TimerFrame;

	/// <summary>
	/// ゲームオブジェクト：タイマー秒数
	/// </summary>
	public GameObject GameObject_TimerSeconds;

	/// <summary>
	/// 開始時の処理
	/// </summary>
	public void Start() {
		if(this.GameObject_TimerFrame != null) {
			this.GameObject_TimerFrame.GetComponent<Animator>().enabled = false;
		}
		if(this.GameObject_TimerSeconds != null) {
			this.GameObject_TimerSeconds.GetComponent<Animator>().enabled = false;
		}

		// デバッグ用
		this.StartTimer(this.TimeSeconds);
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		if(this.Counting == false) {
			return;
		}

		// タイマーカウント
		this.remainTimeSeconds -= Time.deltaTime;
		if(this.remainTimeSeconds <= 0) {
			// タイマー終了
			this.StopTimer();

			// タイマーゼロイベント発動
			if(this.ZeroTimerEvent != null) {
				this.ZeroTimerEvent.Invoke();
			}
		}
		if(this.GameObject_TimerSeconds != null) {
			// 画面上の秒数を更新
			this.GameObject_TimerSeconds.GetComponent<UnityEngine.UI.Text>().text = ((int)this.remainTimeSeconds).ToString();

			// 残り時間に応じた視覚的変化
			if(this.remainTimeSeconds < (int)(this.TimeSeconds * this.RedRate) + 1) {
				// 赤色に
				this.GameObject_TimerFrame.GetComponent<UnityEngine.UI.Image>().color = new Color(0xed / 255.0f, 0x6a / 255.0f, 0x5a / 255.0f);
				this.GameObject_TimerSeconds.GetComponent<UnityEngine.UI.Text>().fontSize = 96;
			} else if(this.remainTimeSeconds < (int)(this.TimeSeconds * this.OrangeRate) + 1) {
				// 橙色に
				this.GameObject_TimerFrame.GetComponent<UnityEngine.UI.Image>().color = new Color(252 / 255.0f, 235 / 255.0f, 151 / 255.0f);
				this.GameObject_TimerSeconds.GetComponent<UnityEngine.UI.Text>().fontSize = 74;
			}
			this.GameObject_TimerSeconds.GetComponent<UnityEngine.UI.Text>().color = this.GameObject_TimerFrame.GetComponent<UnityEngine.UI.Image>().color;
		}
	}

	/// <summary>
	/// タイマーのカウントダウンを開始します。
	/// </summary>
	/// <param name="timeSeconds">カウント秒数</param>
	public void StartTimer(int timeSeconds) {
		this.Counting = true;
		this.TimeSeconds = timeSeconds;
		this.remainTimeSeconds = this.TimeSeconds + 0.99f;

		// アニメーション開始
		if(this.GameObject_TimerSound != null) {
			this.GameObject_TimerSound.SetActive(true);
			this.GameObject_TimerSound.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.GameObject_TimerSound.GetComponent<AudioSource>().loop = true;
		}
		if(this.GameObject_TimerFrame != null) {
			this.GameObject_TimerFrame.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.GameObject_TimerFrame.GetComponent<Animator>().enabled = true;
		}
		if(this.GameObject_TimerSeconds != null) {
			this.GameObject_TimerSeconds.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.GameObject_TimerSeconds.GetComponent<Animator>().enabled = true;
		}
	}

	/// <summary>
	/// タイマーのカウントダウンを停止します。
	/// </summary>
	public void StopTimer() {
		this.Counting = false;

		// アニメーション停止
		if(this.GameObject_TimerSound != null) {
			this.GameObject_TimerSound.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
			this.GameObject_TimerSound.SetActive(false);
		}
		if(this.GameObject_TimerFrame != null) {
			this.GameObject_TimerFrame.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
		}
		if(this.GameObject_TimerSeconds != null) {
			this.GameObject_TimerSeconds.GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullCompletely;
		}
	}

	/// <summary>
	/// タイマーイベントテスト
	/// </summary>
	public void TimerZeroEventHandler() {
		Debug.Log("タイマーがゼロになりました。");
	}

}
