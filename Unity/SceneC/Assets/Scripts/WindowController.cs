using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowController : MonoBehaviour {

	public GameObject[] windowColumn;
	public GameObject window;
	public GameObject[] setsumei;   //説明文

	public Vector3 startPos, endPos;

	public GameObject YesNoButton;  //２回目押すボタン

	public static bool isClickable = true;


	void Start() {
		transform.localScale = new Vector3(1, 0, 0);
		window.transform.localScale = new Vector3(0, 1, 0);
		startPos = windowColumn[0].transform.position;
		//windowColumn[0] = transform.Find("ColumnL").gameObject;
		//windowColumn[1] = transform.Find("ColumnR").gameObject;
	}

	// Update is called once per frame
	void Update() {
		if(isClickable) {
			switch(ButtonController.isWindow) {
				case 1:
					isClickable = false;
					foreach(GameObject obj in windowColumn) {   //柱を出す
						obj.SetActive(true);
					}
					StartCoroutine(SetsumeiHyouji(ButtonController.score));
					break;
				case 2:
					isClickable = false;
					//StopCoroutine(SetsumeiHyouji(ButtonController.score));
					StartCoroutine(SetsumeiHihyouji(ButtonController.score));
					break;
				case 3:
					isClickable = false;
					//StopCoroutine(SetsumeiHyouji(ButtonController.score));
					StartCoroutine(SetsumeiHihyouji(ButtonController.score));
					break;
				default:
					break;
			}
		}
	}

	IEnumerator SetsumeiHyouji(int i) {
		/*	説明をフェードイン
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
		*/
		yield return new WaitForSeconds(0.1f);  //待つ

		while(transform.localScale.y < 1f) {
			transform.localScale += new Vector3(0, 0.2f, 0);
			yield return new WaitForEndOfFrame();
		}
		transform.localScale = new Vector3(1, 1, 0);

		yield return new WaitForSeconds(0.1f);  //待つ

		window.SetActive(true);

		while(windowColumn[0].transform.position.x > endPos.x) {
			windowColumn[0].transform.position += new Vector3(-70f, 0, 0);
			windowColumn[1].transform.position += new Vector3(70f, 0, 0);
			window.transform.localScale = new Vector3(-(windowColumn[0].transform.position.x - startPos.x) / (endPos.x - startPos.x), 1, 0);
			yield return new WaitForEndOfFrame();
		}

		setsumei[i].SetActive(true);
		while(setsumei[i].GetComponent<Text>().color.a < 1) {
			setsumei[i].GetComponent<Text>().color += new Color(0, 0, 0, 0.1f);
			yield return new WaitForEndOfFrame();
		}
		setsumei[i].GetComponent<Text>().color = new Color(1, 1, 1, 1);
		if(i == 0) {
			setsumei[3].SetActive(true);
			while(setsumei[3].GetComponent<Text>().color.a < 1) {
				setsumei[3].GetComponent<Text>().color += new Color(0, 0, 0, 0.5f);
				yield return new WaitForEndOfFrame();
			}
			setsumei[3].GetComponent<Text>().color = new Color(1, 1, 1, 1);
		}
		yield return new WaitForSeconds(0.1f);  //待つ

		YesNoButton.SetActive(true);

		ButtonController.isWindow = 0;

		yield return new WaitForSeconds(0.1f);  //待つ

		isClickable = true;

	}

	IEnumerator SetsumeiHihyouji(int i) {

		yield return new WaitForSeconds(0.1f);  //待つ

		YesNoButton.SetActive(false);

		while(windowColumn[0].transform.position.x < startPos.x) {
			windowColumn[0].transform.position += new Vector3(70f, 0, 0);
			windowColumn[1].transform.position += new Vector3(-70f, 0, 0);
			window.transform.localScale = new Vector3(-(windowColumn[0].transform.position.x - startPos.x) / (endPos.x - startPos.x), 1, 0);
			yield return new WaitForEndOfFrame();
		}

		window.SetActive(false);
		setsumei[i].SetActive(false);

		yield return new WaitForSeconds(0.1f);  //待つ

		while(transform.localScale.y > 0f) {
			transform.localScale -= new Vector3(0, 0.1f, 0);
			yield return new WaitForEndOfFrame();
		}

		foreach(GameObject obj in windowColumn) {   //柱を消す
			obj.SetActive(false);
		}

		ButtonController.isWindow = 0;

		yield return new WaitForSeconds(0.1f);  //待つ

		isClickable = true;
	}
}
