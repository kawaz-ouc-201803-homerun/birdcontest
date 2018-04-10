package database;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
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
	 * GUIDを生成します。
	 * @return GUID
	 */
	private String getGUID() {
		return UUID.randomUUID().toString().replace("-", "");
	}

	/**
	 * 新規イベント作成
	 * 作成前に存在しているイベントはすべて締切状態にします。
	 *
	 * @return イベントID
	 */
	public String newEvent() {
		String eventId = this.getGUID();

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

			con.commit();

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TEVENT: レコードの追加に失敗");
		}

		return eventId;
	}

	/**
	 * 現在有効なイベントIDを取得
	 *
	 * @return イベントID
	 */
	public String getCurrentEventId() {
		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.createStatement();) {
			val rs = stmt.executeQuery("SELECT * FROM TEVENT WHERE is_closed = 0 ORDER BY receive_time ASC LIMIT 1");

			if(rs.next()) {
				// イベントIDを取り出す
				return rs.getString("event_id");
			} else {
				// 有効なイベントが存在しない
				return null;
			}

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TEVENT: 有効なイベントの取得に失敗");
			return null;
		}
	}

	/**
	 * 投票処理
	 *
	 * @param data 投票データ
	 */
	public void post(ModelAudiencePredict data) {
		try(val con = ConnectionManager.getManager().connect(this)) {

			con.setAutoCommit(false);

			// イベント作成
			try(val stmt = con.prepareStatement("INSERT INTO TPREDICT VALUES (?, ?, ?, ?, datetime('now', 'localtime'), ?, ?)")) {
				stmt.setString(1, this.getGUID());
				stmt.setString(2, data.getEventId());
				stmt.setString(3, data.getNickname());
				stmt.setInt(4, data.getPredict());
				stmt.setString(5, data.getIpAddress());
				stmt.setString(5, data.getOsEnvironment());
				stmt.executeUpdate();
			}

			con.commit();

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TEVENT: レコードの追加に失敗");
		}
	}

	/**
	 * 指定したイベントの投票データをすべて取得
	 *
	 * @param eventId イベントID
	 */
	public List<ModelAudiencePredict> getPosts(String eventId) {
		val list = new ArrayList<ModelAudiencePredict>();

		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.createStatement();) {
			val rs = stmt.executeQuery("SELECT * FROM TPREDICT WHERE event_id = ? ORDER BY receive_time ASC");

			while(rs.next()) {
				list.add(new ModelAudiencePredict(
						rs.getString("id"),
						rs.getString("event_id"),
						rs.getString("name"),
						rs.getInt("predict"),
						rs.getString("receive_time"),
						rs.getString("ip_address"),
						rs.getString("os_env")));
			}

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TPREDICT: レコード一覧の取得に失敗");
		}

		return list;
	}

	/**
	 * 投票締切処理
	 *
	 * @param eventId イベントID
	 */
	public void close(String eventId) {
		try(val con = ConnectionManager.getManager().connect(this)) {

			con.setAutoCommit(false);

			// 指定されたイベントを締切にする
			try(val stmt = con.prepareStatement("UPDATE TEVENT SET is_closed = 1 WHERE event_id = ?")) {
				stmt.setString(1, eventId);
				stmt.executeUpdate();
			}

			con.commit();

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TPREDICT: レコードの更新に失敗");
		}
	}

	/**
	 * オーディエンス参加延べ人数取得
	 *
	 * @return オーディエンス参加延べ人数
	 */
	public int getPeopleCount() {
		try(
			val con = ConnectionManager.getManager().connect(this);
			val stmt = con.createStatement();) {
			val rs = stmt.executeQuery("SELECT COUNT(*) AS people_count FROM TPREDICT");

			if(rs.next()) {
				// 投稿数を取り出す
				return rs.getInt("people_count");
			} else {
				// 一件も投稿されていない
				return 0;
			}

		} catch(SQLException e) {
			e.printStackTrace();
			System.out.println("TPREDICT: レコード数の取得に失敗");
			return -1;
		}
	}

	@Override
	public String[] getCreateTableSQL() {
		return new String[] {
				"CREATE TABLE IF NOT EXISTS TEVENT (id text PRIMARY KEY, create_time datetime NOT NULL, is_closed int NOT NULL)",
				"CREATE TABLE IF NOT EXISTS TPREDICT (id text PRIMARY KEY, event_id text NOT NULL, name text NOT NULL, predict int NOT NULL, receive_time datetime NOT NULL, ip_address text NOT NULL, os_env text)",
		};
	}

}
