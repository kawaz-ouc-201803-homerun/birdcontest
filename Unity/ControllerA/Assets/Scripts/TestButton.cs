using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour {
	// すたーとぼたんをおしたときのがめんせんい
	public MainB mainB;
	public float bscore;
	public void OnClick () {
		this.bscore = mainB.score;
		GameObject.Find ("Text").GetComponent<UnityEngine.UI.Text>().text=bscore.ToString();
		mainB.enabled = false;
	}


}
