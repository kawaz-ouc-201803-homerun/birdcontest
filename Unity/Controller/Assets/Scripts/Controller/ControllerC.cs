using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ControllerC {

	/// <summary>
	/// 端末C の管理クラス
	/// </summary>
	public class ControllerC : ControllerBase {

		/// <summary>
		/// 選択肢の番号
		/// </summary>
		public enum Option {
			Human,       // 気合
			Bomb,        // 爆弾
			Wairo,       // 贈賄
		}

		/// <summary>
		/// 選択肢プレハブオブジェクト
		/// </summary>
		public GameObject OtomoOptions;

		/// <summary>
		/// 開始準備完了したかどうか
		/// </summary>
		private bool isReadyForStart = false;

		/// <summary>
		/// 選択した選択肢の番号
		/// </summary>
		private int selectedOptionIndex = -1;

		/// <summary>
		/// 初回処理
		/// </summary>
		public override void Start() {
			base.Start();
			this.selectedOptionIndex = -1;
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public override void Update() {
			base.Update();

			if(this.isReadyForStart == true && this.doneReadyGo == false) {
				// Ready-Go 表示してミニゲーム開始へ
				if(this.activeSubGame != null) {
					this.activeSubGame.gameObject.SetActive(true);
				}
				this.doReadyGo();
			}
		}

		/// <summary>
		/// ゲームサイクル２周目以降に必要な初期化処理を実行します。
		/// </summary>
		public override void StartNewGame() {
			// 大友プレハブを破棄して再生成する
			Object.DestroyImmediate(this.OtomoOptions);
			this.OtomoOptions =
				Object.Instantiate(
					(GameObject)Resources.Load("Prefabs/OtomoOptionsC"),
					Vector3.zero,
					Quaternion.identity,
					this.transform.Find("Options")
				);
			this.OtomoOptions.transform.Find("SubGameSelectorController").GetComponent<ControllerCButtonController>().SEPlayer = this.SEPlayer;

			Object.DestroyImmediate(this.SubGames[(int)Option.Wairo]);
			this.SubGames[(int)Option.Wairo] =
				Object.Instantiate(
					(GameObject)Resources.Load("Prefabs/SubGames/SubGame_WairoADV"),
					Vector3.zero,
					Quaternion.identity,
					this.transform
				).GetComponent<SubGame.SubGameWairo>();
			this.SubGames[(int)Option.Wairo].GetComponent<SubGame.SubGameWairo>().SEPlayer = this.SEPlayer;

			// 画面初期化
			this.transform.Find("Options").gameObject.SetActive(true);
			foreach(var subGame in this.SubGames) {
				subGame.gameObject.SetActive(false);
			}
			this.Start();
			this.isReadyForStart = false;
		}

		/// <summary>
		/// 進捗報告として送るデータを生成します。
		/// </summary>
		/// <returns>進捗報告として送る辞書型配列</returns>
		protected override Dictionary<string, string> createProgressData() {
			var dictionary = base.createProgressData();
			dictionary["option"] = this.selectedOptionIndex.ToString();
			return dictionary;
		}

		/// <summary>
		/// ミニゲーム「贈賄」を開始
		/// </summary>
		public void StartSubGame_WairoADV() {
			this.transform.Find("Options").gameObject.SetActive(false);
			this.selectedOptionIndex = (int)Option.Wairo;
			this.activeSubGame = this.SubGames[this.selectedOptionIndex];
			this.activeSubGame.StartSubGame();
			this.isReadyForStart = true;
		}

		/// <summary>
		/// ミニゲーム「ボタン連打」を開始
		/// </summary>
		public void StartSubGame_ButtonRepeat() {
			this.transform.Find("Options").gameObject.SetActive(false);
			this.selectedOptionIndex = (int)Option.Human;
			this.activeSubGame = this.SubGames[this.selectedOptionIndex];
			this.activeSubGame.StartSubGame();
			this.isReadyForStart = true;
		}

		/// <summary>
		/// ミニゲーム「３ボタン連続押し」を開始
		/// </summary>
		public void StartSubGame_PushButtons() {
			this.transform.Find("Options").gameObject.SetActive(false);
			this.selectedOptionIndex = (int)Option.Bomb;
			this.activeSubGame = this.SubGames[this.selectedOptionIndex];
			this.activeSubGame.StartSubGame();
			this.isReadyForStart = true;
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
