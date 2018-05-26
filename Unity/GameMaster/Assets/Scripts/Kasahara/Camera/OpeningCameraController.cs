using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 開始前のカメラワーク制御を行います。
/// </summary>
public class OpeningCameraController : MonoBehaviour {

	/// <summary>
	/// カメラワークの所要時間秒数
	/// </summary>
	public const float OpeningTimeSeconds = 9.0f;

	/// <summary>
	/// 飛行用のカメラワーク制御オブジェクト
	/// </summary>
	public CameraSwitcher CameraSwitcher;

	/// <summary>
	/// オープニングのカメラワークが完了して飛行の準備が整ったときに発生させるイベント
	/// </summary>
	public UnityEngine.Events.UnityEvent ReadyForStartEvent;

	/// <summary>
	/// 元のカメラアングルID
	/// </summary>
	private int previousCameraId;

	/// <summary>
	/// オープニング用のカメラワークを開始します。
	/// </summary>
	public void StartOpeningCameraWorks() {
		Debug.Log("オープニング開始");
		this.StartCoroutine(this.doOpeningCameraWorks());
	}

	/// <summary>
	/// 実際にオープニング用のカメラワークを行うコルーチン
	/// </summary>
	private IEnumerator doOpeningCameraWorks() {
		// 飛行用のカメラワークを止める
		this.previousCameraId = this.CameraSwitcher.CurrentCameraId;
		this.CameraSwitcher.DisenabledCameraAll();

		// オープニング用のカメラとアニメーションを有効にする
		this.GetComponent<Camera>().enabled = true;
		this.GetComponent<Animator>().enabled = true;

		// 終了まで待つ
		yield return new WaitForSeconds(OpeningCameraController.OpeningTimeSeconds);

		// 飛行用のカメラワークを復元する
		this.GetComponent<Camera>().enabled = false;
		this.GetComponent<Animator>().enabled = false;
		this.CameraSwitcher.ChangeCameraAngle((CameraSwitcher.CameraID)this.previousCameraId);

		// 飛行開始合図を出す
		if(this.ReadyForStartEvent != null) {
			this.ReadyForStartEvent.Invoke();
		}
	}

}
