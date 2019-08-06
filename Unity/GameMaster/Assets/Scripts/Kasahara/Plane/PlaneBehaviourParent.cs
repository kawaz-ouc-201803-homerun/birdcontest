using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 機体に対して振る舞うための基底クラス
/// </summary>
public class PlaneBehaviourParent : MonoBehaviour {

	/// <summary>
	/// 操作対象の機体オブジェクト
	/// </summary>
	public GameObject Plane;

	/// <summary>
	/// 操作対象の機体のRigidbodyコンポーネント
	/// </summary>
	public Rigidbody PlaneRigidbody;

}
