package servlet;

import java.io.IOException;
import java.math.BigDecimal;
import java.sql.SQLException;
import java.util.UUID;
import java.util.stream.Collectors;
import javax.servlet.ServletException;
import javax.servlet.annotation.WebServlet;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import common.Logger;
import database.AudienceDAO;
import execption.EventException;
import execption.UserException;
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
			Logger.getInstance().LogInfo(AudiencePostServlet.class, "新規 UserSessionId: " + userSessionId);
		} else {
			userSessionId = (String) session.getAttribute("UserSessionId");
			Logger.getInstance().LogInfo(AudiencePostServlet.class, "既存 UserSessionId: " + userSessionId);
		}

		// リクエストボディのJSONからパラメーターオブジェクトを生成
		String body = request.getReader().lines().collect(Collectors.joining("\r\n"));
		val requestParameterModel = JSON.decode(body, ModelJSONICRequest.class);

		// 投稿処理実行
		try {
			val method = this.getClass().getMethod(requestParameterModel.getMethod(),
					HttpServletResponse.class, String.class, String.class, int.class, String.class);
			boolean result = (boolean) method.invoke(this,
					response,
					(String) requestParameterModel.getParams()[0],
					(String) requestParameterModel.getParams()[1],
					((BigDecimal) requestParameterModel.getParams()[2]).intValue(),
					userSessionId);

			if(result == true) {
				// レスポンス正常処理
				response.setStatus(HttpServletResponse.SC_OK);
			}

			// 空のJSONを返す
			response.setCharacterEncoding("UTF-8");
			response.setContentType("application/json");
			response.getWriter().write("{}");

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
	 * @return 正常に終了したかどうか
	 */
	public boolean post(HttpServletResponse response, String eventId, String nickname, int predict, String userSessionId) {
		val dao = new AudienceDAO();
		try {

			dao.post(new ModelAudiencePredict(
					null,
					eventId,
					nickname,
					predict,
					null,
					userSessionId));

			return true;

		} catch(SQLException e) {
			// データベースの接続に失敗した: サーバーエラー扱い
			response.setStatus(HttpServletResponse.SC_INTERNAL_SERVER_ERROR);
		} catch(UserException e) {
			// ユーザーのリクエストが悪い
			response.setStatus(HttpServletResponse.SC_NOT_ACCEPTABLE);
		} catch(EventException e) {
			// イベントの問題
			response.setStatus(HttpServletResponse.SC_SERVICE_UNAVAILABLE);
		}

		return false;
	}

}
