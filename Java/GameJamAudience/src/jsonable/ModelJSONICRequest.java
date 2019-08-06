package jsonable;

import lombok.Data;

/**
 * JSONICリクエストクラス
 *
 * @author tomokis
 *
 */
@Data
public class ModelJSONICRequest {

	/**
	 * メソッド名
	 */
	private String method;

	/**
	 * 引数リスト
	 */
	private Object[] params;

	/**
	 * JSONICリクエストID
	 * nullにするとレスポンスボディを返さなくなる仕様です。
	 */
	private Object id;

}
