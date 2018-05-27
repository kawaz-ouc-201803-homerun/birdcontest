using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OtomoOptions {

	/// <summary>
	/// ミニゲーム選択画面のボタンを制御するクラス
	/// </summary>
	public abstract class OptionButtonController : MonoBehaviour {

		/// <summary>
		/// 説明ウィンドウの表示状態
		/// </summary>
		public enum WindowState {
			Neutral,        // 中立
			Visible,        // 出す
			Hidden,         // 一時的に消す
			Disposed,       // 永遠に消す
		}

		/// <summary>
		/// 選択されたミニゲームのID
		/// </summary>
		public int SelectedSubGameId = -1;

		/// <summary>
		/// ミニゲーム説明ウィンドウの表示状態
		/// </summary>
		public WindowState CurrentWindowState = WindowState.Neutral;

		/// <summary>
		/// ミニゲーム説明ウィンドウ
		/// </summary>
		public GameObject[] DescriptionWindow;

		/// <summary>
		/// ミニゲームの [開始＆キャンセル] ボタンオブジェクト
		/// </summary>
		public GameObject SubGameYesNoButton;

		/// <summary>
		/// ミニゲーム選択画面のオブジェクト
		/// </summary>
		public GameObject SubGameSelectorObject;

		/// <summary>
		/// 連動するミニゲーム説明のコントローラー
		/// </summary>
		public OptionDescriptionController descriptionController;

		/// <summary>
		/// SE再生制御オブジェクト
		/// </summary>
		public SEPlayer SEPlayer;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public virtual void Start() {
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public virtual void Update() {
		}

		/// <summary>
		/// ミニゲーム選択ボタン および [開始＆キャンセル] ボタンの押下処理
		/// サト注記：一つのゲームオブジェクトに複数のボタンイベントの機能を持たせたため、フラグで処理を分岐させるという謎の構造になってしまっている
		/// </summary>
		/// <param name="parameter">選択肢固有のパラメーター</param>
		public void ButtonOnClick(string parameter) {
			if(this.descriptionController.IsSubGameButtonClickable == true) {
				// ミニゲーム選択ボタンの押下処理
				this.SEPlayer.PlaySE((int)SEPlayer.SEID.Decision);
				this.CurrentWindowState = WindowState.Visible;
				this.setSelectedSubGameId(parameter);
				return;
			}

			// [開始＆キャンセル] ボタンの押下処理
			switch(parameter) {
				case "No":
					// 「やっぱりやめる」
					this.SEPlayer.PlaySE((int)SEPlayer.SEID.Cancel);
					this.CurrentWindowState = WindowState.Hidden;
					this.descriptionController.IsSubGameButtonClickable = true;
					foreach(GameObject obj in this.DescriptionWindow) {
						this.StartCoroutine(this.HideDescription(obj));
					}
					break;

				case "Yes":
					// 「これでいく」
					this.SEPlayer.PlaySE((int)SEPlayer.SEID.Decision);
					this.CurrentWindowState = WindowState.Disposed;
					this.descriptionController.IsSubGameButtonClickable = true;
					foreach(GameObject obj in this.DescriptionWindow) {
						this.StartCoroutine(this.HideDescription(obj));
					}
					this.SubGameSelectorObject.SetActive(false);

					// ミニゲーム起動処理
					this.StartSubGame(this.SelectedSubGameId);
					break;
			}
		}

		/// <summary>
		/// 指定したキーワードに基づいて選択中のミニゲームのIDを変更します。
		/// </summary>
		/// <param name="keyword">ミニゲーム固有のキーワード</param>
		protected abstract void setSelectedSubGameId(string keyword);

		/// <summary>
		/// 指定したミニゲームを開始します。
		/// </summary>
		/// <param name="subGameId">ミニゲームID</param>
		protected abstract void StartSubGame(int subGameId);

		/// <summary>
		/// ミニゲーム説明のテキストをフェードアウトするコルーチン
		/// </summary>
		/// <param name="targetObject">対象ゲームオブジェクト</param>
		public IEnumerator HideDescription(GameObject targetObject) {
			yield return new WaitForSeconds(0.1f);

			// 先にボタンを消す
			this.SubGameYesNoButton.SetActive(false);

			// アルファ値を徐々に変更していくことでフェードを行う
			for(float f = 1; f > 0; f -= 0.1f) {
				Color c = targetObject.GetComponent<Text>().color;
				c.a = f;
				targetObject.GetComponent<Text>().color = c;
				yield return new WaitForEndOfFrame();
			}

			// 完全消去
			targetObject.GetComponent<Text>().color = new Color(1, 1, 1, 0);
			targetObject.SetActive(false);
		}

	}

}
