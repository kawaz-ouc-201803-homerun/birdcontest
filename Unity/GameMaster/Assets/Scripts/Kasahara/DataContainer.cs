using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 端末操作フェーズの結果を代入する場所
/// </summary>
public class DataContainer {

	/// <summary>
	/// 端末操作フェーズの結果データ
	/// </summary>
	public static object[] ControllerData {
		get {
			return DataContainer.controllerData;
		}
		set {
			DataContainer.controllerData = value;

			// データを解析してそれぞれの変数に格納する
			DataContainer.OptionA = int.Parse((value[0] as Dictionary<string, string>)["option"]);
			DataContainer.ParamA = int.Parse((value[0] as Dictionary<string, string>)["param"]);
			DataContainer.ParamB = int.Parse((value[1] as Dictionary<string, string>)["param"]);
			DataContainer.OptionC = int.Parse((value[2] as Dictionary<string, string>)["option"]);
			if(DataContainer.OptionC != (int)PhaseControllers.OptionC.Wairo) {
				DataContainer.ParamC = int.Parse((value[2] as Dictionary<string, string>)["option"]);
			} else {
				// 賄賂ミニゲームのみ、別のキーで結果を管理している
				DataContainer.ParamC = int.Parse((value[2] as Dictionary<string, string>)["wairoScore"]);
			}
		}
	}

	/// <summary>
	/// 端末操作フェーズの結果データの実体
	/// </summary>
	private static object[] controllerData;

	/// <summary>
	/// 端末A：選択肢
	/// </summary>
	public static int OptionA {
		get;
		private set;
	}

	/// <summary>
	/// 端末A：結果値
	/// </summary>
	public static int ParamA {
		get;
		private set;
	}

	/// <summary>
	/// 端末B：結果値
	/// </summary>
	public static int ParamB {
		get;
		private set;
	}

	/// <summary>
	/// 端末C：選択肢
	/// </summary>
	public static int OptionC {
		get;
		private set;
	}

	/// <summary>
	/// 端末C：結果値
	/// </summary>
	public static int ParamC {
		get;
		private set;
	}

}
