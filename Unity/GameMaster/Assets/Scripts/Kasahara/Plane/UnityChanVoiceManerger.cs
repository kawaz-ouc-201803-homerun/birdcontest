using System.Collections;
using UnityEngine;

/// <summary>
/// ユニティちゃんのボイス再生制御
/// ＊このスクリプトは飛行機に直接アタッチして下さい。
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
		TakingOff,      // 離陸時
	}

	/// <summary>
	/// 最初のPlaneに触れたときに離陸ボイスを再生します。
	/// </summary>
	/// <param name="other">触れたオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		// NOTE: Pedalタグ付きのPlaneは爆発の時には出現しないため、「爆発の衝撃によりパイロットが気絶する」設定を守れる
		if(other.gameObject.tag == "Pedal") {
			Debug.Log("ユニティちゃん離陸ボイストリガー発動");
			this.PlayVoice(UnityChanVoiceIndexes.TakingOff);
		}
	}

	/// <summary>
	/// ユニティちゃんボイスを再生します。
	/// </summary>
	/// <param name="audioClipIndex">再生するボイスのインデックス</param>
	public void PlayVoice(UnityChanVoiceIndexes audioClipIndex) {
		this.UnityChanAudioSource.PlayOneShot(this.UnityChanVoices[(int)audioClipIndex]);
	}

}
