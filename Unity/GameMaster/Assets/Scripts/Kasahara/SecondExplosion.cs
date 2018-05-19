using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondExplosion : ParentClass {
	
	public GameObject secoundbomb;		//インスペクター上でSecoundBombを代入

	public static bool enabledOnTrigegrEnter = false;		//OnTriggerEnterの実行のフラグをデフォルトではオフに

	void OnTriggerEnter (Collider other) {

		//フラグがオフなら何もしない
		if (enabledOnTrigegrEnter == false) {
			
			return;

		}

		//TrigegrタグがついているPlaneに触れるとフラグをオンに
		if (other.gameObject.tag == "Trigger") {
			
			secoundbomb.transform.position = plane.transform.position;		//SecoundBombの位置をPlaneに合わせる

			secoundbomb.SetActive (true);

		}
	}
}