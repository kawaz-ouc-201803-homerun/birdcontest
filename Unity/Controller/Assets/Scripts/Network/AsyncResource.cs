using System;
using System.Collections.Generic;

/// <summary>
/// 非同期リソースオブジェクト
/// </summary>
public class AsyncResource<T> {

	/// <summary>
	/// 非同期リソース
	/// </summary>
	public T Resource;

	/// <summary>
	/// 非同期接続を参照するためのオブジェクト
	/// </summary>
	public IAsyncResult AsyncResult;

}
