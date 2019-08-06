using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ControllerC {

	/// <summary>
	/// 端末C のミニゲーム選択ボタン制御
	/// </summary>
	public class ControllerCButtonController : OtomoOptions.OptionButtonController {

		/// <summary>
		/// 指定したキーワードに基づいて選択中のミニゲームのIDを変更します。
		/// </summary>
		/// <param name="keyword">ミニゲーム固有のキーワード</param>
		protected override void setSelectedSubGameId(string keyword) {
			switch(keyword) {
				case "Human":
					this.SelectedSubGameId = (int)ControllerC.Option.Human;
					break;

				case "Bomb":
					this.SelectedSubGameId = (int)ControllerC.Option.Bomb;
					break;

				case "Wairo":
					this.SelectedSubGameId = (int)ControllerC.Option.Wairo;
					break;
			}
		}

		/// <summary>
		/// 指定したミニゲームを開始します。
		/// </summary>
		/// <param name="subGameId">ミニゲームID</param>
		protected override void StartSubGame(int subGameId) {
			switch(subGameId) {
				case (int)ControllerC.Option.Human:
					this.transform.GetComponentInParent<ControllerC>().StartSubGame_ButtonRepeat();
					break;

				case (int)ControllerC.Option.Bomb:
					this.transform.GetComponentInParent<ControllerC>().StartSubGame_PushButtons();
					break;

				case (int)ControllerC.Option.Wairo:
					this.transform.GetComponentInParent<ControllerC>().StartSubGame_WairoADV();
					break;
			}
		}
	}

}
