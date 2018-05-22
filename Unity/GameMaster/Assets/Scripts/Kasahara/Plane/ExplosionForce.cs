using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects{
public class ExplosionForce : MonoBehaviour {
		public float explosionForce = 4;
	
	void Update () {
			if (Input.GetKeyDown (KeyCode.Return)) {
				
				float multiplier = GetComponent<ParticleSystemMulti> ().multiplier;

				float r = 10 * multiplier;
				var cols = Physics.OverlapSphere (transform.position, r);
				var rigidbodies = new List<Rigidbody> ();
				foreach (var col in cols) {
					if (col.attachedRigidbody != null && !rigidbodies.Contains (col.attachedRigidbody)) {
						rigidbodies.Add (col.attachedRigidbody);
					}
				}
				foreach (var rb in rigidbodies) {
					rb.AddExplosionForce (explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);
				}
			}
		}
}
}
