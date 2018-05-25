using UnityEngine;

/// <summary>
/// 援護役：爆弾
/// 仕込み役と援護役が両方とも爆弾だったときは墜落させます。
/// </summary>
public class SecondExplosion : PlaneBehaviourParent {

	/// <summary>
	/// 端末操作結果データオブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// 飛距離計算を行うオブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DistanceCalculator DistanceCalculator;

	/// <summary>
	/// 爆弾オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject SecondBomb;

	/// <summary>
	/// 黒煙オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject Smoke;

	/// <summary>
	/// 墜落時に差し替えるダミーの飛行機オブジェクト
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public GameObject DummyPlane;

	/// <summary>
	/// トリガー対象に接触したら爆発させるかどうか
	/// </summary>
	public bool EnabledOnTrigegrEnter = false;

	/// <summary>
	/// トリガー対象に接触したら開始します。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(this.EnabledOnTrigegrEnter == false || other.gameObject.tag != "Trigger") {
			return;
		}

		Debug.Log("援護役「爆弾トリガー」発動");

		if(this.DataContainer.OptionA == (int)PhaseControllers.OptionA.Bomb
		&& this.DataContainer.OptionC == (int)PhaseControllers.OptionC.Bomb) {
			// AとCどちらも爆弾だった場合、ダミーの機体に差し替えて墜落演出を行う

			// ダミー機体のセットアップ
			this.DummyPlane.transform.position = this.Plane.transform.position;
			// this.distanceCalculator.enabled = false;         // 元の飛行機を消すとエラー吐きまくるのであらかじめ機能を停止させておく
			this.DistanceCalculator.Target = this.DummyPlane;   // 飛距離計算の対象をダミー機体に差し替える
			GameObject.Destroy(this.Plane);
			this.DummyPlane.SetActive(true);

			// 黒煙を上げて、ダミー機体を上に飛ばす
			this.Smoke.SetActive(true);
			this.DummyPlane.GetComponent<Rigidbody>().AddForce(
				this.transform.up * 20,
				ForceMode.VelocityChange
			);
		}

		// 爆発位置を現在座標に合わせて爆発実行
		this.SecondBomb.transform.position = this.Plane.transform.position;
		this.SecondBomb.SetActive(true);
		this.EnabledOnTrigegrEnter = false;
	}

}
