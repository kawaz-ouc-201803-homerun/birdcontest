using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour {
	public GameObject Seccond;
	public GameObject End;

	// すたーとぼたんをおしたときのがめんせんい
	public void OnClick () {
		this.Seccond.SetActive(false);
		this.End.SetActive(true);
	}


}
