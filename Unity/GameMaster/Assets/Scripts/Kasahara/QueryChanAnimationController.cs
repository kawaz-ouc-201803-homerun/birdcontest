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
	/// アニメーション開始
	/// ＊AutoMoveのStartMoveEventにセットして下さい。
	/// </summary>
	public void StartAnimation() {
		// クエリちゃんのアニメーションを開始
		this.enabled = true;
		this.QueryChanAnimator.SetTrigger("Start");

		// 飛行機を押すアニメーションの速度を最初は遅くしておく
		this.speed = 3.0f;
		this.QueryChanAnimator.speed = this.speed;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		// アニメーションのスピードをだんだん上げていく
		if(this.speed <= 8) {
			this.speed = this.speed + 2.5f * Time.deltaTime;
			this.QueryChanAnimator.speed = this.speed;
		}
	}

}
