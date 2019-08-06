using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 場外着地判定
/// ＊このスクリプトは、飛行機に直接アタッチして下さい。
/// </summary>
public class JudgeOutOfGround : MonoBehaviour {

	/// <summary>
	/// 場内着地判定
	/// </summary>
	public LandingJudge LandingJudge;

	/// <summary>
	/// 西１丁目以東に着地したら強制終了させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "OutOfGround" && this.enabled == true) {
			Debug.Log("場外着地のため強制終了");
			this.enabled = false;

			// イベントを発生させる
			if(this.LandingJudge.LandingEvent != null) {
				this.LandingJudge.LandingEvent.Invoke();
			}
		}
	}

}
