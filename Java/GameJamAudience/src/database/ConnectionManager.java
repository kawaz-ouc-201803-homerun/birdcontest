package database;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import common.Logger;
import lombok.val;

/**
 * データベース接続管理クラス
 *
 * @author tomokis
 *
 */
public class ConnectionManager {

	/**
	 * データベースの接続文字列
	 */
	static private final String DB_CONNECTION_STRING = "jdbc:sqlite:/usr/local/src/tomcat/sqlite/GameJamAudience.db";
	// static private final String DB_CONNECTION_STRING = "jdbc:sqlite:E:/GitBucket/kawaz_ouc_201803_homerun/Java/GameJamAudience.db";

	/**
	 * シングルトン
	 */
	private ConnectionManager() {
	}

	/**
	 * 内部保持のインスタンス
	 */
	static private ConnectionManager mgr = new ConnectionManager();

	/**
	 * 唯一のインスタンスを返す
	 *
	 * @return このクラスのインスタンス
	 */
	static public ConnectionManager getManager() {
		return ConnectionManager.mgr;
	}

	/**
	 * データベースに接続する
	 * try-with-resources構文と共に用い、必ずcloseすること
	 *
	 * @param tableInitializer テーブルの初期化用SQLを発行するデータベース接続媒介オブジェクト
	 * @return コネクションオブジェクト
	 * @throws SQLException 接続失敗
	 */
	public Connection connect(DAOInterface tableInitializer) throws SQLException {
		// データベースに接続: ファイルがない場合は作成される
		try {
			Class.forName("org.sqlite.JDBC");
		} catch(ClassNotFoundException e) {
			e.printStackTrace();
			Logger.getInstance().LogError(ConnectionManager.class, "JDBCドライバーの初期化に失敗しました。データベース関連の操作は無効です");
		}

		if(tableInitializer != null) {
			// 今回のコネクションで使用するテーブルの初期化のSQL発行
			try(
				val con = DriverManager.getConnection(DB_CONNECTION_STRING);
				val stmt = con.createStatement()) {
				val sqls = tableInitializer.getCreateTableSQL();
				con.setAutoCommit(false);
				for(String sql : sqls) {
					stmt.execute(sql);
				}
				con.commit();
			}
		}

		return DriverManager.getConnection(DB_CONNECTION_STRING);
	}

}
