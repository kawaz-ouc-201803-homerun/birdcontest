using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末操作フェーズの結果を保管する場所です。
/// ＊飛行フェーズのrootオブジェクトにアタッチして下さい。
/// </summary>
public class DataContainer : MonoBehaviour {

	/// <summary>
	/// 飛行フェーズのルートオブジェクト
	/// ＊インスペクターにて 3DObjects_Kasahara を設定して下さい。
	/// </summary>
	public GameObject RootObject;

	/// <summary>
	/// 端末操作フェーズの結果データ
	/// </summary>
	public object[] ControllerData {
		get;
		private set;
	}

	/// <summary>
	/// 端末A：選択肢
	/// </summary>
	public int OptionA {
		get;
		private set;
	}

	/// <summary>
	/// 端末A：結果値
	/// </summary>
	public int ParamA {
		get;
		private set;
	}

	/// <summary>
	/// 端末A：クエリちゃん
	/// </summary>
	public GameObject ControllerAQueryChan;

	/// <summary>
	/// 端末A：爆弾
	/// </summary>
	public GameObject ControllerABomb;

	/// <summary>
	/// 端末A：牽引車
	/// </summary>
	public GameObject ControllerACar;

	/// <summary>
	/// 端末B：結果値
	/// </summary>
	public int ParamB {
		get;
		private set;
	}

	/// <summary>
	/// 端末B：飛行機の機体
	/// </summary>
	public GameObject ControllerBPlane;

	/// <summary>
	/// 端末C：選択肢
	/// </summary>
	public int OptionC {
		get;
		private set;
	}

	/// <summary>
	/// 端末C：結果値
	/// </summary>
	public int ParamC {
		get;
		private set;
	}

	/// <summary>
	/// 端末C：爆弾
	/// </summary>
	public GameObject ControllerCBomb;

	/// <summary>
	/// 端末C：応援カットイン
	/// </summary>
	public Cheerup ControllerCCheerup;

	/// <summary>
	/// 端末C：賄賂カットイン
	/// </summary>
	public PayBribe ControllerCPayBribe;

	/// <summary>
	/// データを渡して初期化します。
	/// </summary>
	/// <param name="controllerData">各端末が報告してきた辞書型配列データの配列。[0]=端末A、[1]=端末B, [2]=端末C</param>
	public void Setup(object[] controllerData) {
		this.ControllerData = controllerData;

		// データを解析してそれぞれの変数に格納する
		this.OptionA = int.Parse((controllerData[0] as Dictionary<string, string>)["option"]);
		this.ParamA = int.Parse((controllerData[0] as Dictionary<string, string>)["param"]);
		this.ParamB = int.Parse((controllerData[1] as Dictionary<string, string>)["param"]);
		this.OptionC = int.Parse((controllerData[2] as Dictionary<string, string>)["option"]);
		this.ParamC = int.Parse((controllerData[2] as Dictionary<string, string>)["param"]);

		// 端末A
		switch(this.OptionA) {
			case (int)PhaseControllers.OptionA.Bomb:
				Debug.Log("仕込み役「爆弾」");

				// 飛行機の位置を変更
				this.transform.position = new Vector3(-137, 86, -69);
				this.ControllerABomb.SetActive(true);

				// 結果値を爆発威力としてセット
				this.ControllerABomb.GetComponent<UnityStandardAssets.Effects.ExplosionForce>().explosionForce = this.ParamA;
				break;

			case (int)PhaseControllers.OptionA.Car:
				Debug.Log("仕込み役「牽引」");

				// 飛行機の位置を変更
				this.transform.position = new Vector3(-77, 86, -60);
				this.ControllerACar.SetActive(true);

				// 結果値を車の牽引力としてセット
				this.GetComponent<AutoMove>().MovePower = this.ParamA;
				break;

			case (int)PhaseControllers.OptionA.Human:
				Debug.Log("仕込み役「人力」");

				// 飛行機の位置を変更
				this.transform.position = new Vector3(-77, 86, -60);
				this.ControllerAQueryChan.SetActive(true);

				// クエリちゃんのアニメーションを開始
				this.ControllerAQueryChan.GetComponent<QueryChanAnimationController>().enabled = true;
				this.ControllerAQueryChan.GetComponent<QueryChanAnimationController>().QueryChanAnimator.SetTrigger("Start");

				// 結果値を飛行機を押す力としてセット
				this.GetComponent<AutoMove>().MovePower = this.ParamA;
				break;
		}


		// 端末B
		this.GetComponent<PompPedal>().PedalPower = this.ParamB;


		// 端末C
		switch(this.OptionC) {
			case (int)PhaseControllers.OptionC.Bomb:
				Debug.Log("援護役「爆弾」");
				this.GetComponent<SecondExplosion>().enabled = true;
				SecondExplosion.EnabledOnTrigegrEnter = true;

				if(this.OptionA == (int)PhaseControllers.OptionA.Bomb) {
					// AもCも爆弾だった場合、飛行機の墜落演出を行うため爆発の威力を無効化する
					this.ParamC = 0;
				}

				// 爆発威力をセット
				this.ControllerCBomb.GetComponent<UnityStandardAssets.Effects.ExplosionPhysicsForce>().explosionForce = this.ParamC;
				break;

			case (int)PhaseControllers.OptionC.Human:
				Debug.Log("援護役「気合」");
				this.ControllerCCheerup.IsCutinEnabled = true;
				this.ControllerCCheerup.UpperPower = this.ParamC;
				break;

			case (int)PhaseControllers.OptionC.Wairo:
				Debug.Log("援護役「賄賂」");
				this.ControllerCPayBribe.IsCutinEnabled = true;
				break;
		}

		// 着地判定の開始
		this.RootObject.GetComponent<LandingJudge>().enabled = true;
	}

}
