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
	/// ゲームオブジェクト：タイマーSE音源
	/// </summary>
	public GameObject GameObject_TimerSound;

	/// <summary>
	/// ゲームオブジェクト：タイマー終了SE音源
	/// </summary>
	public GameObject GameObject_TimerEndSound;

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
			// タイマー終了＆イベント発生
			this.StopTimer(true);
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
			} else {
				// 白色に
				this.GameObject_TimerFrame.GetComponent<UnityEngine.UI.Image>().color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);
				this.GameObject_TimerSeconds.GetComponent<UnityEngine.UI.Text>().fontSize = 64;
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
		}
		if(this.GameObject_TimerSeconds != null) {
			this.GameObject_TimerSeconds.GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
	}

	/// <summary>
	/// タイマーのカウントダウンを停止します。
	/// </summary>
	/// <param name="useCallback">終了時のコールバック関数を呼び出すかどうか</param>
	public void StopTimer(bool useCallback) {
		this.Counting = false;

		// 終了サウンド再生
		if(this.GameObject_TimerEndSound != null) {
			this.GameObject_TimerEndSound.GetComponent<AudioSource>().Play();
		}

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

		if(useCallback == true) {
			// 終了時のコールバックを呼び出す
			Debug.Log("タイマーがゼロになりました。");
			if(this.ZeroTimerEvent != null) {
				this.ZeroTimerEvent.Invoke();
			}
		}
	}

}
