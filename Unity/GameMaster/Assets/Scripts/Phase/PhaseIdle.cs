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
	public const float MessageAllViewSeconds = 30.0f;

	/// <summary>
	/// テキストエリアの元文章
	/// </summary>
	public const string TextSource = @"あなた達 ""３人"" は「" + PhaseManager.GameTitle + @"」に出場するチームメイトだ。
一人は助走をつけ、一人は人力飛行機を漕ぎ、一人はそれを手助けする。
それぞれがメチャクチャに動いて、より遠くを目指せ！";

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
	/// テキストのメッセージを進めるコルーチン
	/// </summary>
	private Coroutine messageCoroutine;

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
			this.messageCoroutine = this.parent.StartCoroutine(this.nextMessageCharacter());
		}
	}

	/// <summary>
	/// フェーズ破棄
	/// </summary>
	public override void Destroy() {
		// メッセージの文字を進めるコルーチンを止める
		this.parent.StopCoroutine(this.messageCoroutine);
	}

	/// <summary>
	/// メッセージの文字を１文字進めるコルーチン
	/// </summary>
	private IEnumerator nextMessageCharacter() {
		while(true) {
			if(this.textCursorIndex + 1 > PhaseIdle.TextSource.Length) {
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
			this.textArea.text = PhaseIdle.TextSource.Substring(0, this.textCursorIndex);
		}
	}

	/// <summary>
	/// 前のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>前のフェーズのインスタンス</returns>
	public override PhaseBase GetPreviousPhase() {
		// このフェーズが一番最初なのでこれ以上は戻せない
		return null;
	}

	/// <summary>
	/// 次のフェーズのインスタンスを生成して返します。
	/// </summary>
	/// <returns>次のフェーズのインスタンス</returns>
	public override PhaseBase GetNextPhase() {
		return new PhaseControllers(this.parent);
	}

	/// <summary>
	/// このフェーズのBGMファイル名を返します。
	/// </summary>
	/// <returns>BGMファイル名</returns>
	public override string GetBGMFileName() {
		return "Sounds/BGM/AC【タイトル】hoshinomatataki";
	}

}
