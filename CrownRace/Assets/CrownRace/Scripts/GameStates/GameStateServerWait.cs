using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using ProtoBuf;
using System.Net;
using System.Net.Sockets;

public class ThreadMessage{
	public string msg;
	public object[] parameters;
}

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
	private List<PlayerItemUI> playerItemList = new List<PlayerItemUI> ();
	#region thread msg
	//receive from TcpListener's message(the msg form other thread, but we should deal it at main thread)
	private LinkedList<ThreadMessage> msgBufferList = new LinkedList<ThreadMessage>();
	private LinkedList<ThreadMessage> msgProcessList = new LinkedList<ThreadMessage> ();
	private object msgLock = new object ();
	public void AddToMsgList(ThreadMessage msg){
		lock (msgLock) {
			msgBufferList.AddLast (msg);
		}
	}
	void DealThreadMsg()
	{
		if (msgBufferList.Count <= 0)
			return;
		lock (msgLock) {
			foreach (ThreadMessage item in msgBufferList) {
				msgProcessList.AddLast (item);
			}
			msgBufferList.Clear ();
		}
		foreach (ThreadMessage msg in msgProcessList) {
			Message (msg.msg, msg.parameters);
		}
		msgProcessList.Clear ();
	}
	#endregion

	public void Enter(GameStateBase owner)
	{
		GameObject messageUIPrefab = Resources.Load("UI/MessageUICanvas")as GameObject;
		GameObject messageUIGO = GameObject.Instantiate(messageUIPrefab);
		messageUI = messageUIGO.GetComponent<MessageUI>();
		messageUI.Init ();
		//
		playerItemPrefab = Resources.Load("UI/PlayerItemUI")as GameObject;
		//
		GameObject prefab = Resources.Load ("UI/GameServerUICanvas")as GameObject;
		GameObject go = GameObject.Instantiate (prefab);
		ctr = go.GetComponent<GameServerUI> ();
		//
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.LOGIN_ACK_CMD, LoginAck, "LoginAck");
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq, "HeartbeatReq");

	}

	public void Execute(GameStateBase owner)
	{
		DealThreadMsg ();
	}

	public void Exit(GameStateBase owner)
	{
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.LOGIN_ACK_CMD, LoginAck);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq);
		Debug.Log ("exit GameStateServerWait");
		if (null != ctr && null != ctr.gameObject) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("BackToStart")) {
			DoBackToStart ();
		} else if (message.Equals ("StartGame")) {
			
		} else if (message.Equals ("NewClientAdd")) {
			DoNewClientAdd (parameters);
		} else if (message.Equals ("StartServer")) {
			DoStartServer (parameters);
		} else if (message.Equals ("ClientDisconnect")) {
			DoClientDisconnect (parameters);
		}
	}
	void DoClientDisconnect(object[] p)
	{
		int player_id = (int)p [0];
		PlayerItemUI target = null;
		foreach(PlayerItemUI item in playerItemList) {
			if (item.PlayerID == player_id) {
				target = item;
				break;
			}
		}
		if (null != target) {
			playerItemList.Remove (target);
			GameGlobalData.RemovePlayerData (player_id);
			GameGlobalData.ResyclePlayerResName (target.ResName);
			GameObject.Destroy (target.gameObject);
		}
	}
	void DoStartServer(object[] p)
	{
		string ipstr = (string)p [0];
		string endpointstr = (string)p [1];
		IPAddress ip;
		int port;
		if (IPAddress.TryParse (ipstr, out ip) &&
			int.TryParse (endpointstr, out port)) {
			DoRealStartServer (ip, port);
		} else {
			ip = NetUtils.GetInternalIP ();
			port = 8000;
			ctr.ServerIP = ip.ToString ();
			ctr.ServerPort = port.ToString();
			DoRealStartServer (ip, port);
		}
	}

	void DoRealStartServer(IPAddress ip, int port)
	{
		if(TcpListenerHelper.Instance.StartListen(ip, port))
		{
			ctr.SetReadOnlyIPInput (true);
			ctr.SetStartBtnEnable (false);
			DoClientConnectToServer ();
		}else{
			messageUI.ShowMessage ("服务器启动失败,请检查ip地址是否正确.", 5);
		}

	}
	void DoClientConnectToServer()
	{
		if(TcpClientHelper.Instance.Connect(TcpListenerHelper.Instance.ServerIP, TcpListenerHelper.Instance.Port))
		{
			Debug.Log("connect server success.");
			login_req req = new login_req();
			TcpClientHelper.Instance.SendData<login_req>(NET_CMD.LOGIN_REQ_CMD, req);
		}else{
			messageUI.ShowMessage ("客户端连接失败!", DoBackToStart);
		}
	}

	void DoNewClientAdd(object[] p)
	{
		int player_id = (int)p[0];
		string client_ip = (string)p[1];
		string resource_name = (string)p[2];
		Debug.Log ("DoNewClientAdd:"+player_id+","+client_ip+","+resource_name);
		GameObject go = GameObject.Instantiate(playerItemPrefab);
		go.transform.parent = ctr.gridGroupCtr.transform;
		go.transform.localScale = Vector3.one;
		PlayerItemUI playerItemUI = go.GetComponent<PlayerItemUI> ();
		string[] ep = client_ip.Split (':');
		playerItemUI.SetInfo (player_id, resource_name, ep.Length>0?ep[0]:"");
		playerItemList.Add (playerItemUI);
		GameGlobalData.AddPlayerData (new PlayerData(player_id, resource_name));
	}

	void DoBackToStart()
	{
		Debug.Log ("DoBackToStart");
		TcpClientHelper.Instance.Close ();
		TcpListenerHelper.Instance.Close();
		GameGlobalData.PlayerID = -1;
		GameGlobalData.PlayerResName = "";
		GameStateManager.Instance().FSM.ChangeState(GameStateStart.Instance());
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
