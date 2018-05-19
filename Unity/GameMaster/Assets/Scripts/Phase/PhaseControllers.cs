﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// フェーズ：各端末操作
/// 
/// ＊スクリーン上で実況する（実況メッセージは余力あれば考えてもらう）
/// ＊各端末を操作する人は、スクリーンを見えないように配置する
/// ＊選んだ選択肢と、アクション内容の進捗を定期的にゲームマスターへ報告してもらう
/// 
/// </summary>
public class PhaseControllers : PhaseBase {

	/// <summary>
	/// メーター順序
	/// </summary>
	public enum PowerMeter {
		StartPower,
		FlightPower,
		LackPower,
		Count,
	}

	/// <summary>
	/// 端末A：選択肢の番号
	/// </summary>
	public enum OptionA {
		Car,        // 自動車による牽引
		Human,      // 人力手押し
		Bomb,       // 爆弾
	}

	/// <summary>
	/// 端末C：選択肢の番号
	/// </summary>
	public enum OptionC {
		Human,       // 気合
		Bomb,        // 爆弾
		Wairo,       // 贈賄
	}

	/// <summary>
	/// 賄賂ミニゲームのステップ数
	/// </summary>
	public const int WairoStepLength = 4;

	/// <summary>
	/// 賄賂ミニゲームの最大スコア
	/// </summary>
	public const int WairoScoreMax = 3;

	/// <summary>
	/// 操作端末のIPアドレス
	/// </summary>
	public static string[] ControllerIPAddresses = new string[] {
#if UNITY_EDITOR
		"127.0.0.1",
		"127.0.0.1",
		"127.0.0.1",
#else
		// GM開始時に設定を動的に変える
		"192.168.11.11",
		"192.168.11.12",
		"192.168.11.13",
#endif
	};

	/// <summary>
	/// オーディエンス投票を監視する間隔秒数
	/// </summary>
	private const float AudienceWatchSeconds = 5.0f;

	/// <summary>
	/// オーディエンス投票を締め切る直前の待機秒数
	/// </summary>
	private const int ClosingAudienceWaitSeconds = 30;

	/// <summary>
	/// 操作端末に与えられるゲーム時間
	/// </summary>
	private const int ControllerLimitSeconds = 30;

	/// <summary>
	/// 各端末の進捗状況を更新する間隔秒数
	/// </summary>
	private const float ControllerProgressRefreshSeconds = 2.0f;

	/// <summary>
	/// 実況の A-C、B 表示を切り替える間隔秒数
	/// </summary>
	private const float StreamTextChangeSeconds = 5.0f;

	/// <summary>
	/// オーディエンス投票を締め切るまでの残り秒数
	/// </summary>
	private float closingAudienceRemainSeconds;

	/// <summary>
	/// 通信システム
	/// </summary>
	private NetworkGameMaster connector;

	/// <summary>
	/// イベントID
	/// </summary>
	private string eventId;

	/// <summary>
	/// オーディエンス投票情報として画面に出すテキスト
	/// </summary>
	private string postText;

	/// <summary>
	/// オーディエンス投票が通信障害によって利用できない状態になっているかどうか
	/// </summary>
	private bool isAudienceEventError;

	/// <summary>
	/// オーディエンス投票を締め切る直前に画面に出すテキスト
	/// </summary>
	private const string ClosingTextSource = @"${REMAIN_SECOND} 秒後に投票を締め切ります...
まだ投票できていない人は今のうちに済ませてね！";

	/// <summary>
	/// 画面上部のスクロール文字列の元のテキスト（平常運行時）
	/// </summary>
	private const string TopDescriptionSourceNormal = @"ただいま３名のプレイヤーが飛行の準備をしています。プレイ希望の方は次のゲームが始まるまでお待ち下さい（待ち時間はおよそ５分です）。観客の皆さんも結果を予想してみましょう！ スマホから画面のＱＲコードを読み込んで投票してみて下さい♪";

	/// <summary>
	/// 画面上部のスクロール文字列の元のテキスト（オーディエンス投票障害発生時）
	/// </summary>
	private const string TopDescriptionSourceError = @"ただいま３名のプレイヤーが飛行の準備をしています。プレイ希望の方は次のゲームが始まるまでお待ち下さい（待ち時間はおよそ５分です）。観客の皆さんも結果を予想をして頂きたいところですが、あいにくただいま障害発生中のため投票できません。";

	/// <summary>
	/// 各端末の操作が完了したかどうか
	/// </summary>
	private bool[] isControllerCompleted;

	/// <summary>
	/// 各端末の通信でエラーが発生しているかどうか
	/// </summary>
	private bool[] isControllerError;

	/// <summary>
	/// バックドアによって端末の操作内容が変更されたかどうか
	/// </summary>
	public static bool BackDoorOperated = false;

	/// <summary>
	/// 各端末の操作状況
	/// </summary>
	public static Dictionary<string, string>[] ControllerProgresses;

	/// <summary>
	/// 各端末の操作状況を更新するための Timer.deltaTime 加算用
	/// </summary>
	private float controllerProgressRefreshTimer;

	/// <summary>
	/// 実況の A-C、B 表示切替をするための Timer.deltaTime 加算用
	/// </summary>
	private float streamChangeTimer;

	/// <summary>
	/// 実況の A-C、B 表示切替フラグ
	/// </summary>
	private bool streamFlipper;

	/// <summary>
	/// 実況テキストエリアの元文章
	/// </summary>
	private string textSource;

	/// <summary>
	/// 実況テキストエリアの現在表示している文字列インデックス
	/// </summary>
	private int textCursorIndex;

	/// <summary>
	/// 実況メッセージを格納するゲームオブジェクトのコンポーネント
	/// </summary>
	private UnityEngine.UI.Text textArea;

	/// <summary>
	/// 実行中のメッセージ送りコルーチン
	/// </summary>
	private Coroutine messageCoroutine;

	/// <summary>
	/// コンストラクター
	/// </summary>
	/// <param name="parent">フェーズ管理クラスのインスタンス</param>
	public PhaseControllers(PhaseManager parent) : base(parent, null) {
		this.connector = new NetworkGameMaster(PhaseControllers.ControllerIPAddresses);
		PhaseControllers.BackDoorOperated = false;
	}

	/// <summary>
	/// ゲームオブジェクトの初期化
	/// </summary>
	public override void Start() {
		// ##### オーディエンス投票周り #####
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");
		closingWindow.gameObject.SetActive(false);
		this.parent.StartCoroutine(this.watcherAudience());

		// 新規投票イベント生成
		this.startAudienceEvent();

		// ##### 操作端末周り #####
		this.isControllerCompleted = new bool[PhaseControllers.ControllerIPAddresses.Length];
		this.isControllerError = new bool[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text = "";
			GameObject.Find("Controller_CompleteCheck" + i).transform.localScale = new Vector3(0, 0, 1);
			GameObject.Find("Controller_CompleteCheck" + i).GetComponent<UnityEngine.UI.Image>().enabled = false;
			for(int n = 0; n < GameObject.Find("Controller_Meters" + i).transform.childCount; n++) {
				GameObject.Find("Controller_Meters" + i).transform.GetChild(n).GetComponent<Meter>().SetValue(0);
			}
		}

		// 各端末に開始指示を出す
		PhaseControllers.ControllerProgresses = new Dictionary<string, string>[PhaseControllers.ControllerIPAddresses.Length];
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			PhaseControllers.ControllerProgresses[i] = null;
			this.startController(i);
		}

		// 実況テキスト
		this.textArea = GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>();
		this.textArea.text = "";
		this.textSource = "";
		this.textCursorIndex = 0;
		this.streamChangeTimer = 0;
		this.streamFlipper = false;
	}

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public override void Update() {
		// ##### 操作端末周り #####
		// すべての端末が終了したか判定する
		var isControllerCompleteAll = (this.isControllerCompleted.Where(a => a == false).Count() == 0);
		var closingWindow = GameObject.Find("ControllerPhase").transform.Find("Controller_ClosingAudienceWindow");

		// 端末の進捗状況
		this.controllerProgressRefreshTimer += Time.deltaTime;
		for(int i = 0; i < PhaseControllers.ControllerIPAddresses.Length; i++) {
			if(this.controllerProgressRefreshTimer >= PhaseControllers.ControllerProgressRefreshSeconds) {
				// 一定間隔で実行する処理

				// 操作状況を文字列化して表示
				GameObject.Find("Controller_Description" + i).GetComponent<UnityEngine.UI.Text>().text =
					this.conrollerProgressToString(i, PhaseControllers.ControllerProgresses[i]);

				if(PhaseControllers.BackDoorOperated == true || this.isControllerError[i] == false) {
					// 平常運転 or バックドアによって強制的に書き換えられたときのみ、各種メーター更新
					this.setMetersByConrollerProgress(i, PhaseControllers.ControllerProgresses[i]);
				}
			}

			// 完了したときのマーク表示
			var checkBox = GameObject.Find("Controller_CompleteCheck" + i);
			var checkBoxImage = checkBox.GetComponent<UnityEngine.UI.Image>();
			if(checkBoxImage.enabled != this.isControllerCompleted[i]) {
				checkBoxImage.enabled = this.isControllerCompleted[i];

				// チェックマークをiTween表示
				iTween.ScaleTo(checkBox, new Vector3(1, 1, 1), 1.0f);
			}
		}
		if(this.controllerProgressRefreshTimer >= PhaseControllers.ControllerProgressRefreshSeconds) {
			// 一定間隔で実行する処理が完了したらタイマーを初期化する
			this.controllerProgressRefreshTimer = 0;
		}

		// 実況更新
		var afterText = this.getStreamText(PhaseControllers.ControllerProgresses);
		var beforeText = this.textSource;
		this.textSource = afterText;

		this.streamChangeTimer += Time.deltaTime;
		if(this.streamChangeTimer >= PhaseControllers.StreamTextChangeSeconds) {
			// 一定時間ごとに実況の A-C、B 表示を切り替え
			this.streamFlipper = !this.streamFlipper;
			this.streamChangeTimer = 0;
		}

		if(afterText != beforeText) {
			// テキストが変更になったときにメッセージ送りを開始する
			GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>().text = "";
			if(this.messageCoroutine != null) {
				// 既に実行中の場合は中止する
				this.parent.StopCoroutine(this.messageCoroutine);
			}
			this.messageCoroutine = this.parent.StartCoroutine(this.nextMessageCharacter());
		} else {
			// 以前のテキストと同じときは引き続きメッセージ送りを行う
			GameObject.Find("Controller_StreamText").GetComponent<UnityEngine.UI.Text>().text = this.textSource.Substring(0, this.textCursorIndex);
		}

		// ##### オーディエンス投票周り #####
		// 投票状況更新
		GameObject.Find("Controller_AudiencePostInformation").GetComponent<UnityEngine.UI.Text>().text = this.postText;

		// スクロール位置を常に一番上にする
		var scrollView = GameObject.Find("Controller_PostedAudience");
		scrollView.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 1f;

		// オーディエンス投票締切直前カウントダウン表示
		if(isControllerCompleteAll == true) {
			if(this.isAudienceEventError == true) {

				// オーディエンス投票が障害発生中のときはすぐに次のフェーズへ移行する
				this.ChangeToFlightPhase();
				this.IsUpdateEnabled = false;

			} else {

				UnityEngine.UI.Text closingTextUI;

				if(closingWindow.gameObject.activeInHierarchy == false) {
					// オーディエンス投票締切直前カウントダウン開始
					closingWindow.gameObject.SetActive(true);
					this.closingAudienceRemainSeconds = PhaseControllers.ClosingAudienceWaitSeconds + 0.99f;

					// ウィンドウオープン
					closingWindow.localScale = Vector3.zero;
					iTween.ScaleTo(
						closingWindow.gameObject,
						new Vector3(1, 1, 1),
						1.0f
					);
				} else {
					// オーディエンス投票締切直前カウントダウン中
					this.closingAudienceRemainSeconds -= Time.deltaTime;
					if(this.closingAudienceRemainSeconds < 0) {
						// タイムアップ：次のフェーズへ移行
						this.ChangeToFlightPhase();
						this.IsUpdateEnabled = false;
						return;
					}
				}

				// 表示文字列を更新
				closingTextUI = GameObject.Find("Controller_ClosingAudienceText").GetComponent<UnityEngine.UI.Text>();
				closingTextUI.text = PhaseControllers.ClosingTextSource
					.Replace("${REMAIN_SECOND}", ((int)this.closingAudienceRemainSeconds).ToString());

			}
		}

		// ##### その他 #####
		// オーディエンス投票をやっているかどうかで上部スクロール文字列の中身を変える
		if(this.isAudienceEventError == false) {
			GameObject.Find("Controller_TopDescriptionText").GetComponent<UnityEngine.UI.Text>().text = PhaseControllers.TopDescriptionSourceNormal;
		} else {
			GameObject.Find("Controller_TopDescriptionText").GetComponent<UnityEngine.UI.Text>().text = PhaseControllers.TopDescriptionSourceError;
		}

		// バックドア：端末の操作を強制終了する
		if(Input.GetKeyDown(KeyCode.Alpha1) == true) {
			this.isControllerCompleted[0] = true;
		}
		if(Input.GetKeyDown(KeyCode.Alpha2) == true) {
			this.isControllerCompleted[1] = true;
		}
		if(Input.GetKeyDown(KeyCode.Alpha3) == true) {
			this.isControllerCompleted[2] = true;
		}
	}

	/// <summary>
	/// このフェーズが破棄されるときに実行する処理
	/// </summary>
	public override void Destroy() {
		// 通信切断
		this.connector.CloseConnectionsAll();
		this.connector.CloseAudiencePredicts(this.eventId, null);

		// コルーチン停止
		if(this.messageCoroutine != null) {
			this.parent.StopCoroutine(this.messageCoroutine);
		}
	}

	/// <summary>
	/// 新規オーディエンス投票イベントを生成します。
	/// </summary>
	private void startAudienceEvent() {
		this.connector.StartAudiencePredicts(new System.Action<string>((result) => {
			if(string.IsNullOrEmpty(result) == false) {
				// 生成されたイベントIDを保管
				this.eventId = result;
				Debug.Log("イベント開始: ID=" + result);
				this.isAudienceEventError = false;
			} else {
				// エラー発生時
				Debug.LogError("イベント開始失敗");
				this.isAudienceEventError = true;
			}
		}));
	}

	/// <summary>
	/// 操作端末へ開始指示を出します。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void startController(int roleId) {
		Debug.Log("開始指示: 端末ID=" + roleId + ", IPアドレス=" + PhaseControllers.ControllerIPAddresses[roleId]);

		this.connector.StartController(
			new ModelControllerStart() {
				RoleId = roleId,
				LimitTimeSecond = PhaseControllers.ControllerLimitSeconds,
			},
			roleId,
			new System.Action(() => {
				Debug.Log("接続開始成功: 端末ID=" + roleId);
				this.isControllerError[roleId] = false;

				// 進捗報告と完了報告を受け付ける
				this.watcherControllerProgresses(roleId);
				this.watcherControllerCompletes(roleId);
			}),
			new System.Action(() => {
				Debug.LogWarning("接続開始失敗: 端末ID=" + roleId);

				// 当該端末をエラー状態に変更
				this.isControllerError[roleId] = true;

				if(this.parent.PhaseIndex == PhaseManager.PhaseIndexMap[this.GetType()]) {
					// 現在のフェーズであるうちは自動でリトライを続ける
					this.startController(roleId);
				}
			})
		);
	}

	/// <summary>
	/// 操作端末の進捗報告を受け付けます。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void watcherControllerProgresses(int roleId) {
		this.connector.ReceiveControllerProgress(
			roleId,
			new System.Action<ModelDictionary<string, string>>((result) => {
				PhaseControllers.ControllerProgresses[roleId] = result.GetDictionary();

				if(this.isControllerCompleted[roleId] == false) {
					// 完了報告が届いていないうちは引き続き受け付ける
					this.watcherControllerProgresses(roleId);
				}
			}
		));
	}

	/// <summary>
	/// 操作端末の完了報告を受け付けます。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	private void watcherControllerCompletes(int roleId) {
		this.connector.WaitForControllers(
			roleId,
			new System.Action<ModelDictionary<string, string>>((result) => {
				this.isControllerCompleted[roleId] = true;
				PhaseControllers.ControllerProgresses[roleId] = result.GetDictionary();
			}
		));
	}

	/// <summary>
	/// メッセージの文字を１文字進めるコルーチン
	/// </summary>
	private IEnumerator nextMessageCharacter() {
		this.textCursorIndex = 0;

		while(true) {
			if(this.textCursorIndex + 1 > this.textSource.Length) {
				// 最後まで達したとき終了
				yield break;
			} else {
				// 先に遅延
				yield return new WaitForSeconds(PhaseManager.MessageSpeed);
			}

			// １文字進める
			this.textCursorIndex++;
			this.textArea.text = this.textSource.Substring(0, this.textCursorIndex);
		}
	}

	/// <summary>
	/// オーディエンス投票を監視するコルーチン
	/// </summary>
	private IEnumerator watcherAudience() {
		while(true) {
			yield return new WaitForSeconds(PhaseControllers.AudienceWatchSeconds);
			if(this.isAudienceEventError == true) {
				Debug.LogWarning("イベントIDが無効のため投票情報を取得できません。");
				this.postText = "＜＜障害発生中＞＞";
				continue;
			}

			// オーディエンス投票データを受け取る
			this.connector.GetAudiencePredicts(this.eventId, new System.Action<ModelAudiencePredictsResponse>((result) => {
				if(result == null) {
					// 取得失敗
					return;
				}
				Debug.Log("オーディエンス投票データ: レスポンス取得OK");

				// 取得時点では昇順だが、表示領域の関係で逆順に直す場合は以下をコメントアウト解除すること
				result.audiencePredicts.Reverse();

				// 表示用文字列を生成
				using(var str = new System.IO.StringWriter()) {
					foreach(var post in result.audiencePredicts) {
						str.WriteLine(post.receiveTime.Substring(11) + "  " + post.nickname + " さんが投票しました");
					}
					this.postText = str.ToString();
				}
			}));
		}
	}

	/// <summary>
	/// 操作端末の進捗情報を文字列にします。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	/// <param name="progress">操作端末の進捗情報</param>
	/// <returns>文字列化された進捗情報</returns>
	private string conrollerProgressToString(int roleId, Dictionary<string, string> progress) {
		using(var buf = new System.IO.StringWriter()) {
			// 役割とキャラクター名は常時表示
			switch(roleId) {
				case (int)NetworkConnector.RoleIds.A_Prepare:
					buf.Write("助走－クエリちゃん\n");
					break;

				case (int)NetworkConnector.RoleIds.B_Flight:
					buf.Write("飛行－ユニティちゃん\n");
					break;

				case (int)NetworkConnector.RoleIds.C_Assist:
					buf.Write("援護－サファイアートちゃん\n");
					break;
			}
			if(this.isControllerError[roleId] == true) {
				// 障害発生中
				buf.Write("　＜＜障害発生中＞＞");
				return buf.ToString();
			}
			if(progress == null) {
				// まだ開始指示を受け取っていない端末
				buf.Write("　開始準備中...");
				return buf.ToString();
			}

			// バリデーション
			if(progress.ContainsKey("roleId") == true && int.Parse(progress["roleId"]) != roleId) {
				Debug.LogError("GMで管理している役割IDと操作端末が申告してきた役割IDが一致しません。");
			}

			// 具体的な数値はメーターでグラフィカルに表示するため、ここではそれ以外の情報を入れる
			buf.Write("　");
			switch(roleId) {
				case (int)NetworkConnector.RoleIds.A_Prepare:
					switch(int.Parse(progress["option"])) {
						case (int)OptionA.Human:
							buf.Write("人力手押し");
							break;

						case (int)OptionA.Car:
							buf.Write("クルマでけん引");
							break;

						case (int)OptionA.Bomb:
							buf.Write("ボム");
							break;
					}
					break;

				case (int)NetworkConnector.RoleIds.B_Flight:
					buf.Write("スタミナチャージ");
					break;

				case (int)NetworkConnector.RoleIds.C_Assist:
					switch(int.Parse(progress["option"])) {
						case (int)OptionC.Human:
							buf.Write("全力応援");
							break;

						case (int)OptionC.Bomb:
							buf.Write("ボム");
							break;

						case (int)OptionC.Wairo:
							buf.Write("大会主催者にワイロ");
							if(progress.ContainsKey("wairoStep") == true) {
								// 選択肢を進めた回数（今、何問め？）
								buf.Write(" [" + (int)(int.Parse(progress["wairoStep"]) / (float)PhaseControllers.WairoStepLength * 100f) + " %]");
							}
							break;
					}
					break;
			}

#if UNITY_EDITOR
			// デバッグ用
			//foreach(var item in progress) {
			//	buf.Write(item.Key + "=" + item.Value + ", ");
			//}
#endif

			return buf.ToString();
		}
	}

	/// <summary>
	/// 操作端末の進捗情報を基に性能バランス値を各種メーターにセットします。
	/// </summary>
	/// <param name="roleId">役割ID</param>
	/// <param name="progress">操作端末の進捗情報</param>
	private void setMetersByConrollerProgress(int roleId, Dictionary<string, string> progress) {
		int option;
		var values = new float[(int)PowerMeter.Count];

		if(progress == null) {

			// まだ始まっていないときは初期状態を維持する
			values[(int)PowerMeter.StartPower] = 0;
			values[(int)PowerMeter.FlightPower] = 0;
			values[(int)PowerMeter.LackPower] = 0;

		} else {

			switch(roleId) {
				case (int)NetworkConnector.RoleIds.A_Prepare:
					// 選択肢に応じて基本バランスが変わる
					option = int.Parse(progress["option"]);
					switch(option) {
						case (int)OptionA.Human:
							// 人力
							values[(int)PowerMeter.StartPower] = 0.1f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.4f;
							values[(int)PowerMeter.FlightPower] = 0;
							values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.5f;
							break;

						case (int)OptionA.Car:
							// 牽引
							values[(int)PowerMeter.StartPower] = 0.4f + int.Parse(progress["param"]) / 100.0f * 0.4f;
							values[(int)PowerMeter.FlightPower] = 0;
							values[(int)PowerMeter.LackPower] = 0.1f + int.Parse(progress["param"]) / 100.0f * 0.4f;
							break;

						case (int)OptionA.Bomb:
							// 爆弾
							values[(int)PowerMeter.StartPower] = 1.0f;
							values[(int)PowerMeter.FlightPower] = 0.2f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.8f;
							values[(int)PowerMeter.LackPower] = 0;
							break;
					}
					break;

				case (int)NetworkConnector.RoleIds.B_Flight:
					// 基本バランスは固定で、スコアに応じて増減する
					values[(int)PowerMeter.StartPower] = 0;
					values[(int)PowerMeter.FlightPower] = int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 1.0f;
					values[(int)PowerMeter.LackPower] = 0;
					break;

				case (int)NetworkConnector.RoleIds.C_Assist:
					// 選択肢に応じて基本バランスが変わる
					option = int.Parse(progress["option"]);
					switch(option) {
						case (int)OptionC.Human:
							// 応援
							values[(int)PowerMeter.StartPower] = 0;
							values[(int)PowerMeter.FlightPower] = 0.1f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.25f;
							values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.3f;
							break;

						case (int)OptionC.Bomb:
							// 爆弾
							values[(int)PowerMeter.StartPower] = 0;
							values[(int)PowerMeter.FlightPower] = 0.2f + int.Parse(progress["param"]) / (float)ControllerLimitSeconds * 0.5f;
							values[(int)PowerMeter.LackPower] = 0;
							break;

						case (int)OptionC.Wairo:
							// 賄賂
							values[(int)PowerMeter.StartPower] = 0;
							values[(int)PowerMeter.FlightPower] = 0;
							values[(int)PowerMeter.LackPower] = 0.2f + int.Parse(progress["param"]) / (float)PhaseControllers.WairoScoreMax * 0.8f;
							break;
					}
					break;
			}

		}

		// メーターにセット
		var meters = GameObject.Find("Controller_Meters" + roleId).transform;
		for(int i = 0; i < meters.childCount; i++) {
			meters.GetChild(i).GetComponent<Meter>().SetValue(values[i]);
		}
	}

	/// <summary>
	/// 現在の端末操作状況を基に、実況テキストを返します。
	/// </summary>
	/// <returns>実況テキスト</returns>
	private string getStreamText(Dictionary<string, string>[] progresses) {
		if(progresses == null || progresses.Length == 0) {
			// 進捗オブジェクトがない
			return "ただいまプレイヤーの準備中です……";
		}

		using(var buf = new StringWriter()) {

			int optionA =
				(progresses[(int)NetworkConnector.RoleIds.A_Prepare] == null) ? -1 :
					(progresses[(int)NetworkConnector.RoleIds.A_Prepare].ContainsKey("option")
						? int.Parse(progresses[(int)NetworkConnector.RoleIds.A_Prepare]["option"]) : -1);
			int optionC =
				(progresses[(int)NetworkConnector.RoleIds.C_Assist] == null) ? -1 :
					(progresses[(int)NetworkConnector.RoleIds.C_Assist].ContainsKey("option")
						? int.Parse(progresses[(int)NetworkConnector.RoleIds.C_Assist]["option"]) : -1);

			bool controllerB = (progresses[(int)NetworkConnector.RoleIds.B_Flight] != null);

			if(controllerB == false || (this.streamFlipper == false && optionA != -1 && optionC != -1)) {

				// AとCの選択肢の組み合わせで実況を表示する

				if(optionA == (int)OptionA.Bomb && optionC == (int)OptionC.Bomb) {
					buf.WriteLine("これは不穏な火薬の香りが漂っていますねー……！");
				} else if(optionA == (int)OptionA.Bomb && optionC == (int)OptionC.Human) {
					buf.WriteLine("どこかから応援の発声練習が聞こえてきましたが……\nおや、爆弾が……？");
				} else if(optionA == (int)OptionA.Bomb && optionC == (int)OptionC.Wairo) {
					buf.WriteLine("爆発音とともにチームの一人が大会本部へ駆けだした！\n一体何があったのでしょうか！");
				} else if(optionA == (int)OptionA.Car && optionC == (int)OptionC.Bomb) {
					buf.WriteLine("車にガソリンを入れるかたわら、爆弾に火が着く！\n今のうちに119番通報の準備をお願いします！");
				} else if(optionA == (int)OptionA.Car && optionC == (int)OptionC.Human) {
					buf.WriteLine("発声練習の横で独りで懸命にガソリンを入れていくゥ！\n少しは給油を手伝う気は無いのか！");
				} else if(optionA == (int)OptionA.Car && optionC == (int)OptionC.Wairo) {
					buf.WriteLine("車とガソリンを用意しているようですが……\nおっと、何やらアタッシュケースをまさぐっていますね…？");
				} else if(optionA == (int)OptionA.Human && optionC == (int)OptionC.Bomb) {
					buf.WriteLine("正当派に人力で助走をつけ……お、おや!?\n何やら導火線に火をつけていますが！？");
				} else if(optionA == (int)OptionA.Human && optionC == (int)OptionC.Human) {
					buf.WriteLine("人力で助走をつけ、さらに応援で気合を注入！\nこれはチームの体力的にキツそうだ……！");
				} else if(optionA == (int)OptionA.Human && optionC == (int)OptionC.Wairo) {
					buf.WriteLine("ひたむきに人間の力で飛行機を進め……\nおや、何やら札束を持って大会本部へ向かっているようですが……！？");
				}

			} else if(controllerB == true) {

				// Bの状況を表示する

				int paramB =
					progresses[(int)NetworkConnector.RoleIds.B_Flight].ContainsKey("param")
						? int.Parse(progresses[(int)NetworkConnector.RoleIds.B_Flight]["param"]) : -1;

				if(paramB * 100f / PhaseControllers.ControllerLimitSeconds >= 70) {
					buf.WriteLine("飛行役のユニティちゃんは\n気合充分といった、ある種威風を纏っている！\nこれは期待が出来そうです！");
				} else if(paramB * 100f / PhaseControllers.ControllerLimitSeconds >= 40) {
					buf.WriteLine("飛行役のユニティちゃんは\nなかなか順調なペースで\nウォーミングアップが出来ているようです！");
				} else {
					// buf.WriteLine("一方飛行役のユニティちゃん、\n若干パイロットの士気が落ちている！\nついには漫画を読み始めるまでありそうだ！");
					// TODO: 「落ちている」ではなく、初期状態なので文章の訂正が必要
					buf.WriteLine("飛行役のユニティちゃんは\nパワーを貯めている！");
				}

			} else {

				// どの端末も開始できていない
				return "ただいまプレイヤーの準備中です……";

			}

			return buf.ToString();
		}
	}

	/// <summary>
	/// 端末操作フェーズを終えて飛行フェーズへ移行します。
	/// </summary>
	public void ChangeToFlightPhase() {
		this.parent.ChangePhase(new PhaseFlight(this.parent, new object[] {
			this.eventId,
			PhaseControllers.ControllerProgresses[0],
			PhaseControllers.ControllerProgresses[1],
			PhaseControllers.ControllerProgresses[2],
		}));
	}

}
