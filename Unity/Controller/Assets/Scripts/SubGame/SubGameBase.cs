using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubGame {

	/// <summary>
	/// 各種ミニゲームの基底クラス
	/// </summary>
	public abstract class SubGameBase : MonoBehaviour {

		/// <summary>
		/// Updateを有効にするかどうか
		/// </summary>
		public bool IsUpdateEnabled {
			get; set;
		}

		/// <summary>
		/// 初回処理
		/// </summary>
		public void Start() {
			// デフォルトでUpdateを無効にする
			this.IsUpdateEnabled = false;
			this.startSubGame();
		}

		/// <summary>
		/// 毎フレーム更新処理
		/// </summary>
		public void Update() {
			if(this.IsUpdateEnabled == true) {
				this.updateSubGame();
			}
		}

		/// <summary>
		/// 初回処理の実体
		/// </summary>
		protected abstract void startSubGame();

		/// <summary>
		/// 毎フレーム更新処理の実体
		/// </summary>
		protected abstract void updateSubGame();

		/// <summary>
		/// 進捗報告として送るデータを辞書型配列にセットします。
		/// </summary>
		/// <param name="dictionary">セットする対象の辞書型配列</param>
		public abstract void SetProgressData(ref Dictionary<string, string> dictionary);

		/// <summary>
		/// 画面に表示するミニゲーム結果をテキストで返します。
		/// </summary>
		/// <returns>ミニゲーム結果テキスト</returns>
		public abstract string GetResultText();

	}

}
