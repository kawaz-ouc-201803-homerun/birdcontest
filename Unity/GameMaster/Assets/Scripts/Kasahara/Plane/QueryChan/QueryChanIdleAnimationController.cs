using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み役：爆発を見届けるクエリちゃんのアニメーション
/// ＊常時アイドル状態を維持します
/// </summary>
public class QueryChanIdleAnimationController : MonoBehaviour {

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start () {
		// アイドル状態のアニメーションを開始
		this.GetComponent<QueryAnimationController>().ChangeAnimation(QueryAnimationController.QueryChanAnimationType.IDLE);
	}

}
