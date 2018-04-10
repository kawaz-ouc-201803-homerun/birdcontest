package jsonable;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * 参加者の延べ人数を集計するAPIのレスポンスクラス
 *
 * @author tomokis
 *
 */
@Data
@AllArgsConstructor
public class ModelAudienceGetPeopleCountResponse {

	/**
	 * 参加者の延べ人数
	 */
	private int count;

}
