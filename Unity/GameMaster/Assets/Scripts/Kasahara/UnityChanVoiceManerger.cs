using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanVoiceManerger : MonoBehaviour {
	//飛行機にアタッチ

	public AudioSource unitychanSource;		//Unityちゃんを代入
	public AudioClip voiceA;		//発進時のボイス
	public AudioClip voiceB;		//飛行中のボイス

	void Update(){
		if (Input.GetKeyDown (KeyCode.Return)) {
			unitychanSource.PlayOneShot (voiceA);
		}
	}

	void OnTriggerEnter (Collider other) {
		//Pedalタグ付きのPlaneは爆発の時には出現しないので、パイロット気絶設定を守れる
		if (other.gameObject.tag == "Pedal") {
			StartCoroutine ("PlayVoice");
		}
		
	}

	IEnumerator PlayVoice(){
		yield return new WaitForSecondsRealtime (3.5f);
		unitychanSource.PlayOneShot (voiceB);

	}
}
