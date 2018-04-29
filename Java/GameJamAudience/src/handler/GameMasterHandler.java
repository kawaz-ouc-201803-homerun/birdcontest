package handler;

import java.sql.SQLException;
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
	 * @throws SQLException エラー発生
	 */
	public ModelAudienceNewEventResponse newEvent() throws SQLException {
		val dao = new AudienceDAO();
		return new ModelAudienceNewEventResponse(dao.newEvent());
	}

	/**
	 * 指定したイベントのオーディエンス予想をすべて取得します。
	 *
	 * @param eventId イベントID
	 * @return 指定したオーディエンス予想のリストを示すレスポンスオブジェクト
	 * @throws SQLException エラー発生
	 */
	public ModelAudiencePredictList getPosts(String eventId) throws SQLException {
		val dao = new AudienceDAO();
		return new ModelAudiencePredictList(dao.getPosts(eventId));
	}

	/**
	 * 指定したイベントのオーディエンス予想を締め切ります。
	 *
	 * @param eventId イベントID
	 * @throws SQLException エラー発生
	 */
	public void close(String eventId) throws SQLException {
		val dao = new AudienceDAO();
		dao.close(eventId);
	}

	/**
	 * オーディエンスの参加延べ人数を返します。
	 *
	 * @return オーディエンスの参加延べ人数を示すレスポンスオブジェクト
	 * @throws SQLException エラー発生
	 */
	public ModelAudienceGetPeopleCountResponse getPeopleCount() throws SQLException {
		val dao = new AudienceDAO();
		return new ModelAudienceGetPeopleCountResponse(dao.getPeopleCount());
	}

}
