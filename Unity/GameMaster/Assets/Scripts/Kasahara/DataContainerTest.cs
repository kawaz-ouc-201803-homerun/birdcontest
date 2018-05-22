using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class DataContainerTest : MonoBehaviour {
	public GameObject Master;		//3DObjects_Kasaharaを代入

	public int optionA;
	public int paramA;

	public int paramB;

	public int optionC;
	public int paramC;

	public GameObject counter;		//DistanceCounterがアタッチされているGameObjectを代入
	public GameObject firstbomb;		//FirstBombを代入
	public GameObject car;		//牽引する車を代入
	public GameObject querychan;	//クエリちゃんを代入
	public GameObject pedalplane;		//ペダルタグが付いているPlaneを代入

	public GameObject secondbomb;		//SecondBombを代入

	void Start () {

		//端末Aからのデータを処理
		switch(optionA){

		case 0:	//爆発
			//飛行機の位置を変更
			this.transform.position = new Vector3 (-137, 93, -69);

			//爆弾をオン
			firstbomb.SetActive (true);

			//paramAを爆発で飛行機を飛ばすときの力に設定
			firstbomb.GetComponent<ExplosionForce> ().explosionForce = paramA;

			break;

		case 1:	//牽引
			Debug.Log ("車");
			//飛行機の位置を変更
			this.transform.position = new Vector3 (-77, 96, -60);

			//車をオン
			car.SetActive(true);

			//Pedalタグが付いているPlaneをオンに
			pedalplane.SetActive(true);

			//paramAを車の牽引力に設定
			this.GetComponent<AutoMove> ().movepower = paramA;

			break;

		case 2:	//人力
			Debug.Log("人");
			//飛行機の位置を変更
			this.transform.position = new Vector3 (-77, 96, -60);

			//クエリちゃんをオンに
			querychan.SetActive(true);

			//Pedalタグが付いてるPlaneをオンに
			pedalplane.SetActive(true);

			//paramAを飛行機を押す力に設定
			this.GetComponent<AutoMove> ().movepower = paramA;


			break;

		default:
			Debug.Log("error:0～3の間で入力してください");
			break;
		}


		//端末BからのデータをPompPedalに送信
		this.GetComponent<PompPedal>().pedalpower = paramB;


		//端末Cからのデータを処理
		switch(optionC){
		case 0:	//爆発
			//二個目の爆弾をオンに
			this.GetComponent<SecondExplosion> ().enabled = true;

			//SecondExplosionのフラグをオンに	
			SecondExplosion.enabledOnTrigegrEnter = true;

			//AもCも爆弾だった場合、飛行機を墜落させるため、爆発の威力を落とす（ダミーを出現させて墜落したように見せるため）
			if (optionA == 0){
				paramC = 0;
			}

			//paramCを、SeondBombの爆発力に設定
			secondbomb.GetComponent<ExplosionPhysicsForce>().explosionForce = paramC;

			break;

		case 1:	//応援
			//Cheerupスクリプトのフラグをオンに
			Cheerup.cheerupOnSwitch = true;

			//paramCをupperに代入
			this.GetComponent<Cheerup>().upper = paramC;

			break;

		case 2:	//賄賂
			//PayBribeスクリプトのフラグをオンに
			PayBribe.bribeOnSwitch = true;

			break;

		default:
			Debug.Log("error:0～3の間で入力してください");
			break;

		}

			//飛距離測定スクリプトの初期設定（スタート時のpositionを原点とする処理）
		counter.GetComponent<DistanceCounter> ().startpositionX = this.transform.position.x;
		counter.GetComponent<DistanceCounter> ().startpositionZ = this.transform.position.z;

	}

	void Update () {
		
		//OptionAで使うスクリプトをEnterキーで制御する処理
		switch (optionA) {
		case 1:
		case 2:
			
			if (Input.GetKeyDown (KeyCode.Return)) {
				
				this.GetComponent<AutoMove> ().enabled = true;		//自動牽引スクリプトをオンに
				querychan.GetComponent<QueryChanAnimationControll> ().enabled = true;		//クエリちゃんのアニメーションを再生
				querychan.GetComponent<QueryChanAnimationControll> ().pushplane.SetTrigger("Start");		//クエリちゃんが飛行機を押すアニメーションを再生

			}
			break;	
		}

		//Enterキーを押したと同時に終了判定するスクリプトを起動する
		if (Input.GetKeyDown (KeyCode.Return)) {
			Master.GetComponent<LandingJudge> ().enabled = true;
		}
			

	}
		
}
