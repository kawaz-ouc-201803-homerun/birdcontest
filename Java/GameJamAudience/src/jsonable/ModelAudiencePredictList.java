package jsonable;

import java.util.List;
import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * オーディエンス予想クラスのリストクラス
 *
 * @author tomokis
 *
 */
@Data
@AllArgsConstructor
public class ModelAudiencePredictList {

	/// <summary>
	/// オーディエンス予想リスト
	/// </summary>
	private List<ModelAudiencePredict> AudiencePredicts;

}
