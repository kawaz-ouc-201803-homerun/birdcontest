using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShowScore : MonoBehaviour {
	public MainB mainB;
	public int bcount;
	// Use this for initialization
	void Start () {
		bcount = mainB.count;
		GameObject.Find ("show.score").GetComponent<UnityEngine.UI.Text> ().text = "あなたのパワーは"+this.bcount+"だ！";
	}
}
