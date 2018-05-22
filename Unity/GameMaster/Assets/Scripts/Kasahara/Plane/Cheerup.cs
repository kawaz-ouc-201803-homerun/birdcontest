using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheerup : MonoBehaviour {

	//応援の処理と、カットインアニメーションを再生するためのスクリプト

	public Rigidbody plane;

	//カットイン演出のための変数
	public GameObject panel;	//サファイアートちゃんの画像が子になっているPanelを代入
	public Animator cutin;		//上と同じPanelを代入
	public float time;
	public float sumtime = 0;

	public float upper;		//paramCから代入

	public static bool cheerupOnSwitch = false;		//OnTriggerEnterの実行フラグをデフォルトでオフに（フラグの切り替えはDataContainerTestから）


	//TriggerタグがついているPlaneに触ったらこのスクリプトをオンに
	void OnTriggerEnter(Collider other){

		//フラグがオフなら何もしない
		if (cheerupOnSwitch == false) {
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
		
	void FixedUpdate () {

		if (upper > 0) {
			upper = upper - 1f * Time.deltaTime;		//飛ぶ力を1秒ずつ減衰
			plane.AddForce (transform.up * upper, ForceMode.VelocityChange);
		}

	}

	void Update(){

		//カットイン演出の処理
		//アニメーションが終わったらTimeScaleを1に戻す
		if (sumtime >= 0 || sumtime <= 3.5f) {

			time = Time.unscaledDeltaTime;
			sumtime = sumtime + time;

		}

		if (sumtime >= 3.5f && cheerupOnSwitch == true) {
			cheerupOnSwitch = false;
			StopAnimation ();

		}

	}
		

	//アニメーション終了のための処理
	void StopAnimation(){

		panel.SetActive (false);		//カットイン絵をオフに
		Time.timeScale = 1;		//TimeScaleを1に

	}

}
