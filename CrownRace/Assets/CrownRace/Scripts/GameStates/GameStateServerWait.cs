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
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.ALL_PLAYER_DATA_NTF_CMD, AllPlayerDataNtf, "AllPlayerDataNtf");

		GameGlobalData.ClearServerPlayerData ();
		GameGlobalData.ResetPlayerRoundEnd ();
		GameGlobalData.ResetPlayerMoveOver ();
	}

	public void Execute(GameStateBase owner)
	{
		
	}

	public void Exit(GameStateBase owner)
	{
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.LOGIN_ACK_CMD, LoginAck);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.ALL_PLAYER_DATA_NTF_CMD, AllPlayerDataNtf);
		Debug.Log ("exit GameStateServerWait");
		foreach(PlayerItemUI item in playerItemList)
		{
			if (null != item && item.gameObject != null)
				GameObject.Destroy (item.gameObject);
		}
		playerItemList.Clear ();
		if (null != ctr && null != ctr.gameObject) {
			GameObject.Destroy (ctr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("BackToStart")) {
			DoBackToStart (parameters);
		} else if (message.Equals ("StartGame")) {
			DoStartGame ();
		} else if (message.Equals ("NewClientAdd")) {
			DoNewClientAdd (parameters);
		} else if (message.Equals ("StartServer")) {
			DoStartServer (parameters);
		} else if (message.Equals ("ClientDisconnect")) {
			DoClientDisconnect (parameters);
		} 
	}
	void DoStartGame()
	{
		Debug.Log ("DoStartGame");
		if(TcpListenerHelper.Instance.clientsContainer.GetClientNum()>0)
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateStartGame.Instance ());
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
			GameGlobalData.RemoveServerPlayerData (player_id);
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
			messageUI.ShowMessage ("客户端连接失败!", DoBackToStart, null);
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
		//playerItemUI.SetInfo (player_id, resource_name, ep.Length>0?ep[0]:"");
		playerItemUI.SetInfo (player_id, resource_name, client_ip.ToLower());
		playerItemList.Add (playerItemUI);

		PlayerRoundData data = new PlayerRoundData(player_id, resource_name);
		if (GameGlobalData.AddServerPlayerData (data)) {
			Debug.Log ("add server playerdata success.");
		}
	}

	void DoBackToStart(object[] p)
	{
		Debug.Log ("DoBackToStart");
		TcpClientHelper.Instance.Close ();
		TcpListenerHelper.Instance.Close();
		GameGlobalData.PlayerID = -1;
		GameGlobalData.PlayerResName = "";
		GameStateManager.Instance().FSM.ChangeState(GameStateLaunch.Instance());
	}
	void LoginAck(byte[] data)
	{
		
		login_ack ack = NetUtils.Deserialize<login_ack>(data);
		Debug.Log("receive LoginAck:"+ack.data.player_id+","+ack.data.res_name);
		GameGlobalData.PlayerID = ack.data.player_id;
		GameGlobalData.PlayerResName = ack.data.res_name;
	}

	void AllPlayerDataNtf(byte[] data)
	{
		Debug.Log ("AllPlayerDatantf");
		all_player_data_ntf ntf = NetUtils.Deserialize<all_player_data_ntf> (data);
		foreach(player_data item in ntf.all_player)
		{
			GameGlobalData.AddClientPlayerData (new PlayerRoundData(item.player_id, item.res_name));
		}
		SceneLoading.LoadSceneName = GameGlobalData.GameSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
	}
}
