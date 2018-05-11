using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StomGaugeGameUI : MonoBehaviour {
	public Text t;

	// Use this for initialization
	void Start () {
		t.text = "タイミングよくゲージを\n[Kキー]で止めろ！！";
	}
	
	// Update is called once per frame
	void Update () {
	}
}
