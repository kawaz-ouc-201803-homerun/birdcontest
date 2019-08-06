using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 機体の着地判定を開始するトリガー
/// </summary>
public class LandingJudgeStarter : MonoBehaviour {

	/// <summary>
	/// 着地判定を行うコンポーネント
	/// </summary>
	public LandingJudge LandingJudgeComponent;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		this.LandingJudgeComponent.enabled = false;
	}

	/// <summary>
	/// トリガー対象が接したら着地判定を開始させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			this.LandingJudgeComponent.enabled = true;
		}
	}

}
