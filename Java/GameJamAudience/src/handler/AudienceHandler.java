package handler;

import database.AudienceDAO;
import jsonable.ModelAudienceGetCurrentEventIdResponse;
import lombok.val;

/**
 * オーディエンス汎用ハンドラー
 * JSONICのWeb-API経由でクライアントから指定された処理を実行します。
 *
 * @author 知樹
 *
 */
public class AudienceHandler {

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
