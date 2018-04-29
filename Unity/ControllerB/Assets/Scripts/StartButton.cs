using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour {

	public GameObject First;
	public GameObject Seccond;
	public GameObject End;

	// すたーとぼたんをおしたときのがめんせんい
	public void OnClick () {
		this.First.SetActive(false);
		this.Seccond.SetActive (true);
	}

}
