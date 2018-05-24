﻿using System;
using UnityEngine;

namespace UnityStandardAssets.Effects {

	/// <summary>
	/// 仕込み役/援護役：爆弾のパーティクルシステム
	/// </summary>
	public class ParticleSystemMulti : MonoBehaviour {

		/// <summary>
		/// パーティクル乗数
		/// </summary>
		public float multiplier = 1;

		/// <summary>
		/// 初期処理
		/// </summary>
		public void Start() {
			var particles = this.GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem particle in particles) {
				ParticleSystem.MainModule mModule = particle.main;
				mModule.startSizeMultiplier *= this.multiplier;
				mModule.startSpeedMultiplier *= this.multiplier;
				mModule.startLifetimeMultiplier *= Mathf.Lerp(this.multiplier, 1, 0.5f);
				particle.Stop();
			}
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public void Update() {
			// NOTE: 現状、Enter/Returnキーで発動する
			if(Input.GetKeyDown(KeyCode.Return) == true) {
				this.particlesPlay();
			}
		}

		/// <summary>
		/// パーティクル開始
		/// </summary>
		private void particlesPlay() {
			var systems = GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem system in systems) {
				ParticleSystem.MainModule mainModule = system.main;
				mainModule.startSizeMultiplier *= this.multiplier;
				mainModule.startSpeedMultiplier *= this.multiplier;
				mainModule.startLifetimeMultiplier *= Mathf.Lerp(this.multiplier, 1, 0.5f);
				system.Clear();
				system.Play();
			}
		}

	}
}