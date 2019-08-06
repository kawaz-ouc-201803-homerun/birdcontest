package common;

import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * ロガー機構クラス
 *
 * @author tomokis
 *
 */
public class Logger {

	/**
	 * 唯一のインスタンス
	 */
	static private Logger instance = new Logger();

	/**
	 * 唯一のインスタンスを返す
	 * @return ロガーオブジェクト
	 */
	static public Logger getInstance() {
		return Logger.instance;
	}

	/**
	 * シングルトン構成
	 */
	private Logger() {
	}

	/**
	 * 情報ログを出力します。
	 * プログラムの動作フローを辿るために使います。
	 *
	 * @param type 出力元クラス
	 * @param message メッセージ内容
	 */
	public void LogInfo(Class<?> type, String message) {
		System.out.println(
				"INFO  " +
				(new SimpleDateFormat("yyyy/MM/dd HH:mm:ss")).format(new Date())
						+ " [" + type.getName() + "] " + message);
	}

	/**
	 * 警告ログを出力します。
	 * 動作に影響はないが、潜在的な問題があるときに使います。
	 *
	 * @param type 出力元クラス
	 * @param message メッセージ内容
	 */
	public void LogWarn(Class<?> type, String message) {
		System.out.println(
				"WARN  " +
				(new SimpleDateFormat("yyyy/MM/dd HH:mm:ss")).format(new Date())
						+ " [" + type.getName() + "] " + message);
	}

	/**
	 * エラーログを出力します。
	 * 動作に影響があり、処理を続行できないときに使います。
	 *
	 * @param type 出力元クラス
	 * @param message メッセージ内容
	 */
	public void LogError(Class<?> type, String message) {
		System.out.println(
				"ERROR " +
				(new SimpleDateFormat("yyyy/MM/dd HH:mm:ss")).format(new Date())
						+ " [" + type.getName() + "] " + message);
	}

}
