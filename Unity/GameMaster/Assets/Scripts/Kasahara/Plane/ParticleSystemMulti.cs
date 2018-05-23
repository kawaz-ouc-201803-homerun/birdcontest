using System;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
	
public class ParticleSystemMulti : MonoBehaviour {

		public float multiplier = 1;

		void Start () {
			var particles = GetComponentsInChildren<ParticleSystem> ();
			foreach (ParticleSystem particle in particles) {
				ParticleSystem.MainModule mModule = particle.main;
				mModule.startSizeMultiplier *= multiplier;
				mModule.startSpeedMultiplier *= multiplier;
				mModule.startLifetimeMultiplier *= Mathf.Lerp(multiplier, 1, 0.5f);
				particle.Stop();
			}
		}


	// Use this for initialization
	private void Update () 
			{
			if(Input.GetKeyDown(KeyCode.Return)){
				ParticlesPlay ();
				
			}
				}

		private void ParticlesPlay(){
			var systems = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem system in systems)
			{
				ParticleSystem.MainModule mainModule = system.main;
				mainModule.startSizeMultiplier *= multiplier;
				mainModule.startSpeedMultiplier *= multiplier;
				mainModule.startLifetimeMultiplier *= Mathf.Lerp(multiplier, 1, 0.5f);
				system.Clear();
				system.Play();
			}
		}
	
	
}
}