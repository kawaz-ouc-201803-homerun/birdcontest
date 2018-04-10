package handler;

import database.AudienceDAO;
import jsonable.ModelAudienceGetPeopleCountResponse;
import jsonable.ModelAudienceNewEventResponse;
import jsonable.ModelAudiencePredictList;
import lombok.val;

/**
 * Unityゲームマスター用ハンドラー
 * JSONICのWeb-API経由でクライアントから指定された処理を実行します。
 *
 * @author 知樹
 *
 */
public class GameMasterHandler {

	/**
	 * オーディエンス予想の新しいイベントを生成します。
	 *
	 * @return 生成したイベントのIDを示すレスポンスオブジェクト
	 */
	public ModelAudienceNewEventResponse newEvent() {
		val dao = new AudienceDAO();
		return new ModelAudienceNewEventResponse(dao.newEvent());
	}

	/**
	 * 指定したイベントのオーディエンス予想をすべて取得します。
	 *
	 * @param eventId イベントID
	 * @return 指定したオーディエンス予想のリストを示すレスポンスオブジェクト
	 */
	public ModelAudiencePredictList getPosts(String eventId) {
		val dao = new AudienceDAO();
		return new ModelAudiencePredictList(dao.getPosts(eventId));
	}

	/**
	 * 指定したイベントのオーディエンス予想を締め切ります。
	 *
	 * @param eventId イベントID
	 */
	public void close(String eventId) {
		val dao = new AudienceDAO();
		dao.close(eventId);
	}

	/**
	 * オーディエンスの参加延べ人数を返します。
	 *
	 * @return オーディエンスの参加延べ人数を示すレスポンスオブジェクト
	 */
	public ModelAudienceGetPeopleCountResponse getPeopleCount() {
		val dao = new AudienceDAO();
		return new ModelAudienceGetPeopleCountResponse(dao.getPeopleCount());
	}

}
