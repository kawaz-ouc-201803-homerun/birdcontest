using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコア表示
/// </summary>
public class ShowScore : MonoBehaviour {

	/// <summary>
	/// ミニゲーム管理オブジェクト
	/// </summary>
	public SubGameButtonRepeat manager;

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	void Update() {
		GameObject.Find("ResultScore").GetComponent<UnityEngine.UI.Text>().text = "あなたのパワーは [" + this.manager.Score + "] だ！";
	}

}
