using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 助走をつける動作を開始させます。
/// </summary>
public class FlightStarter : MonoBehaviour {

	/// <summary>
	/// 端末操作結果データ
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// 端末A：実際に飛行を開始させるオブジェクト
	/// ＊インターフェース型なので残念ながらインスペクターから設定できません。
	/// </summary>
	public Dictionary<int, IFlightStarter> ControllerAFlightStarter;

	/// <summary>
	/// 端末A：爆弾
	/// </summary>
	public UnityStandardAssets.Effects.ExplosionForce ControllerABomb;

	/// <summary>
	/// 端末A：牽引
	/// </summary>
	public AutoMove ControllerACar;

	/// <summary>
	/// 端末A：人力
	/// </summary>
	public AutoMove ControllerAHuman;

	/// <summary>
	/// 端末B：飛行機の機体
	/// </summary>
	public GameObject ControllerBPlane;

	/// <summary>
	/// 端末B：ユニティちゃんボイス
	/// </summary>
	public UnityChanVoiceManerger ControllerBPilot;

	/// <summary>
	/// 開始したかどうか
	/// </summary>
	private bool isStarted = false;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		// 開始可能なオブジェクトを初期化
		this.ControllerAFlightStarter = new Dictionary<int, IFlightStarter>();
		this.ControllerAFlightStarter[(int)PhaseControllers.OptionA.Bomb] = this.ControllerABomb;
		this.ControllerAFlightStarter[(int)PhaseControllers.OptionA.Car] = this.ControllerACar;
		this.ControllerAFlightStarter[(int)PhaseControllers.OptionA.Human] = this.ControllerAHuman;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
#if UNITY_EDITOR
		// デバッグ時のみ、Enterキーで開始する
		if(Input.GetKeyDown(KeyCode.Return) == true && this.isStarted == false) {
			this.isStarted = true;
			this.DoFlightStart();
		}
#endif
	}

	/// <summary>
	/// 一連の飛行動作を開始させます。
	/// </summary>
	public void DoFlightStart() {
		Debug.Log("仕込み開始");
		this.ControllerAFlightStarter[this.DataContainer.OptionA].DoFlightStart();

		// ユニティちゃんの発進ボイスを再生
		this.ControllerBPilot.PlayVoice(UnityChanVoiceManerger.UnityChanVoiceIndexes.Starting);
	}

}
