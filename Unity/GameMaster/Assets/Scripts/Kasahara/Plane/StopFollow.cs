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
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Car") {
			Debug.Log("仕込み役：追従停止");
			
			// 追従を停止してその場に静止させる
			this.PullCar.GetComponent<FollowTarget>().enabled = false;
			this.PullCar.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.PullCar.GetComponent<Rigidbody>().isKinematic = true;

			this.PushHuman.GetComponent<FollowTarget>().enabled = false;
			this.PushHuman.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.PushHuman.GetComponent<Rigidbody>().isKinematic = true;
		}
	}

}
