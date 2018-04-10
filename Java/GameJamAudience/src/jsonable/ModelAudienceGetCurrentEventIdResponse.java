package jsonable;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * 現在有効なイベントのIDを取得するAPIのレスポンスクラス
 *
 * @author tomokis
 *
 */
@Data
@AllArgsConstructor
public class ModelAudienceGetCurrentEventIdResponse {

	/**
	 * 現在有効なイベントID
	 */
	private String currentEventId;

}
