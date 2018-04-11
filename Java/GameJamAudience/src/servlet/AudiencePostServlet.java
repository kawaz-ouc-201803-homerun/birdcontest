package servlet;

import java.io.IOException;
import java.math.BigDecimal;
import java.sql.SQLException;
import java.util.Date;
import java.util.UUID;
import java.util.stream.Collectors;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import database.AudienceDAO;
import jsonable.ModelAudiencePredict;
import jsonable.ModelJSONICRequest;
import lombok.val;
import net.arnx.jsonic.JSON;

/**
 * オーディエンス投稿用サーブレット
 */
@WebServlet("/servlet/AudiencePost")
public class AudiencePostServlet extends HttpServlet {
	private static final long serialVersionUID = 1L;

    /**
     * @see HttpServlet#HttpServlet()
     */
    public AudiencePostServlet() {
        super();
    }

	/**
	 * @see HttpServlet#doGet(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
	}

	/**
	 * @see HttpServlet#doPost(HttpServletRequest request, HttpServletResponse response)
	 */
	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		// セッションを取得してユーザーセッションIDを割り振る
		val session = ((HttpServletRequest) request).getSession();
		session.setMaxInactiveInterval(10 * 60);
		String userSessionId = "";
		if(session.getAttribute("UserSessionId") == null) {
			userSessionId = UUID.randomUUID().toString().replace("-", "");
			session.setAttribute("UserSessionId", userSessionId);
			System.out.println("[新規] UserSessionId: " + userSessionId);
		} else {
			userSessionId = (String) session.getAttribute("UserSessionId");
			System.out.println("[既存] UserSessionId: " + userSessionId);
		}

		// リクエストボディのJSONからパラメーターオブジェクトを生成
		String body = request.getReader().lines().collect(Collectors.joining("\r\n"));
		val requestParameterModel = JSON.decode(body, ModelJSONICRequest.class);

		// 投稿処理実行
		try {
			val method = this.getClass().getMethod(requestParameterModel.getMethod(),
					String.class, String.class, int.class, String.class, String.class);
			method.invoke(this,
					(String) requestParameterModel.getParams()[0],
					(String) requestParameterModel.getParams()[1],
					((BigDecimal) requestParameterModel.getParams()[2]).intValue(),
					userSessionId);
		} catch(Exception e) {
			// メソッドを実行できなかった: サーバーエラー扱い
			response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
		}
	}

	/**
	 * オーディエンスの投票処理を行います。
	 *
	 * @param response レスポンスオブジェクト
	 * @param eventId イベントID
	 * @param nickname ニックネーム
	 * @param predict 予想飛距離
	 * @param userSessionId ユーザーセッションID
	 */
	public void post(HttpServletResponse response, String eventId, String nickname, int predict, String userSessionId)  {
		val dao = new AudienceDAO();
		try {
			dao.post(new ModelAudiencePredict(
					null,
					eventId,
					nickname,
					predict,
					(new Date()).toString(),
					userSessionId
				));
		} catch(SQLException e) {
			// データベースの接続に失敗した: サーバーエラー扱い
			response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
		} catch(Exception e) {
			// ユーザーのリクエストが悪い
			response.setStatus(HttpServletResponse.SC_NOT_ACCEPTABLE);
		}
	}

}
