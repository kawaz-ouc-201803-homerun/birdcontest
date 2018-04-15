using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 画面上にログを出力します。
/// </summary>
public class Logger : MonoBehaviour {

	/// <summary>
	/// 時刻フォーマット
	/// </summary>
	private const string TimeFormat = "HH:mm:ss.fff";

	/// <summary>
	/// 実行ログバッファー
	/// </summary>
	private static StringWriter loggerProcess = new StringWriter();

	/// <summary>
	/// 結果ログバッファー
	/// </summary>
	private static StringWriter loggerResult = new StringWriter();

	/// <summary>
	/// 開始時の処理
	/// </summary>
	public void Start() {
		GameObject.Find("ProcessLog").GetComponent<UnityEngine.UI.Text>().text = "";
		GameObject.Find("ResultLog").GetComponent<UnityEngine.UI.Text>().text = "";
	}

	/// <summary>
	/// 毎フレームの処理
	/// </summary>
	public void Update() {
		GameObject.Find("ProcessLog").GetComponent<UnityEngine.UI.Text>().text = Logger.loggerProcess.ToString();
		GameObject.Find("ResultLog").GetComponent<UnityEngine.UI.Text>().text = Logger.loggerResult.ToString();
	}

	/// <summary>
	/// 実行ログに書き込みます。
	/// </summary>
	/// <param name="message">メッセージ</param>
	static public void LogProcess(string message) {
		using(var w = TextWriter.Synchronized(Logger.loggerProcess)) {
			w.WriteLine(
				DateTime.Now.ToString(Logger.TimeFormat) + ": " + message
			);
		}
		Debug.Log(message);
	}

	/// <summary>
	/// 結果ログに書き込みます。
	/// </summary>
	/// <param name="message">メッセージ</param>
	static public void LogResult(string message) {
		using(var w = TextWriter.Synchronized(Logger.loggerResult)) {
			w.WriteLine(
				DateTime.Now.ToString(Logger.TimeFormat) + ": " + message
			);
		}
		Debug.Log(message);
	}

}
