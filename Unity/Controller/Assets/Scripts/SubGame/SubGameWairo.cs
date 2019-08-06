using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubGame {

	/// <summary>
	/// 賄賂ミニゲーム
	/// </summary>
	public class SubGameWairo : SubGameBase {

		/// <summary>
		/// 立ち絵のキャラクターID
		/// </summary>
		public enum CharacterSide {
			Organizer,          // 主催者
			Sapphiartchan,      // サファイアートちゃん
		}

		/// <summary>
		/// 表示させるメッセージのソース群
		/// Multilineでエディター内のテキストボックスを複数行対応にしています。
		/// </summary>
		[Multiline(3)]
		public string[] WairoTextSources;

		/// <summary>
		/// 選択肢のパターン群
		/// Multilineでエディター内のテキストボックスを複数行対応にしています。
		/// </summary>
		[Multiline(2)]
		public string[] WairoOptionSources;

		/// <summary>
		/// テキストボックスのオブジェクト
		/// </summary>
		public GameObject WairoTextBox;

		/// <summary>
		/// テキストウィンドウのオブジェクト
		/// </summary>
		public GameObject WairoTextWindow;

		/// <summary>
		/// テキストボックスのテキストコンポーネント
		/// </summary>
		private Text wairoText;

		/// <summary>
		/// テキストボックスのアニメーターコンポーネント
		/// </summary>
		private Animator textAnimator;

		/// <summary>
		/// テキストを送れる状態か否か
		/// </summary>
		private bool isTextable = false;

		/// <summary>
		/// 選択肢の番号（1・10・100）
		/// </summary>
		private int quizNum = 0;

		/// <summary>
		/// シナリオ分岐用フラグ
		/// </summary>
		private int wairoMultiFlag = 0;

		/// <summary>
		/// 対GM報告用：交渉により得たポイント
		/// </summary>
		private int wairoScore = 1;

		/// <summary>
		/// 交渉により得られるポイントの最大値
		/// </summary>
		public const int WairoScoreMax = 4;

		/// <summary>
		/// 対GM報告用：選択肢いま何問目？
		/// すべて完了したら選択肢の数＋１の状態にします。
		/// </summary>
		private int wairoStep = 0;

		/// <summary>
		/// 主催者の立ち絵グラフィック
		/// </summary>
		public GameObject Organizer;

		/// <summary>
		/// サファイアートちゃんの立ち絵グラフィック
		/// </summary>
		public GameObject Sapphiartchan;

		/// <summary>
		/// 主催者の立ち絵グラフィックのスプライトパターン集
		/// </summary>
		private Sprite[] organizerSprites;

		/// <summary>
		/// 選択肢のボタンオブジェクト
		/// </summary>
		public GameObject WairoOptionButton;

		/// <summary>
		/// 選択肢のテキストコンポーネント群
		/// </summary>
		public Text[] WairoOptionText;

		/// <summary>
		/// SE再生制御オブジェクト
		/// </summary>
		public SEPlayer SEPlayer;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public override void StartSubGame() {
			base.StartSubGame();
			Debug.Log("Wairo初期化");

			// 変数を初期化
			this.isTextable = false;
			this.quizNum = 0;
			this.wairoMultiFlag = 0;
			this.wairoScore = 1;
			this.wairoStep = 0;

			// ウィンドウをアニメーションしながら表示する
			this.gameObject.SetActive(true);
			this.StartCoroutine(this.RotationAnimation(this.gameObject, 10f));

			// 各種オブジェクトやコンポーネント、スプライトを取得して保管しておく
			this.wairoText = this.WairoTextBox.GetComponent<Text>();
			this.wairoText.text = "";

			this.textAnimator = this.WairoTextBox.GetComponent<Animator>();

			this.organizerSprites = Resources.LoadAll<Sprite>("manager1");
			this.Organizer.GetComponent<Image>().sprite = System.Array.Find(
				this.organizerSprites,
				(sprite) => sprite.name.Equals("manager1_0")
			);
			this.Sapphiartchan.GetComponent<Image>().sprite =
				Resources.LoadAll<Sprite>("Sapphiartchan_Nuetral")[0];

			// メインルーチンを開始
			this.StartCoroutine(this.ShowMessageWithAnimation(0.1f));
			this.StartCoroutine(this.WairoMainRoutine());
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		protected override void updateSubGame() {
			// テキスト送りの動作が行われたかどうかを判定する
			var isButtonDown = (
				Input.GetMouseButtonDown(0) == true
				|| Input.GetKeyDown(KeyCode.Z) == true
			);
			if(this.isTextable == false && this.quizNum == 0 && isButtonDown == true) {
				this.isTextable = true;
				this.SEPlayer.PlaySE((int)SEPlayer.SEID.PushButton);
			}
		}

		/// <summary>
		/// 選択肢を選んだときの処理
		/// </summary>
		/// <param name="parameter">選択肢固有のパラメーター。0=スコア変動なし、1=スコア加算</param>
		public void ButtonOnClick(int parameter) {
			this.SEPlayer.PlaySE((int)SEPlayer.SEID.Decision);
			this.WairoOptionButton.SetActive(false);

			if(this.quizNum == 0) {
				// 回答に対して行うべき処理がない状態
				return;
			}

			switch(parameter) {
				case 0:
					this.wairoScore += 0;
					this.wairoMultiFlag += 0;
					break;

				case 1:
					this.wairoScore += 1;
					this.wairoMultiFlag += this.quizNum;
					break;
			}

			this.quizNum = 0;
			this.isTextable = true;
		}

		/// <summary>
		/// 賄賂ミニゲームのメインルーチン
		/// サト注記：ゴリ押し注意。このコルーチンだけですべてのメッセージ＆グラフィックの制御を行っている
		/// </summary>
		public IEnumerator WairoMainRoutine() {
			// Ready-Go 表示完了待ち
			yield return new WaitForSeconds(4.0f);

			// 最初のメッセージを表示＆待機
			this.StartCoroutine(this.NextToWairoMessage(0));
			yield return new WaitUntil(() => this.isTextable);

			// サファイアートちゃんと主催者を水平移動で表示
			while(this.Organizer.transform.localPosition.x > 250f) {
				this.Organizer.transform.localPosition += new Vector3(-20f, 0, 0);
				this.Organizer.GetComponent<Image>().color += new Color(0, 0, 0, 0.04f);
				this.Sapphiartchan.transform.localPosition += new Vector3(20f, 0, 0);
				this.Sapphiartchan.GetComponent<Image>().color += new Color(0, 0, 0, 0.02f);
				yield return new WaitForEndOfFrame();
			}
			this.Organizer.transform.localPosition = new Vector3(250f, 0, 0);
			this.Sapphiartchan.transform.localPosition = new Vector3(-250f, 0, 0);

			// メッセージ送り＆待機
			this.focusCharacter(CharacterSide.Organizer);
			this.StartCoroutine(this.NextToWairoMessage(1));
			yield return new WaitUntil(() => this.isTextable);

			// 次のメッセージ送り
			this.focusCharacter(CharacterSide.Sapphiartchan);
			this.StartCoroutine(this.NextToWairoMessage(2));

			// 選択肢 [1] ＆ 待機
			this.quizNum = 1;
			this.WairoOptionText[0].text = this.WairoOptionSources[0];
			this.WairoOptionText[1].text = this.WairoOptionSources[1];
			this.WairoOptionButton.SetActive(true);
			yield return new WaitUntil(() => this.isTextable);
			this.wairoStep += 1;

			// 選択肢の結果に応じてシナリオ分岐 [1]
			switch(this.wairoMultiFlag) {
				case 0:
					this.focusCharacter(CharacterSide.Organizer);
					this.StartCoroutine(this.NextToWairoMessage(3));
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(this.NextToWairoMessage(4));
					break;

				case 1:
					this.focusCharacter(CharacterSide.Organizer);
					this.StartCoroutine(this.NextToWairoMessage(5));
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(this.NextToWairoMessage(6));
					break;
			}
			// メッセージ送り待機
			yield return new WaitUntil(() => this.isTextable);

			// 次の共通メッセージを表示＆待機
			this.StartCoroutine(this.NextToWairoMessage(7));
			yield return new WaitUntil(() => this.isTextable);

			// 次のメッセージ送り
			this.StartCoroutine(this.NextToWairoMessage(8));

			// 選択肢 [2] ＆ 待機
			this.quizNum = 10;
			this.WairoOptionText[0].text = this.WairoOptionSources[2];
			this.WairoOptionText[1].text = this.WairoOptionSources[3];
			this.WairoOptionButton.SetActive(true);
			yield return new WaitUntil(() => this.isTextable);
			this.wairoStep += 1;

			// 選択肢の結果に応じてシナリオ分岐 [2]
			switch(this.wairoMultiFlag / 10) {
				case 0:
					this.focusCharacter(CharacterSide.Organizer);
					this.StartCoroutine(this.NextToWairoMessage(9));
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(this.NextToWairoMessage(10));
					break;

				case 1:
					this.focusCharacter(CharacterSide.Organizer);

					// 主催者グラフィック切り替え
					this.Organizer.GetComponent<Image>().sprite = System.Array.Find(
						this.organizerSprites,
						(sprite) => sprite.name.Equals("manager1_1")
					);

					// メッセージエフェクト（揺らす）
					this.textAnimator.SetTrigger("isTremble");
					this.StartCoroutine(this.NextToWairoMessage(11));
					yield return new WaitForSeconds(1f);
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(NextToWairoMessage(12));
					break;
			}
			yield return new WaitUntil(() => this.isTextable);

			// 次の共通メッセージを表示＆待機
			this.StartCoroutine(this.NextToWairoMessage(13));
			yield return new WaitUntil(() => this.isTextable);

			// 次のメッセージ送り
			this.StartCoroutine(this.NextToWairoMessage(14));

			// 選択肢 [3] ＆ 待機
			this.quizNum = 100;
			this.WairoOptionText[0].text = this.WairoOptionSources[4];
			this.WairoOptionText[1].text = this.WairoOptionSources[5];
			this.WairoOptionButton.SetActive(true);
			yield return new WaitUntil(() => this.isTextable);
			this.wairoStep += 1;

			// 選択肢の結果に応じてシナリオ分岐 [3]
			switch(this.wairoMultiFlag / 100) {
				case 0:
					this.focusCharacter(CharacterSide.Organizer);
					this.StartCoroutine(this.NextToWairoMessage(15));
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(this.NextToWairoMessage(16));
					break;

				case 1:
					this.focusCharacter(CharacterSide.Organizer);

					// 主催者グラフィック切り替え
					this.Organizer.GetComponent<Image>().sprite = System.Array.Find(
						this.organizerSprites,
						(sprite) => sprite.name.Equals("manager1_1")
					);

					// メッセージエフェクト（揺らす）
					this.textAnimator.SetTrigger("isTremble");
					this.StartCoroutine(this.NextToWairoMessage(17));
					yield return new WaitForSeconds(1f);
					yield return new WaitUntil(() => this.isTextable);

					this.focusCharacter(CharacterSide.Sapphiartchan);
					this.StartCoroutine(this.NextToWairoMessage(18));
					break;
			}
			yield return new WaitUntil(() => this.isTextable);

			// 最後のメッセージを表示
			this.StartCoroutine(this.NextToWairoMessage(19));

			// サファイアートちゃんのグラフィック切り替え＆待機
			this.Sapphiartchan.GetComponent<Image>().sprite =
				Resources.LoadAll<Sprite>("Sapphiartchan_WinPose")[0];
			yield return new WaitUntil(() => this.isTextable);

			// サファイアートちゃんと主催者を水平移動で隠す
			while(this.Organizer.transform.localPosition.x < 1000f) {
				this.Organizer.transform.localPosition += new Vector3(20f, 0, 0);
				this.Organizer.GetComponent<Image>().color -= new Color(0, 0, 0, 0.04f);
				this.Sapphiartchan.transform.localPosition += new Vector3(-20f, 0, 0);
				this.Sapphiartchan.GetComponent<Image>().color -= new Color(0, 0, 0, 0.02f);
				yield return new WaitForEndOfFrame();
			}
			this.Organizer.transform.localPosition = new Vector3(1000f, 0, 0);
			this.Sapphiartchan.transform.localPosition = new Vector3(-1000f, 0, 0);
			yield return new WaitForSeconds(0.5f);

			// ウィンドウを隠す
			while(this.WairoTextWindow.transform.lossyScale.x > 0) {
				this.WairoTextWindow.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(0.5f);

			// 賄賂ADV終了
			this.wairoStep += 1;
			this.WairoTextWindow.SetActive(false);

			// タイマーを強制的にゼロにする
			this.gameObject.SetActive(false);
			GameObject.Find("Timer").GetComponent<Timer>().StopTimer(true);
		}

		/// <summary>
		/// 指定した話し手の方にフォーカスを当てる
		/// </summary>
		/// <param name="characterSide">フォーカスを当てるキャラクター</param>
		private void focusCharacter(CharacterSide characterSide) {
			this.Organizer.GetComponent<Image>().color = new Color(1, 1, 1, 
				(characterSide == CharacterSide.Organizer ? 1 : 0.5f)
			);
			this.Sapphiartchan.GetComponent<Image>().color = new Color(1, 1, 1,
				(characterSide == CharacterSide.Sapphiartchan ? 1 : 0.5f)
			);
		}

		/// <summary>
		/// 次のメッセージを表示するコルーチン
		/// </summary>
		/// <param name="textSourceIndex">メッセージソースのインデックス</param>
		public IEnumerator NextToWairoMessage(int textSourceIndex) {
			this.isTextable = false;
			this.wairoText.text = this.WairoTextSources[textSourceIndex];
			yield return new WaitForSeconds(1.0f);
		}

		/// <summary>
		/// テキストをアニメーションしながら表示するコルーチン
		/// </summary>
		/// <param name="speed">アニメーション速度</param>
		public IEnumerator ShowMessageWithAnimation(float speed) {
			while(this.WairoTextWindow.transform.localScale.y < 1) {
				this.WairoTextWindow.transform.localScale += new Vector3(0, speed, 0);
				yield return new WaitForEndOfFrame();
			}
			this.WairoTextWindow.transform.localScale = new Vector3(1, 1, 0);
		}

		/// <summary>
		/// 指定したオブジェクトを90度左右に回転させる演出を行うコルーチン
		/// </summary>
		/// <param name="targetObject">対象ゲームオブジェクト</param>
		/// <param name="speed">回転速度</param>
		public IEnumerator RotationAnimation(GameObject targetObject, float speed) {
			float angle = 90;
			while(angle > 0 && angle < 180) {
				targetObject.transform.rotation = Quaternion.Euler(0, 0, angle);
				angle -= speed;
				yield return new WaitForEndOfFrame();
			}
			targetObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		}

		/// <summary>
		/// 画面に表示するミニゲーム結果をテキストで返します。
		/// </summary>
		/// <returns>ミニゲーム結果テキスト</returns>
		public override string GetResultText() {
			return "贈賄達成率 ＝ " + (int)(this.wairoScore / (float)SubGameWairo.WairoScoreMax * 100f) + " ％";
		}

		/// <summary>
		/// 進捗報告として送るデータを辞書型配列にセットします。
		/// </summary>
		public override void SetProgressData(ref Dictionary<string, string> dictionary) {
			dictionary["param"] = this.wairoScore.ToString();
			dictionary["step"] = this.wairoStep.ToString();
		}

	}

}
