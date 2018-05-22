using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects{
	
public class ExplosionController : MonoBehaviour {
		

	public GameObject explosion;
		/*public ParticleSystem particle1;
		public ParticleSystem particle2;
		public ParticleSystem particle3;
		public ParticleSystem particle4;
		public ParticleSystem particle5;*/

		// Use this for initialization
	void Start () {
			var particles = GetComponentInChildren<ParticleSystem> ();
			/*particle1.Stop();
			particle2.Stop();
			particle3.Stop();
			particle4.Stop();
			particle5.Stop();*/
			//explosion.SetActive (false);
				particles.Stop ();

	}
	
	// Update is called once per frame
	public void Update () {
			if (Input.GetKeyDown (KeyCode.Return)) {
				/*particle1.Clear ();
				particle2.Clear ();
				particle3.Clear ();
				particle4.Clear ();
				particle5.Clear ();

				particle1.Play();
				particle2.Play();
				particle3.Play();
				particle4.Play();
				particle5.Play();*/


				//explosion.SetActive (true);

			}
		}
			
}
}
