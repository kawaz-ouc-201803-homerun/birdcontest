//###################################################################
//    サーバー接続処理
//###################################################################

/**
 * WebAPIのURL
 */
var SERVERURL = "http://tsownserver.dip.jp:8080/GameJamAudience/handler/Audience.json";


/**
 * ページ読み込み時に実行します。
 */
$(function() {
	CheckPostable();
})


/**
 * 現在投票可能な状態かどうかを問い合わせます。
 */
function CheckPostable() {
	$.ajax({
		url: SERVERURL,
		type: "POST",
		async: true,
		contentType: "application/JSON",
		dataType: "JSON",
		data: JSON.stringify({
			"method": "getCurrentEventId",
			"params": [],
			"id": 1
		}),
		success: function(data) {
			// 取得完了: data.resultにはパース済みの項目が格納されている状態

			// TODO: data.result["eventId"] が空でない=そのまま、空=網掛けして「投票締切」メッセージ表示

		},
		error: function() {
			// 取得失敗: エラーページに飛ばす
			location.href = "";
		}
	})
}


/**
 * オーディエンス予想を投票します。
 */
function DoPost() {
	$.ajax({
		url: SERVERURL,
		type: "POST",
		async: true,
		contentType: "application/JSON",
		dataType: "JSON",
		data: JSON.stringify({
			"method": "post",
			"params": [],
			"id": 2
		}),
		success: function(data) {
			// 送信完了: 送信完了ページに飛ばす
			location.href = "";
		},
		error: function() {
			// 取得失敗: エラーページに飛ばす
			location.href = "";
		}
	})
}
