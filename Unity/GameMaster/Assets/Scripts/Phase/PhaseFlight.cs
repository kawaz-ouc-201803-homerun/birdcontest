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
	/// コンストラクター
	/// </summary>
	/// <param name="parameters">[0]=SceneA結果、[1]=SceneB結果、[2]=SceneC結果</param>
	public PhaseFlight(object[] parameters) : base(parameters) {

	}
	
	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {

	}

}
