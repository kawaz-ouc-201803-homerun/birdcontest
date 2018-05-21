using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ControllerA {

	/// <summary>
	/// 端末A のミニゲーム選択ボタン制御
	/// </summary>
	public class ControllerAButtonController : OtomoOptions.OptionButtonController {

		/// <summary>
		/// 指定したキーワードに基づいて選択中のミニゲームのIDを変更します。
		/// </summary>
		/// <param name="keyword">ミニゲーム固有のキーワード</param>
		protected override void setSelectedSubGameId(string keyword) {
			switch(keyword) {
				case "Human":
					this.SelectedSubGameId = (int)ControllerA.Option.Human;
					break;

				case "Bomb":
					this.SelectedSubGameId = (int)ControllerA.Option.Bomb;
					break;

				case "Car":
					this.SelectedSubGameId = (int)ControllerA.Option.Car;
					break;
			}
		}

		/// <summary>
		/// 指定したミニゲームを開始します。
		/// </summary>
		/// <param name="subGameId">ミニゲームID</param>
		protected override void StartSubGame(int subGameId) {
			switch(subGameId) {
				case (int)ControllerA.Option.Human:
					this.transform.GetComponentInParent<ControllerA>().StartSubGame_ButtonRepeat();
					break;

				case (int)ControllerA.Option.Bomb:
					this.transform.GetComponentInParent<ControllerA>().StartSubGame_PushButtons();
					break;

				case (int)ControllerA.Option.Car:
					this.transform.GetComponentInParent<ControllerA>().StartSubGame_MeterStop();
					break;
			}
		}
	}

}
