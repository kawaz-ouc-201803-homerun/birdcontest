using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModelControllerProgress = ModelDictionary<string, string>;

/// <summary>
/// ゲームマスターとしてテストを行います。
/// 簡略化のため、１台の操作端末とだけ通信を行います。
/// </summary>
public class TesterGM : TesterBase {

	/// <summary>
	/// UDPによる操作端末の進捗報告を受け取る回数
	/// </summary>
	private const int UDPProgressReceiveCount = 2;

	/// <summary>
	/// UDPによる操作端末の進捗報告を受け取った回数
	/// </summary>
	private int UDPProgressReceiveCounter = 0;

	/// <summary>
	/// テストを実行します。
	/// </summary>
	/// <param name="parameters">テストに必要なパラメーターの連想配列</param>
	public override void DoTest(Dictionary<string, object> parameters) {
		Logger.LogProcess("ゲームマスターとしてテストを開始します。");

		// パラメーター初期化
		this.UDPProgressReceiveCounter = 0;
		this.parameters = parameters;
		this.connector = new NetworkGameMaster(new string[] {
			(string)parameters["CtrlIP"],
			(string)parameters["CtrlIP"],
			(string)parameters["CtrlIP"],
		});

		this.testProcess1();
	}

	/// <summary>
	/// 操作端末に役割を送信してゲームを開始させます。
	/// </summary>
	private void testProcess1() {
		int roleId = (int)this.parameters["RoleID"];

		// NOTE: 本来のゲームマスターは、以下の処理を全端末分だけ行うが、このテスターは１端末しか相手にしないので１回のみ通信を行う

		Logger.LogProcess("操作端末へゲーム開始指示を送ります...");
		(this.connector as NetworkGameMaster).StartController(
			new ModelControllerStart() {
				RoleId = roleId,
				LimitTimeSecond = 60,
			},
			roleId,
			() => {
				// 成功時: ゲーム開始

				// NOTE: 本来のゲームマスターは以下のように通信できた端末の数を数えて、全員始まったら次のフェーズに移るようにするが、このテスターは１端末しか相手にしないので無条件に次のフェーズへ移る
				// this.startedContollerCounter += 1;
				// if(this.startedControllerCounter == GMTester.ControllerCount) {
				// TODO: 次のフェーズへ移る
				// }

				Logger.LogProcess("操作端末へのゲーム開始指示が完了しました。");
				this.testProcess2();
			},
			() => {
				// 失敗時: 再試行
				Logger.LogProcess("操作端末への接続に失敗しました。" + NetworkConnector.ConnectionWaitSecondsForConnect + " 秒後に再試行します...");
				System.Threading.Thread.Sleep(NetworkConnector.ConnectionWaitSecondsForConnect * 1000);
				this.testProcess1();
			}
		);
	}

	/// <summary>
	/// 操作端末の進捗報告をUDPで受け取ります。
	/// </summary>
	private void testProcess2() {
		Logger.LogProcess("UDPで操作端末の進捗報告を受け付けます..." + (this.UDPProgressReceiveCounter + 1) + " / " + TesterGM.UDPProgressReceiveCount + " 回目");

		(this.connector as NetworkGameMaster).ReceiveControllerProgress(new Action<ModelControllerProgress>((data) => {
			Logger.LogProcess("UDPによる操作端末の進捗報告を受信しました。");
			this.testProcessControllerStatus(data);

			// 一定回数に達するまで進捗報告を受け付ける
			this.UDPProgressReceiveCounter++;
			if(this.UDPProgressReceiveCounter < TesterGM.UDPProgressReceiveCount) {
				// 再度受信待ちへ
				Logger.LogProcess("操作端末の進捗報告受付を継続します。");
				this.testProcess2();
			} else {
				// 次のフェーズへ
				Logger.LogProcess("操作端末の進捗報告受付を終了します。");
				this.testProcess3();
			}
		}));
	}

	/// <summary>
	/// 操作端末の完了報告をTCPで受け取ります。
	/// </summary>
	private void testProcess3() {
		Logger.LogProcess("TCPで操作端末の完了報告を受け付けます...");

		// NOTE: 本来のゲームマスターは以下の処理を端末の数だけ繰り返し実行する必要がある

		(this.connector as NetworkGameMaster).WaitForControllers(new Action<ModelControllerProgress>((data) => {
			Logger.LogProcess("TCPによる操作端末の完了報告を受信しました。");
			this.testProcessControllerStatus(data);

			// NOTE: 本来のゲームマスターは以下のように通信できた端末の数を数えて、全員始まったら次のフェーズに移るようにするが、このテスターは１端末しか相手にしないので無条件に次のフェーズへ移る
			// this.completedContollerCounter += 1;
			// if(this.completedControllerCounter == GMTester.ControllerCount) {
			// TODO: 次のフェーズへ移る
			// }

			// テスト完了
			this.connector.CloseConnectionsAll();
			Logger.LogProcess("すべてのテストフェーズが完了しました。");
			if(this.TestCompletedCallBack != null) {
				// テスト完了時のコールバック関数を呼び出す
				this.TestCompletedCallBack();
			}
		}));
	}

	/// <summary>
	/// 操作端末の進捗報告を受理します。
	/// </summary>
	/// <param name="data">進捗報告データ</param>
	private void testProcessControllerStatus(ModelControllerProgress data) {
		// NOTE: 本来のゲームマスターは以下のように処理分岐して細かく情報を取り出す必要があるが、ここでは単に文字列として表示するだけなので何もしない
		switch(data.GetDictionary()["RoleID"]) {
			case "0":
				// TODO: 端末Aのデータ取り出し処理
				break;
			case "1":
				// TODO: 端末Bのデータ取り出し処理
				break;
			case "2":
				// TODO: 端末Cのデータ取り出し処理
				break;
		}

		// 受け取ったデータをそのまま文字列として表示
		Logger.LogProcess("操作端末データ受信 -> " + data.GetJSON());
	}

}
