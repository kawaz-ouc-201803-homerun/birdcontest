package handler;

import java.util.Date;
import database.AudienceDAO;
import jsonable.ModelAudienceGetCurrentEventIdResponse;
import jsonable.ModelAudiencePredict;
import lombok.val;

/**
 * オーディエンス投稿用ハンドラー
 * JSONICのWeb-API経由でクライアントから指定された処理を実行します。
 *
 * @author 知樹
 *
 */
public class AudienceHandler {

	/**
	 * オーディエンスの投票処理を行います。
	 *
	 * @param eventId イベントID
	 * @param nickname ニックネーム
	 * @param predict 予想飛距離
	 * @param ipAddress IPアドレス
	 * @param osEnvironment 機種・OSなど
	 */
	public void post(String eventId, String nickname, int predict, String ipAddress, String osEnvironment) {
		val dao = new AudienceDAO();
		dao.post(new ModelAudiencePredict(
			null,
			eventId,
			nickname,
			predict,
			(new Date()).toString(),
			ipAddress,
			osEnvironment
		));
	}

	/**
	 * 現在投票可能かどうかを返します。
	 *
	 * @return 投票可能かどうかを示すレスポンスオブジェクト
	 */
	public ModelAudienceGetCurrentEventIdResponse getCurrentEventId() {
		val dao = new AudienceDAO();
		return new ModelAudienceGetCurrentEventIdResponse(dao.getCurrentEventId());
	}

}
