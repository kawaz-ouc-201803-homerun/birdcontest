using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// オーディエンス投票システムのテスター（ゲームマスター用）
/// </summary>
public class TesterAudience : MonoBehaviour {

	/// <summary>
	/// 通信機能
	/// </summary>
	protected NetworkConnector connector;

	/// <summary>
	/// イベントID
	/// </summary>
	protected string eventId;

	/// <summary>
	/// 初期設定
	/// </summary>
	public void Start() {
		this.connector = new NetworkGameMaster(null);

		// 以下のボタンはイベントを作成しないと使えないようにする
		GameObject.Find("Button_CloseEvent").GetComponent<Button>().interactable = false;
		GameObject.Find("Button_GetPredicts").GetComponent<Button>().interactable = false;
	}

	/// <summary>
	/// ボタン押下：新規イベント生成
	/// </summary>
	public void OnNewEvent() {
		var result = (this.connector as NetworkGameMaster).StartAudiencePredicts();
		if(string.IsNullOrEmpty(result) == false) {
			Logger.LogProcess("新規イベント作成: ID=" + result);
			Logger.LogResult("成功: オーディエンス投票システム: 新規イベント作成");

			// イベントを使ったAPIを使えるようにする
			this.eventId = result;
			GameObject.Find("Button_CloseEvent").GetComponent<Button>().interactable = true;
			GameObject.Find("Button_GetPredicts").GetComponent<Button>().interactable = true;

		} else {
			Logger.LogResult("失敗: オーディエンス投票システム: 新規イベント作成");
		}
	}

	/// <summary>
	/// ボタン押下：イベント締切
	/// </summary>
	public void OnCloseEvent() {
		var result = (this.connector as NetworkGameMaster).CloseAudiencePredicts(this.eventId);
		if(result == System.Net.HttpStatusCode.OK) {
			Logger.LogProcess("イベント締切: ID=" + result);
			Logger.LogResult("成功: オーディエンス投票システム: イベント締切");

			// イベントを使ったAPIを使えなくする
			GameObject.Find("Button_CloseEvent").GetComponent<Button>().interactable = false;
			GameObject.Find("Button_GetPredicts").GetComponent<Button>().interactable = false;

		} else {
			Logger.LogResult("失敗: オーディエンス投票システム: イベント締切");
		}
	}

	/// <summary>
	/// ボタン押下：予想データ取り出し
	/// </summary>
	public void OnGetPredicts() {
		var result = (this.connector as NetworkGameMaster).GetAudiencePredicts(this.eventId);
		if(result != null) {
			Logger.LogProcess("イベント予想一覧: " + result.GetJSON());
			Logger.LogResult("成功: オーディエンス投票システム: イベント予想取得");
		} else {
			Logger.LogResult("失敗: オーディエンス投票システム: イベント予想取得");
		}
	}

	/// <summary>
	/// ボタン押下：参加延べ人数取得
	/// </summary>
	public void OnGetPeopleCount() {
		var result = (this.connector as NetworkGameMaster).GetPeopleCount();
		Logger.LogProcess("イベント参加延べ人数: " + result + " 人");
		Logger.LogResult("成功: オーディエンス投票システム: 参加延べ人数取得");
	}

}
