package database;

/**
 * DAOが持つべきインターフェース
 *
 * @author tomokis
 *
 */
public interface DAOInterface {

	/**
	 * 使用するテーブルがない場合に生成するために必要なSQL文を返す
	 *
	 * @return SQL文の配列
	 */
	public String[] getCreateTableSQL();

}
