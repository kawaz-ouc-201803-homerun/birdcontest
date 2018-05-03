using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：飛行本番
/// 
/// ＊各端末の操作結果を踏まえて、実際に走らせる
/// 
/// </summary>
public class PhaseFlight : PhaseBase {

	/// <summary>
	/// 地点表示を行う秒数
	/// </summary>
	private const float MapAddressShowTimeSecond = 3.0f;

	/// <summary>
	/// 飛距離
	/// </summary>
	private float score;

	/// <summary>
	/// 現在地点の名称
	/// </summary>
	private string mapAddress;

	/// <summary>
	/// 機体オブジェクト
	/// </summary>
	private GameObject airplane;

	/// <summary>
	/// 地点表示コルーチン
	/// </summary>
	private Coroutine mapAddressCoroutine;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="parameters">[0]=イベントID, [1]=SceneA結果, [2]=SceneB結果, [3]=SceneC結果</param>
	public PhaseFlight(PhaseManager parent, object[] parameters) : base(parent, parameters) {
		this.score = 0;
		this.mapAddress = "";
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public override void Start() {
		// メインカメラのアニメーション（舞台背景カメラワーク）を無効化
		GameObject.Find("Main Camera").GetComponent<Animator>().enabled = false;

		// TODO: 機体オブジェクトを保管
		this.airplane = GameObject.Find("Airplane");
		GameObject.Find("Flight_ScoreText").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00} m", 0);
		this.setScore(123.45f);
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		// TODO: 距離計算して setScore(..., true); する

		// 現在地点の表示
		string afterMapAddress = this.getMapAddress(this.airplane.transform.position);
		if(this.mapAddress != afterMapAddress) {
			// 変更があったときに一時的に表示する
			if(this.mapAddressCoroutine != null) {
				this.parent.StopCoroutine(this.mapAddressCoroutine);
			}
			this.mapAddress = afterMapAddress;
			this.mapAddressCoroutine = this.parent.StartCoroutine(this.showMapAddress());
		}
	}

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public override void Destroy() {
		// メインカメラのアニメーション（舞台背景カメラワーク）を有効化
		GameObject.Find("Main Camera").GetComponent<Animator>().enabled = true;
	}

	/// <summary>
	/// 飛距離をセットします。
	/// </summary>
	/// <param name="score">飛距離</param>
	/// <param name="disableEase">iTweenによる滑らかな増減を無効にするかどうか</param>
	private void setScore(float score, bool disableEase = false) {
		var beforeScore = this.score;
		this.score = score;

		// １フレームごとの更新処理内容
		var updateCallback = new Action<object>((value) => {
			GameObject.Find("Flight_ScoreText").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.00} m", (float)value);
		});

		if(disableEase == true) {
			// 直接変更
			updateCallback(score);
		} else {
			// 滑らかに変更
			iTween.ValueTo(
				GameObject.Find("Flight_ScoreText"),
				iTween.Hash(
					"from", beforeScore,
					"to", score,
					"time", 2.0f,
					"easeType", iTween.EaseType.linear,
					"onupdate", updateCallback
				)
			);
		}
	}

	/// <summary>
	/// 現在地点名称表示
	/// </summary>
	private IEnumerator showMapAddress() {
		var window = GameObject.Find("Flight_AddressWindow");

		// 座標と透明度を変化させながら表示する
		GameObject.Find("Flight_AddressText").GetComponent<UnityEngine.UI.Text>().text = this.mapAddress;
		window.transform.position = new Vector3(
			1400f,
			window.transform.position.y,
			window.transform.position.z
		);
		iTween.MoveTo(
			window,
			iTween.Hash(
				"x", 960f,
				"time", PhaseFlight.MapAddressShowTimeSecond,
				"easeType", iTween.EaseType.easeOutQuart,
				"isLocal", true
			)
		);
		yield return new WaitForSeconds(3.0f);

		// 一定時間表示した後、自動的に非表示にする
		yield return new WaitForSeconds(3.0f);

		iTween.MoveTo(
			window,
			iTween.Hash(
				"x", 1400f,
				"time", PhaseFlight.MapAddressShowTimeSecond,
				"easeType", iTween.EaseType.easeOutQuart,
				"isLocal", true
			)
		);
		yield return new WaitForSeconds(3.0f);
	}

	/// <summary>
	/// 座標から大通付近の住所を割り出します。
	/// </summary>
	/// <param name="position">座標</param>
	/// <returns>住所テキスト</returns>
	private string getMapAddress(Vector3 position) {

		// TODO: 座標に応じて住所割り出し

		return "札幌市 大通公園";
	}

	/// <summary>
	/// 飛行フェーズを終えて結果フェーズへ移行します。
	/// </summary>
	public void ChangeToResultPhase() {
		this.parent.ChangePhase(new PhaseResult(this.parent, new object[] {
			(string) this.parameters[0] ?? "none",
			this.score
		}));
	}

}
