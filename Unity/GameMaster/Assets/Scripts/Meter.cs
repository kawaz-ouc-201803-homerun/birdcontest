using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Slider製のメーターを制御します。
/// このスクリプトはiTweenが必須です。
/// </summary>
public class Meter : MonoBehaviour {

	/// <summary>
	/// メーターを滑らかに変化させるのに要する秒数
	/// </summary>
	private const float ChangeEaseSeconds = 1.0f;
	
	/// <summary>
	/// 内部的に紐づけるスライダーコンポーネント
	/// </summary>
	private Slider component_Slider;

	/// <summary>
	/// メーターの値。０～１までの値を取ります。
	/// </summary>
	public float Value {
		get;
		protected set;
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public void Start() {
		this.component_Slider = this.transform.Find("Meter").GetComponent<Slider>();
		this.component_Slider.value = this.Value;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
	}

	/// <summary>
	/// メーターの値を動的に変更します。
	/// 内部の値は即座に変更されますが、画面上のメーターは滑らかに変化します。
	/// </summary>
	/// <param name="value">変更後の値</param>
	public void SetValue(float value) {
		var beforeValue = this.Value;
		this.Value = value;

		if(this.component_Slider == null) {
			Debug.LogWarning("メーターに紐づけるゲームオブジェクトが設定されていません。");
			return;
		}

		// メーターを滑らかに変化させる
		iTween.ValueTo(
			this.gameObject,
			iTween.Hash(
				"from", beforeValue,
				"to", value,
				"time", Meter.ChangeEaseSeconds,
				"easeType", iTween.EaseType.easeInOutQuad,
				"onupdate", new Action<object>((newValue) => {
					this.component_Slider.value = (float)newValue;
				})
			)
		);
	}

}
