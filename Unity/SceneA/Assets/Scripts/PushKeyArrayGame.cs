using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushKeyArrayGame : MonoBehaviour {
	//public static KeyCode[] key = new KeyCode[]{(KeyCode)97,(KeyCode)98,(KeyCode)99};
	public static KeyCode[] keys = new KeyCode[10];
	public static int length = 1;
	public static int key_num = 0;
	// Use this for initialization
	void Start () {
		key_num = 0;
		SetRandomArrayKey();
		//打つべきキーの表示
		for (int i = 0; i < length; i++)
			Debug.Log (keys [i]);
	}

	// Update is called once per frame
	void Update (){
		if (Input.GetKey (keys [key_num])) {
			PowerUI.power++;
			key_num++;
			Debug.Log ("Score is " + PowerUI.power);
			if(key_num == length){
				if(length < 10)
					length++;
			 	SetRandomArrayKey ();
				key_num = 0;
			}
		}
	}

	//ランダムな配列キーの設定
	void SetRandomArrayKey(){
		for (int i = 0; i < length; i++) {
			//KeyCode.A=97,KeyCode.Z=122
			keys [i] = (KeyCode)Random.Range (97f, 122f);
		}
	}
}