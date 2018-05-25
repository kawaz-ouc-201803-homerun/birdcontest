using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飛行フェーズの終了判定
/// </summary>
public class LandingJudge : MonoBehaviour {

	// 終了時のイベントハンドラー
	public UnityEngine.Events.UnityEvent LandingEvent;

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		// TODO: 飛行機が静止してから一定時間が経過したら、自動的に終了扱いにする
		// TODO: 高さが一定以上の場合、木に引っかかっているので振動させる？
		// TODO: 賄賂のとき、ParamCの％倍率に応じて点数を操作する
		// TODO: 地面に着地したとき、飛距離の計測を停止させる

		if(Input.GetKeyDown(KeyCode.Escape) == true) {
			// Escapeキーで飛行を強制終了
			if(this.LandingEvent != null) {
				this.LandingEvent.Invoke();
			}
		}
	}

}
