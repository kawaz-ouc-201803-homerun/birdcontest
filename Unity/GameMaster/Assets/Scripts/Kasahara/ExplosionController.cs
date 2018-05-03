using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects{
	
public class ExplosionController : MonoBehaviour {
	[SerializeField]
	public GameObject explosion;

	// Use this for initialization
	void Start () {
			explosion.SetActive (false);
	}
	
	// Update is called once per frame
	public void Update () {
			if (Input.GetKeyDown (KeyCode.Return)) {
				explosion.SetActive (true);
			}
		}
			
}
}
