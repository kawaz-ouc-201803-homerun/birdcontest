using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects {

	/// <summary>
	/// 仕込み役/援護役：爆弾の爆風動作
	/// </summary>
	public class ExplosionForce : MonoBehaviour, IFlightStarter {

		/// <summary>
		/// 爆発の強さ
		/// </summary>
		public float explosionForce = 4;

		/// <summary>
		/// 実況ステップ制御オブジェクト
		/// </summary>
		public StreamTextStepController StreamController;
		
		/// <summary>
		/// 爆発を実行します。
		/// </summary>
		public void DoFlightStart() {
			this.StartCoroutine(this.doExplosion());
		}

		/// <summary>
		/// 爆発を行うコルーチン
		/// </summary>
		private IEnumerator doExplosion() {
			Debug.Log("初動爆発");
			yield return new WaitForSeconds(3.0f);

			var explosionParticle = GetComponent<ParticleSystemMulti>();
			float multiplier = explosionParticle.multiplier;
			float r = 10 * multiplier;
			var cols = Physics.OverlapSphere(this.transform.position, r);
			var rigidbodies = new List<Rigidbody>();

			// 爆風に当たるオブジェクトのRigidbodyを列挙
			foreach(var col in cols) {
				if(col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody)) {
					rigidbodies.Add(col.attachedRigidbody);
				}
			}

			// 爆風県内の対象オブジェクトに相応の力を与える
			foreach(var rb in rigidbodies) {
				rb.AddExplosionForce(this.explosionForce * multiplier, this.transform.position, r, 1 * multiplier, ForceMode.Impulse);
			}

			// 爆発のパーティクル開始
			explosionParticle.PlayExplosionParticles();

			// 実況更新
			this.StreamController.CurrentFlightGameStep = StreamTextStepController.FlightStep.StartFlight;
		}

	}
}
