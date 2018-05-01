using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：アイドル状態
/// 
/// ＊ゲーム概要説明
/// ＊ブースBGMを流す
/// 
/// </summary>
public class PhaseIdle : PhaseBase {

	/// <summary>
	/// メッセージをすべて表示しきってから待機する秒数
	/// </summary>
	private const float MessageAllViewSeconds = 30.0f;

	/// <summary>
	/// テキストエリアの元文章
	/// </summary>
	private string textSource;

	/// <summary>
	/// テキストエリアの現在表示している文字列インデックス
	/// </summary>
	private int textCursorIndex;

	/// <summary>
	/// テキスト送りを開始したかどうか
	/// </summary>
	private bool isStarted;

	/// <summary>
	/// メッセージを格納するゲームオブジェクトのコンポーネント
	/// </summary>
	private UnityEngine.UI.Text textArea;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	public PhaseIdle(PhaseManager parent) : base(parent, null) {
		this.textCursorIndex = -1;
	}

	/// <summary>
	/// ゲームオブジェクトの初期化
	/// </summary>
	public override void Start() {
		this.textArea = GameObject.Find("Idle_DescriptionGameText").GetComponent<UnityEngine.UI.Text>();
		this.textSource = this.textArea.text
			.Replace("${TITLE}", PhaseManager.GameTitle);
		this.textArea.text = "";
		this.textArea.enabled = true;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		if(this.isStarted == false) {
			// メッセージ送り開始
			this.isStarted = true;
			this.parent.StartCoroutine(this.nextMessageCharacter());
		}
	}

	/// <summary>
	/// メッセージの文字を１文字進めるコルーチン
	/// </summary>
	private IEnumerator nextMessageCharacter() {
		while(true) {
			if(this.textCursorIndex + 1 > this.textSource.Length) {
				// 最後まで達したとき：指定秒だけ待機して初期化
				yield return new WaitForSeconds(PhaseIdle.MessageAllViewSeconds);
				this.textArea.text = "";
				this.textCursorIndex = -1;
			} else {
				// 先に遅延
				yield return new WaitForSeconds(PhaseManager.MessageSpeed);
			}

			// １文字進める
			this.textCursorIndex++;
			this.textArea.text = this.textSource.Substring(0, this.textCursorIndex);
		}
	}

}
