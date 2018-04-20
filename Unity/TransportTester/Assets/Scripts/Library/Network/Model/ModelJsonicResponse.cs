using System;
using System.Collections.Generic;

/// <summary>
/// JSONICのRPC-APIが返す結果オブジェクト
/// </summary>
[Serializable]
public class ModelJsonicResponse<T> {

	/// <summary>
	/// APIメソッドが返した結果オブジェクト
	/// </summary>
	public T result;

	/// <summary>
	/// エラー内容
	/// </summary>
	public ModelJsonicError error;

	/// <summary>
	/// 通信ID
	/// </summary>
	public string id;

}
