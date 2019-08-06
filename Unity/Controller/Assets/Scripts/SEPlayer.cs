using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE再生制御を行います。
/// </summary>
public class SEPlayer : MonoBehaviour {

	/// <summary>
	/// SE音源
	/// </summary>
	public AudioSource SEAudioSource;

	/// <summary>
	/// SEのサウンドファイル
	/// </summary>
	public AudioClip[] SEList;

	/// <summary>
	/// SEのID
	/// </summary>
	public enum SEID {
		Decision,			// 決定音
		Cancel,				// キャンセル音
		ControllerStart,	// コントローラー開始音
		PushButton,			// ボタン押下音
	}

	/// <summary>
	/// SEを通常再生します。
	/// </summary>
	/// <param name="SEID">SEのID</param>
	public virtual void PlaySE(int SEID) {
		this.SEAudioSource.PlayOneShot(this.SEList[SEID]);
	}

}
