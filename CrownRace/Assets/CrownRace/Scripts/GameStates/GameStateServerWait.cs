using UnityEngine;
using System.Collections;
using com.crownrace.msg;
using ProtoBuf;
using System.Net;
using System.Net.Sockets;

public class GameStateServerWait : IStateBase {

	private GameStateServerWait()
	{}

	private static GameStateServerWait m_instance;
	private static object m_lockHelper = new object();
	public static GameStateServerWait Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateServerWait ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameServerUI ctr;
	private MessageUI messageUI;
	private GameObject playerItemPrefab;
	public void Enter(GameStateBase owner)
	{
		GameObject messageUIPrefab = Resources.Load("UI/MessageUICanvas")as GameObject;
		GameObject messageUIGO = GameObject.Instantiate(messageUIPrefab);
		messageUI = messageUIGO.GetComponent<MessageUI>();
		//
		playerItemPrefab = Resources.Load("UI/PlayerItemUI")as GameObject;
		//
		GameObject prefab = Resources.Load ("UI/GameServerUICanvas")as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ctr = go.GetComponent<GameServerUI> ();
		//
		if(TcpListenerHelper.Instance.StartListen())
		{
			ctr.ServerIP = TcpListenerHelper.Instance.ServerIP;
		}else{
			messageUI.ShowMessage("开启服务器失败", 3, DoBackToStart);
		}
		//
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.LOGIN_ACK_CMD, LoginAck, "LoginAck");
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq, "HeartbeatReq");
		if(TcpClientHelper.Instance.Connect(TcpListenerHelper.Instance.ServerIP, 8000))
		{
			Debug.Log("connect server success.");
			login_req req = new login_req();
			req.name = "";
			req.res_name = "";
			TcpClientHelper.Instance.SendData<login_req>(NET_CMD.LOGIN_REQ_CMD, req);
			Debug.Log("send login req");
		}else{
			Debug.Log("connect server failed.");
		}
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		if (null != ctr && null != ctr.gameObject) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		if(message.Equals("BackToStart"))
		{
			DoBackToStart();
		}else if(message.Equals("StartGame"))
		{
			
		}else if(message.Equals("NewClientAdd"))
		{
			DoNewClientAdd(parameters);
		}
	}
	void DoNewClientAdd(object[] p)
	{
		int player_id = (int)p[0];
		string client_ip = (string)p[1];
		string resource_name = (string)p[2];
		GameObject go = GameObject.Instantiate(playerItemPrefab);
		go.transform.parent = ctr.gridGroupCtr.transform;
		go.transform.localScale = Vector3.one;
	}

	void DoBackToStart()
	{
		TcpListenerHelper.Instance.Close();
		GameStateManager.Instance().FSM.ChangeState(GameStateStart.Instance());
	}
	void LoginAck(byte[] data)
	{
		Debug.Log("receive LoginAck");
		login_ack ack = NetUtils.Deserialize<login_ack>(data);
		GameGlobalData.PlayerID = ack.player_id;
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
