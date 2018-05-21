using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControllerB {

	/// <summary>
	/// 端末B の管理クラス
	/// </summary>
	public class ControllerB : ControllerBase {

		/// <summary>
		/// 初回処理
		/// </summary>
		public override void Start() {
			base.Start();
			this.StartNewGame();
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public override void Update() {
			base.Update();
			if(this.doneReadyGo == false) {
				this.doReadyGo();
			}
		}

		/// <summary>
		/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
		/// </summary>
		public override void StartNewGame() {
			// ミニゲームを表示
			this.activeSubGame = this.SubGames[0];
			this.activeSubGame.gameObject.SetActive(true);
			this.activeSubGame.StartSubGame();
		}

		/// <summary>
		/// Ready-Go完了後にゲームを開始させます。
		/// </summary>
		protected override void AfterHideGo() {
			base.AfterHideGo();
			this.activeSubGame.IsUpdateEnabled = true;
		}

	}

}
