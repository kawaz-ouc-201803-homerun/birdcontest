//
//RandomWind.cs for unity-chan!
//
//Original Script is here:
//ricopin / RandomWind.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityChan
{
	public class RandomWind : MonoBehaviour
	{
		private SpringBone[] springBones;
		//public Slider slide = null;
		public bool isWindActive = true;
		public float windForce = 0.005f;

		// Use this for initialization
		void Start ()
		{
			springBones = GetComponent<SpringManager> ().springBones;
		}

		// Update is called once per frame
		void Update ()
		{
			Vector3 force = Vector3.zero;
			if (isWindActive) {
				force = new Vector3 (Mathf.PerlinNoise (Time.time, 0.0f) * windForce, 0, 0);
			}
			//force = Quaternion.AngleAxis (slide.value, Vector3.up) * force;

			for (int i = 0; i < springBones.Length; i++) {
				springBones [i].springForce = force;
			}
		}

		public void SetToggle () {
			isWindActive ^= true;
		}


		void OnGUI ()
		{
			Rect rect1 = new Rect (10, Screen.height - 80, 100, 30);
			isWindActive = GUI.Toggle (rect1, isWindActive, "Random Wind");
		}


	}
}