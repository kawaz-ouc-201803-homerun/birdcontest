using System.Collections;
using UnityEngine;

/// <summary>
/// ユニティちゃんのボイス再生制御
/// </summary>
public class UnityChanVoiceManerger : MonoBehaviour {

	/// <summary>
	/// ユニティちゃん音源
	/// </summary>
	public AudioSource UnityChanAudioSource;

	/// <summary>
	/// ユニティちゃんボイス
	/// </summary>
	public AudioClip[] UnityChanVoices;

	/// <summary>
	/// ユニティちゃんボイスの配列インデックス
	/// </summary>
	public enum UnityChanVoiceIndexes {
		Starting,       // 発進時
		Flying,      // 飛んでいるとき
	}

	/// <summary>
	/// ユニティちゃんボイスを再生します。
	/// </summary>
	/// <param name="audioClipIndex">再生するボイスのインデックス</param>
	public void PlayVoice(UnityChanVoiceIndexes audioClipIndex) {
		this.UnityChanAudioSource.PlayOneShot(this.UnityChanVoices[(int)audioClipIndex]);
	}

}
