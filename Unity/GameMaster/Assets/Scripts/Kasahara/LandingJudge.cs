using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingJudge : MonoBehaviour {

	//Escapeキーを押してゲームを終了させる

	public UnityEngine.Events.UnityEvent LandingEvent;

	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log ("終了");

			if (this.LandingEvent != null) {
				this.LandingEvent.Invoke ();
			}

		}

	}

}
