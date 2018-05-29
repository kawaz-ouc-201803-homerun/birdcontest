using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仕込み役：牽引車で飛行機に助走をつける（物理）
/// </summary>
public class AutoMoveCar : PlaneBehaviourParent, IFlightStarter {

	/// <summary>
	/// 端末操作結果データ
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// アクセルオブジェクト
	/// </summary>
	public Accelerator Accel;

	/// <summary>
	/// エンジンSE制御オブジェクト
	/// </summary>
	public GameObject CarEngineSE;

	/// <summary>
	/// 牽引車の追従制御オブジェクト
	/// </summary>
	public FollowTarget FollowTarget;

	/// <summary>
	/// 牽引車のアニメーション制御コンポーネント
	/// </summary>
	public Animator CarAnimator;

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
	/// タイヤが空転しているかどうか
	/// </summary>
	private bool isRacing = false;

	/// <summary>
	/// クエリちゃんボイス再生制御オブジェクト
	/// </summary>
	public QueryChanVoicePlayer VoicePlayer;

	/// <summary>
	/// 毎フレーム更新処理
	/// 機体に対し、毎フレーム増加していく力を加え続けます。
	/// </summary>
	public void FixedUpdate() {
		if(this.isRacing == false && this.Accel.CurrentState == Accelerator.State.Running) {
			// アクセル走行中
			this.Accel.AccelUpDown(false);
			this.MovePower = this.MovePower + 5f * Time.deltaTime;
			this.PlaneRigidbody.AddForce(this.PlaneRigidbody.transform.forward * this.MovePower, this.PowerForce);
		}
	}

	/// <summary>
	/// StopperタグがついているPlaneに触った時に離陸開始となり、このスクリプトを停止させます。
	/// </summary>
	/// <param name="other">接したオブジェクトのコライダー</param>
	public void OnTriggerEnter(Collider other) {
		if(this.enabled == true && other.gameObject.tag == "Stopper") {
			// 特に根拠がない上向きの力を加えて離陸させる
			this.PlaneRigidbody.AddForce(this.PlaneRigidbody.transform.up * 500, ForceMode.Impulse);
			this.enabled = false;

			// ボイス再生
			this.VoicePlayer.PlaySE((int)QueryChanVoicePlayer.SEID.Ending);
		}
	}

	/// <summary>
	/// 自律移動を開始します。
	/// </summary>
	public virtual void DoFlightStart() {
		// アクセル演出開始
		this.StartCoroutine(this.accelEffect());
	}

	/// <summary>
	/// アクセルとともにこのコンポーネントを無効化します。
	/// </summary>
	public void Disable() {
		Debug.Log("牽引終了");
		this.enabled = false;
		this.CarEngineSE.SetActive(false);
	}

	/// <summary>
	/// アクセル開度の演出を行うコルーチン
	/// </summary>
	private IEnumerator accelEffect() {
		// クラッチ切った状態でアクセル吹かす：目標回転数は変換前の値を使って算出する
		this.Accel.TargetRevs =
			int.Parse(((Dictionary<string, string>)this.DataContainer.ControllerData[(int)NetworkConnector.RoleIds.A_Prepare])["param"]) / 100f;
		this.Accel.ReadyForStart();
		yield return new WaitForSeconds(1.5f);

		// ボイス再生
		this.VoicePlayer.PlaySE((int)QueryChanVoicePlayer.SEID.CarStart);
		yield return new WaitForSeconds(1.5f);

		// クラッチ繋ぐ
		this.Accel.Run();
		if(this.Accel.CurrentState == Accelerator.State.Racing) {
			// エンジンの回転数が高すぎてタイヤが空転
			Debug.Log("タイヤが空転");
			this.isRacing = true;

			// ボイス再生
			this.VoicePlayer.PlaySE((int)QueryChanVoicePlayer.SEID.CarRacing);

			var defaultPosition = this.transform.position;
			var defaultAngles = this.transform.eulerAngles;

			this.CarAnimator.enabled = true;
			yield return new WaitForSeconds(1.5f);

			// 回転数がゼロに戻ってリスタートする
			this.CarAnimator.enabled = false;
			this.transform.position = defaultPosition;
			this.transform.eulerAngles = defaultAngles;
			this.isRacing = false;

			yield return new WaitForFixedUpdate();
			yield return new WaitForSeconds(1.0f);
		}

		// 移動開始
		this.Accel.CurrentState = Accelerator.State.Ready;
		this.enabled = true;
		if(this.StartMoveEvent != null) {
			// 移動を開始したときにイベントを発生させる
			this.StartMoveEvent.Invoke();
		}
	}

}
