using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ControllerC {

	/// <summary>
	/// ミニゲームの説明ウィンドウを制御するクラス
	/// </summary>
	public class SubGameDescriptionController : MonoBehaviour {

		/// <summary>
		/// ミニゲームの説明ウィンドウの両端枠オブジェクト
		/// インスペクターにて [0]=右、[1]=左 の順に格納されます。
		/// </summary>
		public GameObject[] DescriptionWindowColumns;

		/// <summary>
		/// ミニゲームの説明ウィンドウのオブジェクト
		/// インスペクターにて、ミニゲームのID順に格納されます。
		/// </summary>
		public GameObject DescriptionWindow;

		/// <summary>
		/// ミニゲームの説明文
		/// </summary>
		public GameObject[] Descriptions;

		/// <summary>
		/// ミニゲームの説明ウィンドウの初期位置
		/// </summary>
		private Vector3[] WindowColumnStartPositions;
		
		/// <summary>
		/// ミニゲームの説明ウィンドウの終端位置
		/// </summary>
		private readonly Vector3 WindowEndPosition = new Vector3(63.5f, 0, 0);

		/// <summary>
		/// ミニゲームの [開始＆キャンセル] ボタンオブジェクト
		/// </summary>
		public GameObject YesNoButton;

		/// <summary>
		/// ミニゲームボタン押下時の処理が有効であるか否か
		/// </summary>
		static public bool IsSubGameButtonClickable = true;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Start() {
			this.transform.localScale = new Vector3(1, 0, 0);
			this.DescriptionWindow.transform.localScale = new Vector3(0, 1, 0);
			this.WindowColumnStartPositions = new Vector3[] {
				this.DescriptionWindowColumns[0].transform.position,
				this.DescriptionWindowColumns[1].transform.position,
			};
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public void Update() {
			if(SubGameDescriptionController.IsSubGameButtonClickable == false) {
				return;
			}

			// ウィンドウの表示状態に応じた処理を行う
			switch(SubGameButtonController.CurrentWindowState) {
				case SubGameButtonController.WindowState.Visible:
					SubGameDescriptionController.IsSubGameButtonClickable = false;
					this.StartCoroutine(this.ShowDescription(SubGameButtonController.SelectedSubGameId));

					// 説明ウィンドウの両端枠を表示する
					foreach(GameObject obj in this.DescriptionWindowColumns) {
						obj.SetActive(true);
					}
					break;

				case SubGameButtonController.WindowState.Hidden:
					SubGameDescriptionController.IsSubGameButtonClickable = false;
					this.StartCoroutine(this.HideDescription(SubGameButtonController.SelectedSubGameId));
					break;

				case SubGameButtonController.WindowState.Disposed:
					SubGameDescriptionController.IsSubGameButtonClickable = false;
					this.StartCoroutine(this.HideDescription(SubGameButtonController.SelectedSubGameId));
					break;
			}
		}

		/// <summary>
		/// ミニゲームの説明ウィンドウを表示するアニメーションを行うコルーチン
		/// サト注記：ゴリ押し注意
		/// </summary>
		/// <param name="targetSubGameId">対象ミニゲームの番号</param>
		public IEnumerator ShowDescription(SubGameButtonController.SubGameId targetSubGameId) {
			yield return new WaitForSeconds(0.1f);
			
			// ウィンドウの両端枠を上下方向に広げる
			while(this.transform.localScale.y < 1f) {
				this.transform.localScale += new Vector3(0, 0.2f, 0);
				yield return new WaitForEndOfFrame();
			}
			this.transform.localScale = new Vector3(1, 1, 0);
			yield return new WaitForSeconds(0.1f);

			// ウィンドウ全体を表示する
			this.DescriptionWindow.SetActive(true);

			// ウィンドウ全体を左右方向に広げる
			while(this.DescriptionWindowColumns[0].transform.position.x > this.WindowEndPosition.x) {
				this.DescriptionWindowColumns[0].transform.position += new Vector3(-70f, 0, 0);
				this.DescriptionWindowColumns[1].transform.position += new Vector3(70f, 0, 0);
				this.DescriptionWindow.transform.localScale = new Vector3(
					-(this.DescriptionWindowColumns[0].transform.position.x - this.WindowColumnStartPositions[0].x)
						/ (this.WindowEndPosition.x - this.WindowColumnStartPositions[0].x),
					1,
					0
				);
				yield return new WaitForEndOfFrame();
			}

			// 説明文を表示する
			this.Descriptions[(int)targetSubGameId].SetActive(true);
			while(this.Descriptions[(int)targetSubGameId].GetComponent<Text>().color.a < 1) {
				this.Descriptions[(int)targetSubGameId].GetComponent<Text>().color += new Color(0, 0, 0, 0.1f);
				yield return new WaitForEndOfFrame();
			}
			this.Descriptions[(int)targetSubGameId].GetComponent<Text>().color = new Color(1, 1, 1, 1);

			if(targetSubGameId == SubGameButtonController.SubGameId.Human) {
				// 気合を選択したときのみ、サブテキスト「がんばれがんばれできるできる」を表示
				var subTextObject = this.Descriptions[(int)SubGameButtonController.SubGameId.Human].transform.Find("Text").gameObject;
				subTextObject.SetActive(true);
				while(subTextObject.GetComponent<Text>().color.a < 1) {
					subTextObject.GetComponent<Text>().color += new Color(0, 0, 0, 0.5f);
					yield return new WaitForEndOfFrame();
				}
				subTextObject.GetComponent<Text>().color = new Color(1, 1, 1, 1);
			}
			yield return new WaitForSeconds(0.1f);

			// [開始＆キャンセル] ボタンを表示
			this.YesNoButton.SetActive(true);

			// ウィンドウの表示状態を更新
			SubGameButtonController.CurrentWindowState = SubGameButtonController.WindowState.Neutral;
			yield return new WaitForSeconds(0.1f);

			// 説明ウィンドウ表示中は他のミニゲームボタンを押せないようにする
			SubGameDescriptionController.IsSubGameButtonClickable = false;
		}

		/// <summary>
		/// ミニゲームの説明ウィンドウを消去するアニメーションを行うコルーチン
		/// サト注記：ゴリ押し注意
		/// </summary>
		/// <param name="targetSubGameId">対象ミニゲームの番号</param>
		public IEnumerator HideDescription(SubGameButtonController.SubGameId targetSubGameId) {
			yield return new WaitForSeconds(0.1f);

			// 先に [開始＆キャンセル] ボタンを隠す
			this.YesNoButton.SetActive(false);

			// ウィンドウ全体を左右方向に畳む
			while(this.DescriptionWindowColumns[0].transform.position.x < this.WindowColumnStartPositions[0].x) {
				this.DescriptionWindowColumns[0].transform.position += new Vector3(70f, 0, 0);
				this.DescriptionWindowColumns[1].transform.position += new Vector3(-70f, 0, 0);
				this.DescriptionWindow.transform.localScale = new Vector3(
					-(this.DescriptionWindowColumns[0].transform.position.x - this.WindowColumnStartPositions[0].x)
						/ (this.WindowEndPosition.x - this.WindowColumnStartPositions[0].x),
					1,
					0
				);
				yield return new WaitForEndOfFrame();
			}
			this.DescriptionWindowColumns[0].transform.position = this.WindowColumnStartPositions[0];
			this.DescriptionWindowColumns[1].transform.position = this.WindowColumnStartPositions[1];

			// 畳み終わったウィンドウと説明文を隠す
			this.DescriptionWindow.SetActive(false);
			this.Descriptions[(int)targetSubGameId].SetActive(false);
			yield return new WaitForSeconds(0.1f);

			// 残された両端枠をさらに上下方向に畳む
			while(this.transform.localScale.y > 0f) {
				this.transform.localScale -= new Vector3(0, 0.1f, 0);
				yield return new WaitForEndOfFrame();
			}
			
			// 説明ウィンドウの両端枠を隠す
			foreach(GameObject obj in this.DescriptionWindowColumns) {
				obj.SetActive(false);
			}

			// ウィンドウの表示状態を更新
			SubGameButtonController.CurrentWindowState = SubGameButtonController.WindowState.Neutral;
			yield return new WaitForSeconds(0.1f);

			// 完全に閉じ終わったら再びミニゲームボタンを有効にする
			SubGameDescriptionController.IsSubGameButtonClickable = true;
		}

	}

}
