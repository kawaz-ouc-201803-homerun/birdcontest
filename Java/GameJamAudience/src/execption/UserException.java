package execption;

/**
 * ユーザーの操作が不正な場合にスローする例外
 *
 * @author tomokis
 *
 */
public class UserException extends Exception {

	/**
	 * コンストラクター
	 *
	 * @param message 例外メッセージ
	 */
	public UserException(String message) {
		super(message);
	}

}
