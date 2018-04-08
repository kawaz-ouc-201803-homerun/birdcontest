using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
	public GameObject[] setsumei;	//説明図
	public GameObject YesNoButton;	//２回目押すボタン

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ButtonOnClick (string str){			
		switch (str) {
		//ボタン１回目
		case "Bomb":
			StartCoroutine(SetsumeiHyouji(0));			
			break;
		case "Coin":
			StartCoroutine(SetsumeiHyouji(1));
			break;
		//ボタン２回目
		case "Yes":
			break;
		case "No":
			int i;
			for(i = 0; i <= setsumei.Length; i++){
				StartCoroutine(SetsumeiHihyouji(i));
			}
			break;
		default:
			break;
		}
	}

	IEnumerator SetsumeiHyouji(int i){		//説明図をフェードイン
		yield return new WaitForSeconds(0.1f);
		for (float f = 0; f < 1; f += 0.1f) {
        	Color c = setsumei[i].GetComponent<Image>().color;
        	c.a = f;
        	setsumei[i].GetComponent<Image>().color = c;
			yield return new WaitForEndOfFrame();
		}
		setsumei[i].GetComponent<Image>().color = new Color(1,1,1,1);
		YesNoButton.SetActive(true);
	}
	IEnumerator SetsumeiHihyouji(int i){		//説明図をフェードアウト
		yield return new WaitForSeconds(0.1f);
		YesNoButton.SetActive(false);
		for (float f = 1; f > 0; f -= 0.1f) {
        	Color c = setsumei[i].GetComponent<Image>().color;
        	c.a = f;
        	setsumei[i].GetComponent<Image>().color = c;
			yield return new WaitForEndOfFrame();
		}
		setsumei[i].GetComponent<Image>().color = new Color(1,1,1,0);
	}

	float HokanKansu(float f){
		return -2*Mathf.Pow(f,3)+3*Mathf.Pow(f,2);
	}
}
