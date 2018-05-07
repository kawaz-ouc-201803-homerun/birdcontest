using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ローカルテスト用の画面遷移ボタン
/// </summary>
public class TestButton : MonoBehaviour {

	/// <summary>
	/// メイン画面
	/// </summary>
	public GameObject MainScreen;

	/// <summary>
	/// 終了時の画面
	/// </summary>
	public GameObject EndScreen;

	/// <summary>
	/// ENDボタンを押したときの画面遷移
	/// </summary>
	public void OnClick () {
		this.MainScreen.SetActive(false);
		this.EndScreen.SetActive(true);
	}
	
}
