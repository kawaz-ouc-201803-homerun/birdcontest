using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
///  JSONICのRPC-APIが返すエラー情報
/// </summary>
[Serializable]
public class ModelJsonicError {

	/// <summary>
	/// エラーコード
	/// </summary>
	public int code;

	/// <summary>
	/// エラーメッセージ
	/// </summary>
	public string message;
	
}