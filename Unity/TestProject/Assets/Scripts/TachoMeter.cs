using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクセルオブジェクトのエンジン回転数に合わせてタコメーターを変動させます。
/// </summary>
public class TachoMeter : MonoBehaviour {

	/// <summary>
	/// ニュートラル時のエンジン回転数
	/// </summary>
	public const float NeutralRevs = 0.12f;

	/// <summary>
	/// アクセルオブジェクト
	/// </summary>
	public Accelerator Accel;

	/// <summary>
	/// メーターの針
	/// </summary>
	private GameObject Pointer;

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public void Start () {
		this.Pointer = this.transform.Find("Pointer").gameObject;
		this.Pointer.transform.eulerAngles = Vector3.zero;
	}
	
	/// <summary>
	/// 毎フレーム更新
	/// </summary>
	public void Update () {
		// メーター針の回転限界角度の縮尺をエンジン回転数に適用する
		var rotateRate = (TachoMeter.NeutralRevs + this.Accel.EngineRevs) * -240f + 360f;
		this.Pointer.transform.eulerAngles = new Vector3(0, 0, rotateRate);
	}

}
