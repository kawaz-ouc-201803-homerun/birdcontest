using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

/// <summary>
/// 端末操作フェーズを飛ばして直接データを代入するデバッグ用の間接クラス
/// ＊飛行フェーズのrootオブジェクトにアタッチして下さい。
/// </summary>
public class DataContainerDebugger : MonoBehaviour {

	/// <summary>
	/// 端末操作結果データ
	/// ＊インスペクターにて設定して下さい。
	/// </summary>
	public DataContainer DataContainer;

	/// <summary>
	/// 端末A：選択肢
	/// </summary>
	public PhaseControllers.OptionA OptionA;

	/// <summary>
	/// 端末A：結果値
	/// </summary>
	public int ParamA;

	/// <summary>
	/// 端末B：結果値
	/// </summary>
	public int ParamB;

	/// <summary>
	/// 端末C：選択肢
	/// </summary>
	public PhaseControllers.OptionC OptionC;

	/// <summary>
	/// 端末C：結果値
	/// </summary>
	public int ParamC;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
#if UNITY_EDITOR
		// 操作端末の結果データオブジェクトに直接インスペクターで設定された値を適用する
		this.DataContainer.Setup(new object[] {
			new Dictionary<string, string>() {
				{ "option", ((int)this.OptionA).ToString() },
				{ "param", this.ParamA.ToString() },
			},
			new Dictionary<string, string>() {
				{ "param", this.ParamB.ToString() },
			},
			new Dictionary<string, string>() {
				{ "option", ((int)this.OptionC).ToString() },
				{ "param", this.ParamC.ToString() },
			},
		});
#endif
	}

}
