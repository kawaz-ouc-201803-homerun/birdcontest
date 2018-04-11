package jsonable;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * オーディエンス予想クラス
 *
 * @author tomokis
 *
 */
@Data
@AllArgsConstructor
public class ModelAudiencePredict {

	/// <summary>
	/// 投稿ID
	/// </summary>
	private String id;

	/// <summary>
	/// イベントID
	/// </summary>
	private String eventId;

	/// <summary>
	/// 投稿者名
	/// </summary>
	private String nickname;

	/// <summary>
	/// 予想値
	/// </summary>
	private int predict;

	/// <summary>
	/// 投稿日時
	/// </summary>
	private String receiveTime;

	/// <summary>
	/// ユーザーセッションID
	/// </summary>
	private String userSessionId;

}
