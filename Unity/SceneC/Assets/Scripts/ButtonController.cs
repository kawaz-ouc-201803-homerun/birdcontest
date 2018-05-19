using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

	static public int score = 0;    //スコア
	static public int isWindow = 0; //説明ウインドウを出すか否か（0:中立，1:出す，2:消す，3:永遠に消す）
	public GameObject[] setsumei;   //説明ウインドウ
	public GameObject YesNoButton;  //Yes/Noボタン

	public GameObject matometekesuObject;   //消すやつ

	public GameObject wairo;
	public GameObject wairoTextBox; //テキストボックス
	private Text wairoText; //テキストボックスのテキスト
	[Multiline(3)] public string[] wairoTexts;  //表示させるテキスト
	private Animator textAnim;  //テキストボックスのアニメータ
	bool isWairo = false;   //賄賂ミニゲームが始まっているか否か
	bool isTextable = false;    //テキストを送れるか否か
	public int quizNum = 0; //選択肢の番号（1,10,100）
	public int wairoFlug = 0;
	static public int wairoScore = 0;
	static public int wairoStep = 0;
	public GameObject organizer, sapphiartchan; //主催者，サファイアートちゃん
	private Sprite[] organizerSp;

	public GameObject sentakuButton;
	public Text[] sentakuText;


	void Start() {
		wairoText = wairoTextBox.GetComponent<Text>();

		wairoText.text = "";
		textAnim = wairoTextBox.GetComponent<Animator>();

		organizerSp = Resources.LoadAll<Sprite>("manager1");
		organizer.GetComponent<SpriteRenderer>().sprite = System.Array.Find<Sprite>(organizerSp, (sprite) => sprite.name.Equals("manager1_0"));
		sapphiartchan.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sapphiartchan_Nuetral")[0];

	}

	void Update() {
		//Debug.Log(organizer.transform.position.x);
		if(isWairo && quizNum == 0 && Input.GetMouseButtonDown(0)) {
			isTextable = true;
		}

	}


	public void ButtonOnClick(string str) {

		if(WindowController.isClickable) {
			isWindow = 1;

			switch(str) {
				//ボタン１回目
				case "Bomb":    //爆弾を押した
					score = 1;
					break;
				case "Coin":    //賄賂を押した
					score = 2;
					break;
				case "Kiai":    //気合いを押した
					score = 0;
					break;
				//ボタン２回目
				case "No":
					score = 0;
					isWindow = 2;
					foreach(GameObject obj in setsumei) {   //説明を消す
						StartCoroutine(TextHihyouji(obj));
					}
					break;
				case "Yes":
					isWindow = 3;
					foreach(GameObject obj in setsumei) {   //説明を消す
						StartCoroutine(TextHihyouji(obj));
					}
					StartCoroutine(RotationAnimation(matometekesuObject, 2f));
					if(score == 2) {    //賄賂ミニゲームへ
						isWairo = true;
						wairo.SetActive(true);
						StartCoroutine(RotationAnimation(wairo, 10f));
						StartCoroutine(TextWindowScaleAnimation(0.1f));
						StartCoroutine(Wairo());
					}
					break;
				default:
					break;
			}
		}
	}

	public void ButtonOnClickSentakushi(int i) {
		switch(i) {
			case 0:
				sentakuButton.SetActive(false);
				if(quizNum != 0) {
					wairoScore += 0;
					wairoFlug += 0;
					quizNum = 0;
					isTextable = true;
				}
				break;
			case 1:
				sentakuButton.SetActive(false);
				if(quizNum != 0) {
					wairoScore += 1;
					wairoFlug += quizNum;
					quizNum = 0;
					isTextable = true;
				}
				break;
		}
	}

	IEnumerator TextHihyouji(GameObject obj) {      //説明のテキストをフェードアウト
		yield return new WaitForSeconds(0.1f);
		YesNoButton.SetActive(false);
		for(float f = 1; f > 0; f -= 0.1f) {
			Color c = obj.GetComponent<Text>().color;
			c.a = f;
			obj.GetComponent<Text>().color = c;
			yield return new WaitForEndOfFrame();
		}
		obj.GetComponent<Text>().color = new Color(1, 1, 1, 0);
		obj.SetActive(false);
	}

	float HokanKansu(float f) { //補間函数（使ってない）
		return -2 * Mathf.Pow(f, 3) + 3 * Mathf.Pow(f, 2);
	}


	IEnumerator Wairo() {
		yield return new WaitForSeconds(1.0f);

		matometekesuObject.SetActive(false);

		StartCoroutine(TextOkuri(0));
		yield return new WaitUntil(() => isTextable);

		//主催者を水平移動
		while(organizer.transform.position.x > 4f) {
			organizer.transform.position += new Vector3(-0.6f, 0, 0);
			sapphiartchan.transform.position += new Vector3(0.6f, 0, 0);
			organizer.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.04f);
			sapphiartchan.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.02f);
			yield return new WaitForEndOfFrame();
		}
		organizer.transform.position = new Vector3(4f, 0, 0);
		sapphiartchan.transform.position = new Vector3(-4f, 0, 0);
		organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

		StartCoroutine(TextOkuri(1));
		yield return new WaitUntil(() => isTextable);

		organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
		sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
		StartCoroutine(TextOkuri(2));
		quizNum = 1;
		sentakuText[0].text = wairoTexts[21];
		sentakuText[1].text = wairoTexts[22];
		sentakuButton.SetActive(true);
		yield return new WaitUntil(() => isTextable);

		wairoStep += 1;

		switch(wairoFlug) {
			case 0:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				StartCoroutine(TextOkuri(3));
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(4));
				break;
			case 1:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				StartCoroutine(TextOkuri(5));
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(6));
				break;
			default:
				break;
		}

		yield return new WaitUntil(() => isTextable);

		StartCoroutine(TextOkuri(7));
		yield return new WaitUntil(() => isTextable);

		StartCoroutine(TextOkuri(8));
		quizNum = 10;
		sentakuText[0].text = wairoTexts[23];
		sentakuText[1].text = wairoTexts[24];
		sentakuButton.SetActive(true);
		yield return new WaitUntil(() => isTextable);

		wairoStep += 1;

		switch(wairoFlug / 10) {
			case 0:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				StartCoroutine(TextOkuri(9));
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(10));
				break;
			case 1:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				organizer.GetComponent<SpriteRenderer>().sprite = System.Array.Find<Sprite>(organizerSp, (sprite) => sprite.name.Equals("manager1_1"));
				textAnim.SetTrigger("isTremble");
				StartCoroutine(TextOkuri(11));
				yield return new WaitForSeconds(1f);
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(12));
				break;
			default:
				break;
		}

		yield return new WaitUntil(() => isTextable);

		StartCoroutine(TextOkuri(13));
		yield return new WaitUntil(() => isTextable);

		StartCoroutine(TextOkuri(14));
		quizNum = 100;
		sentakuText[0].text = wairoTexts[25];
		sentakuText[1].text = wairoTexts[26];
		sentakuButton.SetActive(true);
		yield return new WaitUntil(() => isTextable);

		wairoStep += 1;

		switch(wairoFlug / 100) {
			case 0:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				StartCoroutine(TextOkuri(15));
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(16));
				break;
			case 1:
				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				organizer.GetComponent<SpriteRenderer>().sprite = System.Array.Find<Sprite>(organizerSp, (sprite) => sprite.name.Equals("manager1_1"));
				textAnim.SetTrigger("isTremble");
				StartCoroutine(TextOkuri(17));
				yield return new WaitForSeconds(1f);
				yield return new WaitUntil(() => isTextable);

				organizer.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
				sapphiartchan.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
				StartCoroutine(TextOkuri(18));
				break;
			default:
				break;
		}
		yield return new WaitUntil(() => isTextable);

		wairoStep += 1;

		StartCoroutine(TextOkuri(19));
		sapphiartchan.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sapphiartchan_WinPose")[0];
		yield return new WaitUntil(() => isTextable);

		isWairo = false;
	}

	IEnumerator TextOkuri(int i) {
		isTextable = false;
		wairoText.text = wairoTexts[i];
		yield return new WaitForSeconds(1.0f);
	}

	IEnumerator RotationAnimation(GameObject obj, float speed) {
		float ang = 90;
		while(ang > 0 && ang < 180) {
			obj.transform.rotation = Quaternion.Euler(0, 0, ang);
			ang -= speed;
			yield return new WaitForEndOfFrame();
		}
		obj.transform.rotation = Quaternion.Euler(0, 0, 0);
	}
	IEnumerator TextWindowScaleAnimation(float speed) {
		while(wairo.transform.localScale.y < 1) {
			wairo.transform.localScale += new Vector3(0, speed, 0);
			yield return new WaitForEndOfFrame();
		}
		wairo.transform.localScale = new Vector3(1, 1, 0);
	}

}
