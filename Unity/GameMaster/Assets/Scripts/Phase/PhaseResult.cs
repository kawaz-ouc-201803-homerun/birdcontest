using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェーズ：結果表示
/// 
/// ＊スコア表示
/// ＊メッセージ表示
/// 
/// </summary>
public class PhaseResult : PhaseBase {

	/// <summary>
	/// 日時文字列の書式
	/// </summary>
	private const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

	/// <summary>
	/// ランキングの距離差分書式
	/// </summary>
	private const string RankingScoreFormat = "+0.00;-0.00;";

	/// <summary>
	/// ニアピン賞を採用する最低参加人数
	/// </summary>
	private const int AvailableNearpinMinCount = 3;

	/// <summary>
	/// ランキングの自動スクロールにかける秒数
	/// </summary>
	private const float RankingScrollTimeSecond = 10.0f;

	/// <summary>
	/// ニアピン賞のテキスト
	/// </summary>
	private string nearpinText;

	/// <summary>
	/// ランキングエリアのテキスト
	/// </summary>
	private string rankingText;

	/// <summary>
	/// 通信システム
	/// </summary>
	private NetworkGameMaster connector;

	/// <summary>
	/// オーディエンス投票ランキング
	/// </summary>
	private List<ModelAudiencePredict> ranking;

	/// <summary>
	/// マルチスレッドで使用するための乱数発生器
	/// </summary>
	private System.Random rand;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	/// <param name="param">[0]=イベントID, [1]=飛距離</param>
	public PhaseResult(PhaseManager parent, object[] parameters) : base(parent, parameters) {
		this.connector = new NetworkGameMaster(PhaseControllers.ControllerIPAddresses);
		this.rand = new System.Random();
	}

	/// <summary>
	/// ゲームオブジェクト初期化
	/// </summary>
	public override void Start() {
		// 飛距離
		GameObject.Find("Result_ScoreText").GetComponent<UnityEngine.UI.Text>().text =
			"RESULT ... " + string.Format("{0:0.00} m", (float)this.parameters[1]);

		// オーディエンス情報
		var nearpin = GameObject.Find("Result_NearpinText").GetComponent<UnityEngine.UI.Text>();
		nearpin.text = "";
		var ranking = GameObject.Find("Result_RankingText").GetComponent<UnityEngine.UI.Text>();
		ranking.text = "";

		// 非同期でオーディエンス投票データを取り出す
		this.connector.GetAudiencePredicts((string)this.parameters[0], new System.Action<ModelAudiencePredictsResponse>((result) => {
			Debug.Log("オーディエンス投票データ取得OK");

			// TODO: デバッグ用
			for(int i = 0; i < 10; i++) {
				result.audiencePredicts.Add(new ModelAudiencePredict() {
					nickname = "SAT",
					predict = this.rand.Next(115, 125),
					receiveTime = "2018/04/01 " + this.rand.Next(0, 24).ToString("00") + ":" + this.rand.Next(0, 60).ToString("00") + ":" + this.rand.Next(0, 60).ToString("00")
				});
			}

			// 順位データ整理：最も近い順、投稿早い順
			float score = (float)this.parameters[1];
			result.audiencePredicts.Sort((x, y) => {
				if(Mathf.Abs(x.predict) != Mathf.Abs(y.predict)) {
					// 結果値からの予想値の距離が双方で異なる場合は距離基準で見る
					float x_delta = Mathf.Abs(x.predict - score);
					float y_delta = Mathf.Abs(y.predict - score);
					return ((int)Mathf.RoundToInt(x_delta)) - ((int)Mathf.Round(y_delta));
				}

				// 投稿時刻基準で見る
				var x_time = DateTime.ParseExact(x.receiveTime, PhaseResult.DateTimeFormat, null);
				var y_time = DateTime.ParseExact(y.receiveTime, PhaseResult.DateTimeFormat, null);
				return DateTime.Compare(x_time, y_time);
			});

			if(result.audiencePredicts.Count >= PhaseResult.AvailableNearpinMinCount) {
				// ニアピン賞を出すために必要な人数要件を満たしている場合のみ表示
				this.nearpinText = "ニアピン賞 ... "
					+ result.audiencePredicts[0].nickname + " さん ("
					+ (result.audiencePredicts[0].predict - score).ToString(PhaseResult.RankingScoreFormat)
					+ " m)";
			} else {
				this.nearpinText = "ニアピン賞 ... 今回はありません";
			}

			// ランキング作成
			using(var buf = new System.IO.StringWriter()) {
				int rank = 1;
				foreach(var predict in result.audiencePredicts) {
					buf.WriteLine(rank.ToString().PadLeft(3) + " 位 : "
						+ predict.nickname + " さん ("
						+ predict.predict + " m / "
						+ (predict.predict - score).ToString(PhaseResult.RankingScoreFormat) + " m / "
						+ predict.receiveTime.Substring(11) + ")");
					rank++;
				}

				this.rankingText = buf.ToString();
			}
		}));

		// ランキングの自動スクローラーを起動
		this.parent.StartCoroutine(this.rankingScroller());
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		if(string.IsNullOrEmpty(this.nearpinText) == false) {
			// ニアピン賞のテキスト更新
			GameObject.Find("Result_NearpinText").GetComponent<UnityEngine.UI.Text>().text = this.nearpinText;
		}

		if(string.IsNullOrEmpty(this.rankingText) == false) {
			// ランキングのテキスト更新
			GameObject.Find("Result_RankingText").GetComponent<UnityEngine.UI.Text>().text = this.rankingText;
		}
	}

	/// <summary>
	/// ランキングを自動でスクロールするコルーチン
	/// </summary>
	private IEnumerator rankingScroller() {
		while(true) {
			if(string.IsNullOrEmpty(this.rankingText) == true) {
				// ランキング情報が出ていないときはまだ始めない
				yield return new WaitForSeconds(0.5f);
			}

			yield return new WaitForSeconds(PhaseResult.RankingScrollTimeSecond / 2.0f);

			// 上から下へスクロール
			var scrollbar = GameObject.Find("Result_RankingWindow").transform.Find("Scrollbar Vertical");
			iTween.ValueTo(
				scrollbar.gameObject,
				iTween.Hash(
					"from", 1f,
					"to", 0f,
					"time", 10.0f,
					"easeType", iTween.EaseType.linear,
					"onupdate", new Action<object>((value) => {
						scrollbar.GetComponent<UnityEngine.UI.Scrollbar>().value = (float)value;
					})
				)
			);
			yield return new WaitForSeconds(PhaseResult.RankingScrollTimeSecond);

			// スクロール完了後のウェイト
			yield return new WaitForSeconds(PhaseResult.RankingScrollTimeSecond);

			// 一番上に戻す
			scrollbar.GetComponent<UnityEngine.UI.Scrollbar>().value = 1f;
		}
	}

}
