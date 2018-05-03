using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
	
public class ParticleSystemMulti : MonoBehaviour {

		public float multiplier = 1;

	// Use this for initialization
	private void Update () 
			{
			if(Input.GetKeyDown(KeyCode.Return)){
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
	
	
	// Update is called once per frame

}
}