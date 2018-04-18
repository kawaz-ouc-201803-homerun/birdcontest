using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// WebAPIに渡すパラメータークラス
/// </summary>
public class ModelHttpRequest {

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
