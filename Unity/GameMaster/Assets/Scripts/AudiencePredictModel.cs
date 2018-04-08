using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オーディエンス予想モデル
/// </summary>
public class AudiencePredictModel {

	/// <summary>
	/// 投稿者名
	/// </summary>
	public string AudienceName {
		get; set;
	}

	/// <summary>
	/// 予想値
	/// </summary>
	public int Predict {
		get; set;
	}

	/// <summary>
	/// 投稿日時
	/// </summary>
	public Time CreateDate {
		get; set;
	}

}
