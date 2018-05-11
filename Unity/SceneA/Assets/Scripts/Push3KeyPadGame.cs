using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push3KeyPadGame : MonoBehaviour {
	public static KeyCode[] key = new KeyCode[7];
	// Use this for initialization
	private string axiscodename;
	public static string axisname="tes";
	private float axispower;

	void Start () {
		SetRandomKey();
		//打つべきキーの表示
		Debug.Log (((int)key[0] - 350 + 1) + " "+ ((int)key[1] - 350 + 1) + " " + axisname);
	}

	// Update is called once per frame
	void Update (){
		//ダミーキーで連続入力対策
		if (Input.GetKey (key [3]) || Input.GetKey (key [4]) || Input.GetKey (key [5]) || Input.GetKey (key [6])) {
			for (int j = 0; j < 100; j++) {
				;
			}
		}
		//入力受付しなくなる時がある->ので入力をゲームパッドに変更
		if (Input.GetKey (key [0]) && Input.GetKey (key [1]) && Input.GetAxis(axiscodename)*axispower > 0.8f) {
			PowerUI.power++;
			SetRandomKey();
			//打つべきキーの表示
			Debug.Log (((int)key[0] - 350 + 1) + " "+ ((int)key[1] - 350 + 1) + " " + axisname);
		} 
	}

	//ランダムな2キー+スティックの設定、ダミーキーの設定
	void SetRandomKey(){
		//1~4ボタン
		key [0] = (KeyCode)Random.Range (350f, 354f);
		//5~8ボタン
		key [1] = (KeyCode)Random.Range (354f, 358f);
		int i = (int)Random.Range (0f, 4f);
		if (i == 0) {
			axiscodename = "Horizontal";
			axisname = "→";
			axispower = 1f;
		} else if (i == 1) {
			axiscodename = "Horizontal";
			axisname = "←";
			axispower = -1f;
		} else if (i == 2) {
			axiscodename = "Vertical";
			axisname = "↑";
			axispower = 1f;
		} else if (i == 3) {
			axiscodename = "Vertical";
			axisname = "↓";
			axispower = -1f;
		}
		for(int j=2;j<7;j++){
			key[j]=(KeyCode)Random.Range (354f, 358f);
			if (key [j] == key [0] || key [j] == key [1]){
				j--;
			}
		}
	}
}