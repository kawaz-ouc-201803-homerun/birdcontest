using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

	static public int score = 0;	//スコア
	public GameObject[] setsumei;	//説明図
	public GameObject YesNoButton;	//２回目押すボタン
	public GameObject matometekesuObject;	//消す

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ButtonOnClick (string str){
		switch (str) {
		//ボタン１回目
		case "Bomb":	//爆弾を押した
			score += 1;
			StartCoroutine(SetsumeiHyouji(0));			
			break;
		case "Coin":	//賄賂を押した
			score += 2;
			StartCoroutine(SetsumeiHyouji(1));
			break;
		case "Kiai":	//気合いを押した
			score += 0;
			StartCoroutine(SetsumeiHyouji(2));
			break;
		//ボタン２回目
		case "Yes":
			StartCoroutine(SetsumeiHihyouji(matometekesuObject));
			break;
		case "No":
			score = 0;
			foreach(GameObject obj in setsumei){
				StartCoroutine(SetsumeiHihyouji(obj));
			}
			break;
		default:
			break;
		}
	}

	IEnumerator SetsumeiHyouji(int i){		//説明をフェードイン
		yield return new WaitForSeconds(0.1f);
		setsumei[i].SetActive(true);
		for (float f = 0; f < 1; f += 0.1f) {
        	Color c = setsumei[i].GetComponent<Image>().color;
        	c.a = f;
        	setsumei[i].GetComponent<Image>().color = c;
			yield return new WaitForEndOfFrame();
		}
		setsumei[i].GetComponent<Image>().color = new Color(1,1,1,1);
		yield return new WaitForSeconds(0.2f);
		YesNoButton.SetActive(true);
	}
	IEnumerator SetsumeiHihyouji(GameObject obj){		//説明をフェードアウト（どうして上と引数の型が違うの〜）
		yield return new WaitForSeconds(0.1f);
		YesNoButton.SetActive(false);
		for (float f = 1; f > 0; f -= 0.1f) {
        	Color c = obj.GetComponent<Image>().color;
        	c.a = f;
        	obj.GetComponent<Image>().color = c;
			yield return new WaitForEndOfFrame();
		}
		obj.GetComponent<Image>().color = new Color(1,1,1,0);
		obj.SetActive(false);
	}

	float HokanKansu(float f){	//補間函数（使ってない）
		return -2*Mathf.Pow(f,3)+3*Mathf.Pow(f,2);
	}
}
