using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push3KeysGame : MonoBehaviour {
	public static KeyCode[] key = new KeyCode[]{(KeyCode)97,(KeyCode)98,(KeyCode)99};

	// Use this for initialization
	void Start () {
		SetRandomKey();
		//打つべきキーの表示
		Debug.Log (key [0] + " "+ key [1] + " "+ key [2]);
	}
	
	// Update is called once per frame
	void Update (){
		if(Input.anyKey)
			Debug.Log ("get");
		if(Input.GetKey (key [0]))
			Debug.Log ("0");
		if(Input.GetKey (key [1]))
			Debug.Log ("1");
		if(Input.GetKey (key [2]))
			Debug.Log ("2");
		
		//入力受付しなくなる時がある
		if (Input.GetKey (key [0]) && Input.GetKey (key [1]) && Input.GetKey (key [2])) {
			PowerUI.power++;
			Debug.Log ("Score is " + PowerUI.power);
			SetRandomKey();
			//打つべきキーの表示
			Debug.Log (key [0] + " "+ key [1] + " "+ key [2]);
		}
	}

	//ランダムな３キーの設定
	void SetRandomKey(){
		while(true){
			for (int i = 0; i < 3; i++) {
				//KeyCode.A=97,KeyCode.Z=122
				key [i] = (KeyCode)Random.Range (97f, 122f);
			}
			//重複してないなら抜ける
			if (key [0] != key [1] && key [1] != key [2] && key [0] != key [2]) {
				break;
			}
		}
	}
}