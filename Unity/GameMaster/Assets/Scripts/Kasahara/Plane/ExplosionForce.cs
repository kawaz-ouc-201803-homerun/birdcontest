using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects {

	/// <summary>
	/// 仕込み役/援護役：爆弾の爆風動作
	/// </summary>
	public class ExplosionForce : MonoBehaviour {

		/// <summary>
		/// 爆発の強さ
		/// </summary>
		public float explosionForce = 4;

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		void Update() {
			// NOTE: 現状、Enter/Returnキーで発動する
			if(Input.GetKeyDown(KeyCode.Return) == true) {
				float multiplier = GetComponent<ParticleSystemMulti>().multiplier;
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
			}
		}

	}
}
