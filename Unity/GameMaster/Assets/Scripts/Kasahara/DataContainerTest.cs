using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class DataContainerTest : MonoBehaviour {
	public int optionA;
	public int paramA;

	public int paramB;

	public int optionC;
	public int paramC;

	public GameObject counter;		//DistanceCounterがアタッチされているGameObjectを代入
	public GameObject bomb;		//FirstBombを代入
	public GameObject car;		//牽引する車を代入
	public GameObject querychan;	//クエリちゃんを代入

	void Start () {

		//端末Aからのデータを処理
		switch(optionA){

		case 0:
			//爆発するときは搭乗者が気絶してるので、ペダルをこぐスクリプトをオフにする
			this.GetComponent<PompPedal> ().enabled = false;

			//車をオフ
			car.SetActive(false);

			//自動牽引スクリプトをオフ
			this.GetComponent<AutoMove>().enabled = false;

			//爆弾をオン
			bomb.SetActive (true);

			//飛行機の位置を変更
			this.transform.position = new Vector3 (-137, 93, -69);

			//paramAを爆発で飛行機を飛ばすときの力に設定
			bomb.GetComponent<ExplosionForce> ().explosionForce = paramA;
			break;

		case 1:
			Debug.Log ("車");

			//爆弾の初期化
			bomb.SetActive(false);

			//飛行機の位置を変更
			this.transform.position = new Vector3 (-77, 96, -60);

			//paramAを車の牽引力に設定
			this.GetComponent<AutoMove> ().movepower = paramA;

			break;

		case 2:
			Debug.Log("人");

			//爆弾の初期化
			bomb.SetActive(false);

			//車の初期化
			car.SetActive(false);

			//paramAを飛行機を押す力に設定
			this.GetComponent<AutoMove> ().movepower = paramA;

			this.transform.position = new Vector3 (-77, 96, -60);

			break;

		default:
			Debug.Log("error:0～3の間で入力してください");
			break;
		}

		//端末BからのデータをPompPedalに送信
		this.GetComponent<PompPedal>().pedalpower = paramB;

		//端末Cからのデータを処理
		switch(optionC){
		case 0:
			this.GetComponent<SecondExplosion> ().enabled = true;
			SecondExplosion.enabledOnTrigegrEnter = true;

			break;

		case 1:
			Debug.Log ("応援");
			break;

		case 2:
			Debug.Log("贈賄");
			break;

		default:
			Debug.Log("error:0～3の間で入力してください");
			break;

		}
			
		counter.GetComponent<DistanceCounter> ().startpositionX = this.transform.position.x;
		counter.GetComponent<DistanceCounter> ().startpositionZ = this.transform.position.z;

	}
	
	// Update is called once per frame
	void Update () {
		
		switch (optionA) {
		case 1:
		case 2:
			
			if (Input.GetKeyDown (KeyCode.Return)) {
				
				this.GetComponent<AutoMove> ().enabled = true;
				querychan.GetComponent<QueryChanAnimationControll> ().enabled = true;
				querychan.GetComponent<QueryChanAnimationControll> ().pushplane.SetTrigger("Start");

			}
			break;	
		}
			

	}
		
}
