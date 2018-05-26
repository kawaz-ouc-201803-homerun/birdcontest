using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み役：人力で飛行機に助走をつける（物理）
/// ＊このスクリプトは助走をつける主体となるオブジェクトにアタッチして下さい。
/// </summary>
public class AutoMoveHuman : PlaneBehaviourParent, IFlightStarter {

	/// <summary>
	/// 前に進む力の大きさ
	/// </summary>
	public float MovePower;

	/// <summary>
	/// 加える力の種類
	/// </summary>
	public ForceMode PowerForce;

	/// <summary>
	/// 移動を開始したときに呼び出されるイベント
	/// </summary>
	public UnityEngine.Events.UnityEvent StartMoveEvent;

	/// <summary>
	/// 毎フレーム更新処理
	/// 機体に対し、毎フレーム増加していく力を加え続けます。
	/// </summary>
	public void FixedUpdate() {
		this.MovePower = this.MovePower + 5f * Time.deltaTime;
		this.PlaneRigidbody.AddForce(this.PlaneRigidbody.transform.forward * this.MovePower, this.PowerForce);
	}

	/// <summary>
	/// StopperタグがついているPlaneに触った時に離陸開始となり、このスクリプトを停止させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Stopper") {
			// 特に根拠がない上向きの力を加えて離陸させる
			this.PlaneRigidbody.AddForce(this.PlaneRigidbody.transform.up * 500, ForceMode.Impulse);
			this.enabled = false;
		}
	}

	/// <summary>
	/// 自律移動を開始します。
	/// </summary>
	public virtual void DoFlightStart() {
		this.enabled = true;
		if(this.StartMoveEvent != null) {
			// 移動を開始したときにイベントを発生させる
			this.StartMoveEvent.Invoke();
		}
	}

}
