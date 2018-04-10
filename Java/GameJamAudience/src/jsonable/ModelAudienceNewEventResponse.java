package jsonable;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * 新規イベントを作成するAPIのレスポンスクラス
 *
 * @author tomokis
 *
 */
@Data
@AllArgsConstructor
public class ModelAudienceNewEventResponse {

	/**
	 * イベントID
	 */
	private String eventId;

}
