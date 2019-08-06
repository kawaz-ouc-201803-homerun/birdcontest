using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自分のIPアドレスを表示します。
/// </summary>
public class MyIPAddressGetter : MonoBehaviour {

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Start() {
		string ipAddress = "";
		IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

		foreach(IPAddress ip in ipentry.AddressList) {
			if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
				ipAddress = ip.ToString();
				break;
			}
		}

		// UIに反映
		this.GetComponent<Text>().text = "この端末のIPアドレス ＝ " + ipAddress;
	}

}
