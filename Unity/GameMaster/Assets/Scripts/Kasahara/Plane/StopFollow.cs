using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み訳の牽引車・クエリちゃんを飛行機から切り離します。
/// ＊最初の視点切替のPlaneにアタッチして下さい。
/// </summary>
public class StopFollow : MonoBehaviour {

	/// <summary>
	/// 牽引車
	/// </summary>
	public GameObject PullCar;

	/// <summary>
	/// 手押しする人
	/// </summary>
	public GameObject PushHuman;

	/// <summary>
	/// トリガー対象が接したら追従のコンポーネントを無効化します。
	/// 慣性が残っている場合はそのまま減衰しますが、その場で静止するわけではありません。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Car") {
			this.PullCar.GetComponent<FollowTarget>().enabled = false;
			this.PushHuman.GetComponent<FollowTarget>().enabled = false;
		}
	}

}
