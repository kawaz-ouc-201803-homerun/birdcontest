//###################################################################
//    サーバー接続処理
//###################################################################

/**
 * WebAPIのURL
 */
// var SERVERURL = "http://tsownserver.dip.jp:8080/GameJamAudience/";
var SERVERURL = "http://localhost:8080/GameJamAudience/";


/**
 * ページ読み込み時に実行します。
 */
$(function() {
	// 投票可否を問い合わせる
	window.CheckPostable();
})


/**
 * NowLoadingアニメーションを開始します。
 * アニメーション中、コンテンツは非表示になります。
 */
function BeginLoadingAnimation() {
	var height = $(window).height();
	$(".page-container").css("display", "none");
	$("#loader-bg, #loader").height(height).css("display", "block");
}


/**
 * NowLoadingアニメーションを終了します。
 * 非表示にされていたコンテンツが表示されます。
 */
function EndLoadingAnimation() {
	$("#loader-bg").fadeOut(500);
	$("#loader").fadeOut(200);
	$(".page-container").css("display", "block");
}


/**
 * 入力フォームを無効化します。
 *
 * @param message メッセージ
 */
function DisabledForm(message) {
	$(".page-container").addClass("error");
	$("#nickname").attr("disabled", true);
	$("#predict").attr("disabled", true);
	$("#post").attr("disabled", true);

	// メッセージを変更
	if(!!message) {
		$(".error .txt").text(message);
	}
}


/**
 * 現在投票可能な状態かどうかを問い合わせます。
 */
function CheckPostable() {
	// NowLoading状態にする
	window.BeginLoadingAnimation();

	$.ajax({
		url: SERVERURL + "handler/Audience.json",
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
			// エラーチェック: JSONICのエラーはHTTPステータスOKで返される仕様
			if(!!data.error && data.error != null) {
				console.log(data.error);
				window.ErrorHandling(data.error.code);
				return;
			}

			// 判定結果の取り出し
			window.currentEventId = data.result["currentEventId"];
			if(!window.currentEventId || window.currentEventId == "") {
				// 投票不可
				alert("現在投票は締め切られています。次のゲームが始まるまでお待ち下さい。");
				window.DisabledForm("次のゲームが始まるまでお待ち下さい。");
			}
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			// 標準エラーハンドリング
			window.ErrorHandling(XMLHttpRequest.status);
		},
		complete: function() {
			// NowLoading停止
			window.EndLoadingAnimation();
		}
	})
}


/**
 * オーディエンス予想を投票します。
 */
function DoPost() {
	var nickname = $("#nickname").val();
	var predict = parseInt($("#predict").val());

	// バリデーション処理
	if(nickname == null || nickname.trim() == "") {
		alert("ニックネームを入力して下さい。");
		return;
	}
	if(isNaN(predict) == true || predict < 0 || 99999999 < predict) {
		alert("予想飛距離は 0～99999999 の間で入力して下さい。");
		return;
	}
	if(!("currentEventId" in window) || currentEventId == null) {
		alert("現在投票は締め切られています。次のゲームが始まるまでお待ち下さい。");
		window.DisabledForm("次のゲームが始まるまでお待ち下さい。");
		return;
	}

	// NowLoading状態にする
	window.BeginLoadingAnimation();

	// 結果を投票
	$.ajax({
		url: SERVERURL + "servlet/AudiencePost",
		type: "POST",
		async: true,
		contentType: "application/JSON",
		dataType: "JSON",
		data: JSON.stringify({
			"method": "post",
			"params": [currentEventId, nickname, predict],
			"id": 2
		}),
		success: function(data) {
			// 送信完了: 送信完了ページに飛ばす
			alert("投票が完了しました。");
			window.DisabledForm("投票済みです。");
		},
		error: function(XMLHttpRequest, textStatus, errorThrown) {
			// 標準エラーハンドリング
			window.ErrorHandling(XMLHttpRequest.status);
		},
		complete: function() {
			// NowLoading停止
			window.EndLoadingAnimation();
		}
	})
}


/**
 * Ajax通信のエラーハンドリングを行います。
 *
 * @param statusCode HTTPステータスコード
 */
function ErrorHandling(statusCode) {
	switch(statusCode) {
	    case 406:
			alert("既に投票されています。次のゲームが始まるまでお待ち下さい。");
			window.DisabledForm("投票済みです。");
	    	break;
	    case 500:
			alert("サーバー側の不具合のため、投票に失敗しました。ゲームスタッフまでお声掛け下さい。");
	    	break;
	    default:
	    	alert("予期しないエラーが発生しました。ゲームスタッフまでお声掛け下さい。");
	}
}
