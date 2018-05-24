using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み役：人力手押しのアニメーション（クエリちゃん）
/// </summary>
public class QueryChanAnimationController : MonoBehaviour {

	/// <summary>
	/// クエリちゃんのAnimatorコンポーネント
	/// </summary>
	public Animator QueryChanAnimator;

	/// <summary>
	/// アニメーション速度
	/// </summary>
	private float speed;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		//飛行機を押すアニメーションの速度を最初は遅くしておく
		this.speed = 0.01f;
		this.QueryChanAnimator.speed = this.speed;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		//アニメーションのスピードをだんだん上げていく
		if(this.speed <= 8) {
			this.speed = this.speed + 2.5f * Time.deltaTime;
			this.QueryChanAnimator.speed = this.speed;
		}
	}

}
