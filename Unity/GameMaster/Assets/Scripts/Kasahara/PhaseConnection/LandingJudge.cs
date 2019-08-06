using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛行フェーズの終了判定
/// ＊離陸するまでこのコンポーネントはenabled=trueにしないで下さい。
/// </summary>
public class LandingJudge : PlaneBehaviourParent {

	/// <summary>
	/// 終了時のイベントハンドラー
	/// </summary>
	public UnityEngine.Events.UnityEvent LandingEvent;

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		if(this.PlaneRigidbody.IsSleeping() == true
		|| Input.GetKeyDown(KeyCode.F12) == true) {
			// 飛行機が静止したら、自動的に終了扱いにする
			// F12キーで飛行を強制終了できる
			Debug.Log("飛行終了");

			// 次のフレーム以降に続けて呼び出されないようにする
			this.enabled = false;

			// イベントを発生させる
			if(this.LandingEvent != null) {
				this.LandingEvent.Invoke();
			}
		}
	}
}
