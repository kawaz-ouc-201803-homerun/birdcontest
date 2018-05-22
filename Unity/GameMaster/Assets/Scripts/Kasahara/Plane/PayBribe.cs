using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayBribe : MonoBehaviour {

	//賄賂カットインアニメーションを再生するスクリプト

	public GameObject panel;	//おじさんの画像が子になっているPanelを代入
	public Animator cutin;		//上と同じPanelを代入
	public float time;
	public float sumtime = 0;

	public static bool bribeOnSwitch = false;		//OnTriggerEnterの実行フラグをデフォルトでオフに（フラグの切り替えはDataContainerTestから）

	void OnTriggerEnter(Collider other){

		//フラグがオフなら何もしない
		if (bribeOnSwitch == false) {
			return;
		}

		if (other.gameObject.tag == "Trigger") {

			this.enabled = true;

			//カットイン演出の処理
			panel.SetActive (true);		//カットイン絵の表示
			Time.timeScale = 0;		//ゲーム画面のポーズ
			cutin.SetTrigger ("Start");		//アニメーション遷移

		}

	}

	void Update(){

		//カットイン演出の処理
		//アニメーションが終わったらTimeScaleを1に戻す
		if (sumtime >= 0 || sumtime <= 3.5f) {

			time = Time.unscaledDeltaTime;
			sumtime = sumtime + time;

		}

		if (sumtime >= 3.5f && bribeOnSwitch == true) {
			
			bribeOnSwitch = false;
			StopAnimation ();

		}

	}


	//アニメーション終了のための処理
	void StopAnimation(){

		panel.SetActive (false);		//カットイン絵をオフに
		Time.timeScale = 1;		//TimeScaleを1に

	}
}
