using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainB : MonoBehaviour {
	
	public bool flag;

	public int count;

	public int cricks;

	public float flame;

	public int i;

	public int clickflag;

	public List<int> count2 = new List<int>();


	// Use this for initialization
	void Start () {
		Debug.Log ("MainB");
		this.i = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Click")){
			this.flag = true;
		}
		if (Input.GetButtonUp("Click") && this.flag==true) {
			this.count++;
			this.flag = false;
			GameObject.Find ("Slider").GetComponent<UnityEngine.UI.Slider> ().value = this.count;
			GameObject.Find ("Text").GetComponent<UnityEngine.UI.Text> ().text = this.count.ToString();
			this.cricks++;
		}
		this.flame+=Time.deltaTime;
		if (flame >= 1.0f && cricks > 0) {
			count2.Add(cricks);
			Debug.Log(count2[i]);
			this.flame = 0.0f;
			this.cricks = 0;
			if (i >= 1 && Mathf.Abs(count2 [i] - count2 [i - 1]) <= 1.0f) {
				this.clickflag++;
				// Debug.Log (clickflag);
			} else {
				this.clickflag = 0;
			}
			if(clickflag>=5){
				// count2の平均取って12or13だったら下の処理実行
				int sum = 0;
				count2.ForEach(new System.Action<int>((value) => {
					sum += value;
				}));
				float average = (float)sum / count2.Count;

				Debug.Log ("Average Per Second: " + average);

				Debug.Log ("You are not human!");
				this.count = 0;
				clickflag = 0;
				count2.Clear ();
				i = 0;
			}
			i++;
		}
	}
}