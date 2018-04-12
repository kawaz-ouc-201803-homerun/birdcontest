package database;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import execption.EventException;
import execption.UserException;
import jsonable.ModelAudiencePredict;
import lombok.val;

/**
 * 投票データベースDAO
 *
 * @author tomokis
 *
 */
public class AudienceDAO implements DAOInterface {

	/**
	 * 新規イベント作成
	 * 作成前に存在しているイベントはすべて締切状態にします。
	 *
	 * @return イベントID
	 * @throws SQLException DBエラー
	 */
	public String newEvent() throws SQLException {
		String eventId = this.createGUID();

		try(val con = ConnectionManager.getManager().connect(this)) {

			con.setAutoCommit(false);

			// すべてのイベントを締切にする
			try(val stmt = con.prepareStatement("UPDATE TEVENT SET is_closed = 1")) {
				stmt.executeUpdate();
			}

			// イベント作成
			try(val stmt = con.prepareStatement("INSERT INTO TEVENT VALUES (?, datetime('now', 'localtime'), 0)")) {
				stmt.setString(1, eventId);
				stmt.executeUpdate();
			}

			System.out.println("[INFO] イベント作成 -> EventId=" + eventId);
			con.commit();
			return eventId;

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TEVENT: レコードの追加に失敗");
			throw e;
		}
	}

	/**
	 * 現在有効なイベントIDを取得
	 *
	 * @return イベントID
	 * @throws SQLException DBエラー
	 */
	public String getCurrentEventId() throws SQLException {
		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.createStatement();) {
			val rs = stmt.executeQuery("SELECT * FROM TEVENT WHERE is_closed = 0 ORDER BY create_time ASC LIMIT 1");

			if(rs.next()) {
				// イベントIDを取り出す
				String eventId = rs.getString("event_id");
				System.out.println("[INFO] 現在のイベント -> EventId=" + eventId);
				return eventId;
			} else {
				// 有効なイベントが存在しない
				System.out.println("[WARN] 現在のイベント -> なし");
				return null;
			}

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TEVENT: 有効なイベントの取得に失敗");
			throw e;
		}
	}

	/**
	 * 投票処理
	 *
	 * @param data 投票データ
	 * @throws SQLException DBエラー
	 * @throws UserException ユーザー操作エラー
	 * @throws EventException イベントエラー
	 */
	public void post(ModelAudiencePredict data) throws SQLException, UserException, EventException {
		try(val con = ConnectionManager.getManager().connect(this)) {

			con.setAutoCommit(false);

			// 既にイベントが締め切られていないかチェック
			try(val stmt = con.createStatement()) {
				val rs = stmt.executeQuery("SELECT * FROM TEVENT WHERE is_closed = 0 ORDER BY create_time ASC LIMIT 1");

				if(rs.next()) {
					// イベントIDを取り出す
					if(rs.getString("event_id").equals(data.getEventId())) {
						// OK
					} else {
						// 指定されているイベントIDが最新ではない
						throw new EventException("最新ではないイベントに投票しようとしました。不正なリクエストです。");
					}
				} else {
					// 有効なイベントが存在しない
					throw new EventException("既に投票が締め切られています。次のゲームが始まるまでお待ち下さい。");
				}
			}

			// 既に同じイベントに対して投稿していないかチェック
			try(val stmt = con.prepareStatement("SELECT * FROM TPREDICT WHERE event_id = ? AND user_session_id = ?")) {
				stmt.setString(1, data.getEventId());
				stmt.setString(2, data.getUserSessionId());
				val rs = stmt.executeQuery();

				if(rs.next()) {
					// 既に登録されている
					throw new UserException("既に投票されています。次のゲームが始まるまでお待ち下さい。");
				}
			}

			// イベント作成
			try(val stmt = con.prepareStatement("INSERT INTO TPREDICT VALUES (?, ?, ?, ?, datetime('now', 'localtime'), ?)")) {
				stmt.setString(1, this.createGUID());
				stmt.setString(2, data.getEventId());
				stmt.setString(3, data.getNickname());
				stmt.setInt(4, data.getPredict());
				stmt.setString(5, data.getUserSessionId());
				stmt.executeUpdate();
			}

			System.out.println("[INFO] 投票 -> EventId=" + data.getEventId() + ", NickName=" + data.getNickname() + ", Predict=" + data.getPredict() + ", UserSessionId=" + data.getUserSessionId());
			con.commit();

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TEVENT: レコードの追加に失敗");
			throw e;
		} catch(UserException e) {
			e.printStackTrace();
			System.out.println("[WARN] TEVENT: レコードの追加をキャンセル");
			throw e;
		}
	}

	/**
	 * 指定したイベントの投票データをすべて取得
	 *
	 * @param eventId イベントID
	 * @throws SQLException DBエラー
	 */
	public List<ModelAudiencePredict> getPosts(String eventId) throws SQLException {
		val list = new ArrayList<ModelAudiencePredict>();

		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.prepareStatement("SELECT * FROM TPREDICT WHERE event_id = ? ORDER BY receive_time ASC");) {

			stmt.setString(1, eventId);

			try(val rs = stmt.executeQuery()) {
				while(rs.next()) {
					list.add(new ModelAudiencePredict(
							rs.getString("predict_id"),
							eventId,
							rs.getString("name"),
							rs.getInt("predict"),
							rs.getString("receive_time"),
							null));
				}
			}

			System.out.println("[INFO] 投票データ取得 -> EventID=" + eventId + " ... " + list.size() + " 件");

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TPREDICT: レコード一覧の取得に失敗");
			throw e;
		}

		return list;
	}

	/**
	 * 投票締切処理
	 *
	 * @param eventId イベントID
	 * @throws SQLException DBエラー
	 */
	public void close(String eventId) throws SQLException {
		try(val con = ConnectionManager.getManager().connect(this)) {

			con.setAutoCommit(false);

			// 指定されたイベントを締切にする
			try(val stmt = con.prepareStatement("UPDATE TEVENT SET is_closed = 1 WHERE event_id = ?")) {
				stmt.setString(1, eventId);
				stmt.executeUpdate();
			}

			System.out.println("[INFO] 投票締切: " + eventId);
			con.commit();

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TPREDICT: レコードの更新に失敗");
			throw e;
		}
	}

	/**
	 * オーディエンス参加延べ人数取得
	 *
	 * @return オーディエンス参加延べ人数
	 * @throws SQLException DBエラー
	 */
	public int getPeopleCount() throws SQLException {
		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.createStatement();) {
			val rs = stmt.executeQuery("SELECT COUNT(*) AS people_count FROM TPREDICT");

			if(rs.next()) {
				// 投稿数を取り出す
				int count = rs.getInt("people_count");
				System.out.println("[INFO] オーディエンス参加延べ人数: " + count + " 人");
				return count;
			} else {
				// 一件も投稿されていない
				System.out.println("[INFO] オーディエンス参加延べ人数: 0 人");
				return 0;
			}

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("[ERROR] TPREDICT: レコード数の取得に失敗");
			throw e;
		}
	}

	/**
	 * GUIDを生成します。
	 * @return GUID
	 */
	private String createGUID() {
		return UUID.randomUUID().toString().replace("-", "");
	}

	@Override
	public String[] getCreateTableSQL() {
		return new String[] {
				"CREATE TABLE IF NOT EXISTS TEVENT (event_id text PRIMARY KEY, create_time datetime NOT NULL, is_closed int NOT NULL)",
				"CREATE TABLE IF NOT EXISTS TPREDICT (predict_id text PRIMARY KEY, event_id text NOT NULL, name text NOT NULL, predict int NOT NULL, receive_time datetime NOT NULL, user_session_id text NOT NULL)",
		};
	}

}
