using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushKeysGame : MonoBehaviour {
	private int power = 0;
	private KeyCode[] key= new KeyCode[]{(KeyCode)97,(KeyCode)98,(KeyCode)99};

	// Use this for initialization
	void Start () {
		//打つべきキーの表示
		Debug.Log("-----");
		Debug.Log(key[0]);
		Debug.Log(key[1]);
		Debug.Log(key[2]);
		Debug.Log("-----");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (key [0]) && Input.GetKey (key [1]) && Input.GetKey (key [2])) {
			Debug.Log ("OK");
			//3つのキー(ABC)を押すと、powerが+1される
			power++;
			Debug.Log ("Score is");
			Debug.Log (power);
			for (int i = 0; i < 3; i++) {
				//3キーをランダムに設定
				//KeyCode.A=97,KeyCode.Z=122
				key [i] = (KeyCode)Random.Range (97f, 122f);//DO : 重複回避処理まだ
			}
			//打つべきキーの表示
			Debug.Log ("-----");
			Debug.Log (key [0]);
			Debug.Log (key [1]);
			Debug.Log (key [2]);
			Debug.Log ("-----");

		}
	}
}