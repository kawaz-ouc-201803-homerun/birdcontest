using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAMeter : MonoBehaviour {

	public float bscore;

	public float score;

	public float flame;

	// Use this for initialization
	void Start () {
		Debug.Log ("MainB");
	}

	// Update is called once per frame
	void Update () {
		this.flame+=Time.deltaTime;
		if (flame >= 0.20f) {
			this.score = Random.Range(0f,100f);
			iTween.ValueTo(
				gameObject,
				iTween.Hash(
					"from",this.bscore,
					"to",this.score,
					"time",0.20f,
					"onupdate","ValueChange"
				)
			);
			this.bscore = score;
			this.flame = 0.0f;
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
			GameObject.Find ("Text").GetComponent<UnityEngine.UI.Text> ().text = score.ToString ();
			this.enabled = false;
		}
	}

	void ValueChange(float Change){
		GameObject.Find ("Slider").GetComponent<UnityEngine.UI.Slider> ().value = Change;
	}
}