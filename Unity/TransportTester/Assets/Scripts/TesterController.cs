using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModelControllerProgress = ModelDictionary<string, string>;

/// <summary>
/// 操作端末としてテストを行います。
/// １クラスで３役いずれも成り替われます。
/// </summary>
public class TesterController : TesterBase {

	/// <summary>
	/// UDPによる操作端末の進捗報告を送信する回数
	/// </summary>
	private const int UDPProgressSendCount = 10;

	/// <summary>
	/// UDPによる操作端末の進捗報告を送信した回数
	/// </summary>
	private int UDPProgressSendCounter = 0;

	/// <summary>
	/// UDPによる操作端末の進捗報告を送信する間隔秒数
	/// </summary>
	private const float UDPProgressSendFrequency = 1.0f;

	/// <summary>
	/// UDPによる操作端末の進捗報告を最後に送信した時刻
	/// </summary>
	private float UDPProgressLastSendTime = 0;

	/// <summary>
	/// UDPによる操作端末の進捗報告が有効であるかどうか
	/// </summary>
	private bool enabledUDPReport = false;

	/// <summary>
	/// 毎フレーム更新処理
	/// </summary>
	public void Update() {
		if(this.enabledUDPReport == true) {
			// 一定間隔でUDPによる操作端末の進捗報告を送信する
			if(Time.time - this.UDPProgressLastSendTime >= TesterController.UDPProgressSendFrequency) {
				this.UDPProgressLastSendTime = Time.time;
				this.testProcess2();
			}
		}
	}

	/// <summary>
	/// テストを実行します。
	/// </summary>
	/// <param name="parameters">テストに必要なパラメーターの連想配列</param>
	public override void DoTest(Dictionary<string, object> parameters) {
		Logger.LogProcess("操作端末 " + parameters["RoleID"] + " としてテストを開始します。");

		// パラメーター初期化
		this.UDPProgressSendCounter = 0;
		this.parameters = parameters;
		this.connector = new NetworkController((string)parameters["GMIP"]) {
			RoleId = (int)parameters["RoleID"],
		};

		this.testProcess1();
	}

	/// <summary>
	/// GMからの開始指示を待機します。
	/// </summary>
	private void testProcess1() {
		Logger.LogProcess("GMからの開始指示を待機します...");

		(this.connector as NetworkController).ControllerWaitForStart((int)this.parameters["RoleID"], (obj) => {
			Logger.LogProcess("GMからの開始指示を受信しました。");

			// NOTE: 本来の操作端末は受信内容をもとに初期設定を行うが、ここでは情報表示のみ行う
			Logger.LogProcess("受信内容 -> RoleId=" + obj.RoleId + ", LimitTimeSecond=" + obj.LimitTimeSecond);

			// 次のフェーズへ
			this.enabledUDPReport = true;
		});
	}

	/// <summary>
	/// GMに進捗報告を送信します。
	/// </summary>
	private void testProcess2() {
		Logger.LogProcess("UDPでGMへ進捗報告を送信します..." + (this.UDPProgressSendCounter + 1) + " / " + TesterController.UDPProgressSendCount + " 回目");

		// 送信処理
		(this.connector as NetworkController).ReportProgressToGameMaster(this.createReportData());

		// 一定回数に達するまで進捗報告を受け付ける
		this.UDPProgressSendCounter++;
		if(this.UDPProgressSendCounter < TesterController.UDPProgressSendCount) {
			// 再度受信待ちへ
			Logger.LogProcess("UDPによる進捗報告を継続します。");
		} else {
			// 次のフェーズへ
			Logger.LogProcess("UDPによる進捗報告を終了します。");
			this.enabledUDPReport = false;
			this.testProcess3();
		}
	}

	/// <summary>
	/// GMに完了報告を送信します。
	/// </summary>
	private void testProcess3() {
		Logger.LogProcess("TCPで操作端末の完了報告を送信します...");

		// 送信処理
		(this.connector as NetworkController).ReportCompleteToGameMaster(
			this.createReportData(),
			() => {
				// 成功時: 完了
				Logger.LogProcess("TCPでの操作端末の完了報告に成功しました。");

				// テスト完了
				this.connector.CloseConnectionsAll();
				Logger.LogProcess("すべてのテストフェーズが完了しました。");
				if(this.TestCompletedCallBack != null) {
					// テスト完了時のコールバック関数を呼び出す
					this.TestCompletedCallBack();
				}
			},
			() => {
				// 失敗時: 再試行
				Logger.LogProcess("操作端末への接続に失敗しました。" + NetworkConnector.ConnectionWaitSecondsForConnect + " 秒後に再試行します...");
				System.Threading.Thread.Sleep(NetworkConnector.ConnectionWaitSecondsForConnect * 1000);
				this.testProcess3();
			}
		);
	}

	/// <summary>
	/// GMに報告するデータを作成します。
	/// </summary>
	private ModelControllerProgress createReportData() {
		var controllerReport = new ModelControllerProgress(new Dictionary<string, string>() { });
		controllerReport.GetDictionary()["RoleID"] = ((int)this.parameters["RoleID"]).ToString();

		// 報告するデータを作成する
		switch((int)this.parameters["RoleID"]) {
			case 0:
				controllerReport.GetDictionary()["OptionIndex"] = "1";
				controllerReport.GetDictionary()["ActionResult"] = "114514";
				break;

			case 1:
				controllerReport.GetDictionary()["ActionResult"] = "999";
				break;

			case 2:
				controllerReport.GetDictionary()["OptionIndex"] = "2";
				break;
		}

		return controllerReport;
	}

}
