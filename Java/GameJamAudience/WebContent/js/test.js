//###################################################################
//    システムテスト用スクリプト
//    これはフロントのテストではなく、バックエンドのテストです。
//    実行前にDBのレコードはすべて削除する必要があります。
//###################################################################

/**
 * テスト中に使用するイベントID
 */
var EventId = null;


/**
 * ページ読み込み時に実行します。
 */
$(function(){
	// 先頭のテストケースを開始し、以降は非同期処理が完了したら逐次的に進める
	window.NextToTestCase(0);
});


/**
 * テスト結果を判定します。
 *
 * @param isOK 成功したかどうか
 * @param object 成功したときは次のテストケースの番号、失敗したときはエラー情報
 */
function Assert(isOK, object) {
	if(data.expect == data.result) {
		console.log("    -> OK");
		window.NextToTestCase(object);
	} else {
		// エラーが起きたときは一時停止する
		console.log("    -> NG");
		console.log("[ERROR] " + object);
		debugger;
	}
}


/**
 * テストケースを実行します。
 *
 * @param index テストケース番号 (配列のインデックス)
 */
function NextToTestCase(index) {
	if(!!window.TestCases[index]) {
		window.TestCases[index](index);
	} else {
		console.log("テストがすべて完了しました。");
	}
}


/**
 * テストケース
 */
TestCases = [
	function(i) {
		console.log("[" + i + ": 異常系 - 現在有効なイベントのIDを取得]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "getCurrentEventId",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			console.log(data.result["currentEventId"]);
			if(!window.currentEventId || window.currentEventId == "") {
				// イベントIDが取れなければOK
				window.Assert(true, i + 1);
			} else {
				window.Assert(false, "イベントIDが取れてしまいました。DBを初期化して下さい。");
			}
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 - イベント生成]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "newEvent",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			window.EventId = data.result["eventId"];
			console.log(data.result["eventId"]);
			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 - 現在有効なイベントのIDを取得]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "getCurrentEventId",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			console.log(data.result["currentEventId"]);
			if(!window.currentEventId || window.currentEventId == "") {
				window.Assert(false, "イベントIDが取れませんでした。DBのTEVENTテーブルis_closedを確認して下さい。");
			} else {
				// イベントIDが取れればOK
				window.Assert(true, i + 1);
			}
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 - 投票処理]");
		$.ajax({
			url: SERVERURL + "servlet/AudiencePost",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "post",
				"params": [window.EventId, "SystemTest", 114514],
				"id": i
			})
		})
		.done((data) => {
			// こっちに来たら投票成功
			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// 自作のサーブレットなので失敗したらこっちに飛んでくる仕様
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 - 投票データ取り出し処理]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "getPosts",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			console.log(data.result);
			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 1 - 投票締め切り処理]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "close",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 2 - 投票締め切り処理]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "close",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			// 重複してクローズしてもエラーにならない仕様
			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},

	function(i) {
		console.log("[" + i + ": 正常系 - 参加延べ人数取得]");
		$.ajax({
			url: SERVERURL + "handler/Audience.json",
			type: "POST",
			async: true,
			contentType: "application/JSON",
			dataType: "JSON",
			data: JSON.stringify({
				"method": "getPeopleCount",
				"params": [],
				"id": i
			})
		})
		.done((data) => {
			if(data.error != null) {
				window.Assert(false, data.error);
			}

			console.log(data.result["count"]);
			window.Assert(true, i + 1);
		})
		.fail((XMLHttpRequest, textStatus, errorThrown) => {
			// JSONICのエラーはこっちに来ない想定
			window.Assert(false, XMLHttpRequest);
		});
	},
];
