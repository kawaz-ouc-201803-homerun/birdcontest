using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainB : MonoBehaviour {

	public bool flag;

	public int count;

	public float score;

	public float flame;

	public float sumscore;

	public int bcount;

	// Use this for initialization
	void Start () {
		Debug.Log ("MainB");
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Click")){
			this.flag = true;
		}
		if (Input.GetButtonUp("Click") && this.flag==true) {
			this.count++;
			this.flag = false;
		}
		this.flame+=Time.deltaTime;
		if (flame >= 1.0f) {
			this.score = (6.0f - Mathf.Abs (count-6.0f))/6.0f;
			this.sumscore += score;
			iTween.ValueTo(
				gameObject,
				iTween.Hash(
					"from",this.bcount,
					"to",this.count,
					"time",1.0f,
					"onupdate","ValueChange"
				)
			);
			this.bcount = count;
			GameObject.Find ("Text").GetComponent<UnityEngine.UI.Text> ().text = this.sumscore.ToString();
			this.flame = 0.0f;
			this.count = 0;
		}
	}

	void ValueChange(float Change){
		GameObject.Find ("Slider").GetComponent<UnityEngine.UI.Slider> ().value = Change;
	}
}