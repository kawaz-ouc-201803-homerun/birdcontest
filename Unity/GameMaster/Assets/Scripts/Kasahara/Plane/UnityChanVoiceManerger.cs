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
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		// TODO: 現状、Enterキーで発射しているため、トリガーが変わったらここの条件も変える必要がある
		if(Input.GetKeyDown(KeyCode.Return) == true
		&& this.UnityChanAudioSource.isPlaying == false) {
			// 発進ボイス再生
			this.StartCoroutine(this.PlayVoice((int)UnityChanVoiceIndexes.Starting));
		}
	}

	/// <summary>
	/// 最初のPlaneに触れたときに離陸ボイスを再生します。
	/// </summary>
	/// <param name="other">触れたオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		// NOTE: Pedalタグ付きのPlaneは爆発の時には出現しないため、「爆発の衝撃によりパイロットが気絶する」設定を守れる
		if(other.gameObject.tag == "Pedal") {
			this.StartCoroutine(this.PlayVoice((int)UnityChanVoiceIndexes.TakingOff));
		}
	}

	/// <summary>
	/// ユニティちゃんボイスを再生します。
	/// </summary>
	/// <param name="audioClipIndex">再生するボイスのインデックス</param>
	public IEnumerator PlayVoice(int audioClipIndex) {
		yield return new WaitForSecondsRealtime(1f);
		this.UnityChanAudioSource.PlayOneShot(this.UnityChanVoices[audioClipIndex]);
	}

}
