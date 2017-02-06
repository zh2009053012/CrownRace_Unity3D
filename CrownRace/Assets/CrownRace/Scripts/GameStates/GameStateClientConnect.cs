using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using com.crownrace.msg;

public class GameStateClientConnect : IStateBase {

	private GameStateClientConnect()
	{}

	private static GameStateClientConnect m_instance;
	private static object m_lockHelper = new object();
	public static GameStateClientConnect Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateClientConnect ();
				}
			}
		}
		return m_instance;
	}
	//
	MessageUI messageUI;
	GameClientUI ctr;
	public void Enter(GameStateBase owner)
	{
		GameObject messageUIPrefab = Resources.Load ("UI/MessageUICanvas")as GameObject;
		GameObject messageUIGO = GameObject.Instantiate (messageUIPrefab);
		messageUI = messageUIGO.GetComponent<MessageUI> ();
		messageUI.Init ();
		//
		GameObject clientPrefab = Resources.Load("UI/GameClientUICanvas")as GameObject;
		GameObject clientGO = GameObject.Instantiate (clientPrefab);
		ctr = clientGO.GetComponent<GameClientUI> ();
		ctr.DisConnectBtnInteractable = false;
		ctr.SetInputReadOnly( false );
		ctr.SetNotifyText ("");
		//
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.LOGIN_ACK_CMD, LoginAck, "LoginAck");
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq, "HeartbeatReq");
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.LOGIN_ACK_CMD, LoginAck);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq);
		if (null != messageUI) {
			GameObject.Destroy (messageUI.gameObject);
		}
		if (null != ctr) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("BackToPrevious")) {
			DoBackToPrevious ();
		} else if (message.Equals ("ConnectServer")) {
			DoConnectServer (parameters);
		} else if (message.Equals ("DisconnectServer")) {
			DoDisconnectServer ();
		}
	}
	void DoDisconnectServer()
	{
		TcpClientHelper.Instance.Close ();
		ctr.ConnectBtnInteractable = true;
		ctr.DisConnectBtnInteractable = false;
		ctr.SetInputReadOnly( false);
		ctr.SetNotifyText ("");
		//
		GameGlobalData.PlayerID = -1;
		GameGlobalData.PlayerResName = "";
	}

	void DoConnectServer(object[] p)
	{
		string ipStr = (string)p [0];
		string portStr = (string)p [1];
		IPAddress ip;
		int port;
		if (IPAddress.TryParse (ipStr, out ip) &&
			int.TryParse (portStr, out port)) {

			if (TcpClientHelper.Instance.Connect (ipStr, port)) {
				ctr.ConnectBtnInteractable = false;
				ctr.DisConnectBtnInteractable = true;
				ctr.SetInputReadOnly ( true);
				ctr.SetNotifyText ("连接服务器成功,等待服务器开始游戏.");

				login_req req = new login_req();
				TcpClientHelper.Instance.SendData<login_req>(NET_CMD.LOGIN_REQ_CMD, req);

			} else {
				messageUI.ShowMessage ("连接服务器失败");
			}
		} else {
			messageUI.ShowMessage ("ip地址错误");
		}
	}
	void DoBackToPrevious()
	{
		TcpClientHelper.Instance.Close ();
		//
		GameGlobalData.PlayerID = -1;
		GameGlobalData.PlayerResName = "";
		GameStateManager.Instance ().FSM.ChangeState (GameStateStart.Instance());
	}
	void LoginAck(byte[] data)
	{
		
		login_ack ack = NetUtils.Deserialize<login_ack>(data);
		Debug.Log("receive LoginAck:"+ack.data.player_id+","+ack.data.res_name);
		GameGlobalData.PlayerID = ack.data.player_id;
		GameGlobalData.PlayerResName = ack.data.res_name;
	}
	void HeartbeatReq(byte[] data)
	{
		Debug.Log("receive HeartbeatReq");
		heartbeat_req req = NetUtils.Deserialize<heartbeat_req>(data);
		if(GameGlobalData.PlayerID >= 0)
		{
			heartbeat_ack ack = new heartbeat_ack();
			ack.player_id = GameGlobalData.PlayerID;
			TcpClientHelper.Instance.SendData<heartbeat_ack>(NET_CMD.HEARTBEAT_ACK_CMD, ack);
		}
	}
}
