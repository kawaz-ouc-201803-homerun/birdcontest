using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// JSONICのRPC-APIに渡すリクエストパラメータークラス
/// </summary>
[Serializable]
public class ModelJsonicRequest {

	/// <summary>
	/// 呼び出すメソッド名
	/// </summary>
	public string method;

	/// <summary>
	/// メソッドの引数リスト
	/// </summary>
	public string[] param;

	/// <summary>
	/// 呼び出しID
	/// nullにするとJSONICの仕様によりレスポンスボディなしで返ってくるので注意。
	/// </summary>
	public string id;

}
