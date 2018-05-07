using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スタートボタン
/// </summary>
public class StartButton : MonoBehaviour {

	/// <summary>
	/// 開始時の画面
	/// </summary>
	public GameObject StartScreen;

	/// <summary>
	/// メイン画面
	/// </summary>
	public GameObject MainScreen;

	/// <summary>
	/// スタートボタンを押したときの画面遷移
	/// </summary>
	public void OnClick() {
		this.StartScreen.SetActive(false);
		this.MainScreen.SetActive(true);
	}

}
