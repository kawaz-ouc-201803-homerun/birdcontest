using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ControllerC {

	/// <summary>
	/// ミニゲーム選択画面のボタンを制御するクラス
	/// </summary>
	public class SubGameButtonController : MonoBehaviour {

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
		/// ミニゲームID
		/// </summary>
		public enum SubGameId {
			None = -1,      // 選択されていない
			Human,          // 気合
			Bomb,           // 爆弾
			Wairo,          // 賄賂
		}

		/// <summary>
		/// 選択されたミニゲームのID
		/// </summary>
		static public SubGameId SelectedSubGameId = SubGameId.None;

		/// <summary>
		/// ミニゲーム説明ウィンドウの表示状態
		/// </summary>
		static public WindowState CurrentWindowState = WindowState.Neutral;

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
		/// 賄賂ADVの制御オブジェクト
		/// </summary>
		public GameObject WairoController;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Start() {
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public void Update() {
		}

		/// <summary>
		/// ミニゲーム選択ボタン および [開始＆キャンセル] ボタンの押下処理
		/// サト注記：一つのゲームオブジェクトに複数のボタンイベントの機能を持たせたため、フラグで処理を分岐させるという謎の構造になってしまっている
		/// </summary>
		/// <param name="parameter">選択肢固有のパラメーター</param>
		public void ButtonOnClick(string parameter) {
			if(SubGameDescriptionController.IsSubGameButtonClickable == true) {
				// ミニゲーム選択ボタンの押下処理
				SubGameButtonController.CurrentWindowState = WindowState.Visible;
				switch(parameter) {
					//ボタン１回目
					case "Kiai":
						SubGameButtonController.SelectedSubGameId = SubGameId.Human;
						break;
					case "Bomb":
						SubGameButtonController.SelectedSubGameId = SubGameId.Bomb;
						break;
					case "Coin":
						SubGameButtonController.SelectedSubGameId = SubGameId.Wairo;
						break;
				}
				return;
			}

			// [開始＆キャンセル] ボタンの押下処理
			switch(parameter) {
				case "No":
					// 「やっぱりやめる」
					SubGameButtonController.CurrentWindowState = WindowState.Hidden;
					SubGameDescriptionController.IsSubGameButtonClickable = true;
					foreach(GameObject obj in this.DescriptionWindow) {
						this.StartCoroutine(this.HideDescription(obj));
					}
					break;

				case "Yes":
					// 「これでいく」
					SubGameButtonController.CurrentWindowState = WindowState.Disposed;
					SubGameDescriptionController.IsSubGameButtonClickable = true;
					foreach(GameObject obj in this.DescriptionWindow) {
						this.StartCoroutine(this.HideDescription(obj));
					}
					this.StartCoroutine(this.RotationAnimation(this.SubGameSelectorObject, 2f));
					this.SubGameSelectorObject.SetActive(false);

					// TODO: ミニゲーム個別の移動処理
					if(SubGameButtonController.SelectedSubGameId == SubGameId.Wairo) {
						// 賄賂ミニゲームへ
						this.WairoController.SetActive(true);
						this.StartCoroutine(this.RotationAnimation(this.WairoController, 10f));
					}
					break;
			}
		}

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

		/// <summary>
		/// 指定したオブジェクトを90度右に回転させる演出を行うコルーチン
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

	}

}
