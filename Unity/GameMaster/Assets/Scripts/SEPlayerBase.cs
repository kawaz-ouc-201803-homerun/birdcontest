using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE再生制御の基底クラス
/// </summary>
public abstract class SEPlayerBase : MonoBehaviour {

	/// <summary>
	/// SE音源
	/// </summary>
	public AudioSource SEAudioSource;

	/// <summary>
	/// SEのサウンドファイル
	/// </summary>
	public AudioClip[] SEList;

	/// <summary>
	/// SEを通常再生します。
	/// </summary>
	/// <param name="SEID">SEのID</param>
	public virtual void PlaySE(int SEID) {
		this.SEAudioSource.PlayOneShot(this.SEList[SEID]);
	}

}
