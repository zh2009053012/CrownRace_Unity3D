using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using System.Threading;
using System.Collections.Generic;
using ProtoBuf;
using com.crownrace.msg;

public class ClientData{
	public static int count=0;
	public int id;
	public ulong timestamp;
	public TcpListener server;
	public TcpClient client;
	public NetworkStream stream;
	public Thread thread;
	private bool isThreadAlive;
	private object threadLock = new object ();

	public ClientData(TcpListener server, TcpClient c)
	{
		this.id = count++;
		timestamp = GameUtils.GetUTCTimeStamp ();
		this.server = server;
		client = c;
		stream = c.GetStream();
		isThreadAlive = true;
		thread = new Thread(new ThreadStart(Receive));
		thread.Start();
	}
	void Receive()
	{
		//Debug.Log("ready to receive data from client "+id);

		while(isThreadAlive){
			if (stream.CanRead) {
				byte[] data;
				if (NetUtils.ReceiveVarData (stream, out data)) {
					Debug.Log ("receive data from client:" + id);
					TcpListenerHelper.Instance.AddReceiveDataToBufferList(id, data);
				} else {
					//close client and tell other clients that this one is disconnected from server.
					Debug.Log("Receive:close client "+id);

					if (TcpListenerHelper.Instance.clientsContainer.RemoveClient (id)) {
						Debug.Log ("remove client success." + id);
					} else {
						Debug.Log ("remove client failed."+id);
					}
					ServerDealSystem.NotifyClientLeave (id);
					Close ();
				}
			}
		}
	}
	public bool SendData<T>(NET_CMD cmd, T message)where T:ProtoBuf.IExtensible
	{
		Debug.Log("send data to client "+id);

		packet pack = new packet ();
		pack.cmd = cmd;
		pack.payload = NetUtils.Serialize (message);

		if (!NetUtils.SendVarData (stream, NetUtils.Serialize (pack))) {
			Debug.Log ("send data failed."+id);
			return false;
		} 
		return true;
	}
	public void Close()
	{
		Debug.Log ("CLientData::Close "+id);
		try{
			if(null != thread)
			{
				lock (threadLock) {
					isThreadAlive = false;
				}
			}
			if(null != stream)
				stream.Close();
			if (null != client) {
				client.Close ();
				count--;
			}
		}catch(Exception err) {
			Debug.Log ("close error:"+err.Message);
		}
	}
	~ClientData()
	{
		Debug.Log ("~ClientData");
		Close ();
	}
}

public class ClientsContainer{
	List<ClientData> clientList = new List<ClientData>();
	object lockClient = new object();

	public ClientsContainer(){
	}

	public string GetClientIP(int player_id)
	{
		lock(lockClient)
		{
			foreach (ClientData cd in clientList) {
				if (cd.id == player_id) {
					return cd.client.Client.LocalEndPoint.ToString();
				}
			}
		}
		return "";
	}

	public bool SetClientTimeStamp(int player_id, ulong timestamp){
		lock (lockClient) {
			foreach (ClientData cd in clientList) {
				if (cd.id == player_id) {
					cd.timestamp = timestamp;
					return true;
				}
			}
		}
		return false;
	}

	public bool SendToClient<T>(int player_id, NET_CMD cmd, T msg)where T:ProtoBuf.IExtensible{
		lock (lockClient) {
			foreach (ClientData cd in clientList) {
				if (cd.id == player_id) {
					return cd.SendData<T> (cmd, msg);
				}
			}
		}
		return false;
	}

	public void SendToAllClient<T>(NET_CMD cmd, T msg)where T:ProtoBuf.IExtensible{
		Debug.Log ("ready to send to all client.");
		lock (lockClient) {
			Debug.Log ("send to all client."+clientList.Count);
			foreach (ClientData cd in clientList) {
				if (cd.SendData<T> (cmd, msg)) {
					Debug.Log ("send to player "+cd.id+" success");
				} else {
					Debug.Log ("send to player "+cd.id+" failed.");
				}
			}
		}
	}

	public void AddClient(ClientData cd)
	{
		if(cd != null)
		lock (lockClient) {
			clientList.Add (cd);
		}
	}
	public bool RemoveClient(int player_id)
	{
		Debug.Log ("remove client "+player_id);
		lock (lockClient) {
			int index = -1;
			int count = 0;
			foreach (ClientData cd in clientList) {
				if (cd.id == player_id) {
					index = count;
					break;
				}
				count++;
			}
			if (index >= 0) {
				clientList.RemoveAt (index);
				return true;
			} 
		}
		return false;
	}
	public bool RemoveClient(TcpClient client)
	{
		if (null == client)
			return false;
		lock (lockClient) {
			int index = -1;
			int count = 0;
			foreach (ClientData cd in clientList) {
				if (cd.client.Client.RemoteEndPoint == client.Client.RemoteEndPoint) {
					index = count;
					break;
				}
				count++;
			}
			if (index >= 0) {
				clientList.RemoveAt (index);
				return true;
			}
		}
		return false;
	}
	public bool CloseClient(int player_id)
	{
		Debug.Log ("clientcontainer::close client "+player_id);
		lock (lockClient) {
			try{
				int index = -1;
				int count = 0;
				foreach (ClientData cd in clientList) {
					if(cd == null)
					{
						Debug.Log("clientdata is null.");
						continue;
					}
					if (cd.id == player_id) {
						index = count;
						break;
					}
					count++;
				}
				Debug.Log("index, count:"+index+","+count);
				if (index >= 0) {
					clientList [index].Close ();
					Debug.Log("close over");
					clientList.RemoveAt (index);
					Debug.Log("remove over");
					return true;
				}
			}catch(Exception err) {
				Debug.Log ("close client error:"+err.Message);
			}
		}
		return false;
	}
	public void CloseAllClient()
	{
		lock (lockClient) {
			foreach (ClientData cd in clientList) {
				cd.Close ();
			}
			clientList.Clear ();
		}
	}

}

public class TcpListenerHelper : MonoBehaviour {

	#region single model
	private static TcpListenerHelper instance;
	private static object lockHelper = new object ();
	private static int mainThreadID=-1;
	public static TcpListenerHelper Instance{
		get{ 
			if (instance == null) {
				lock (lockHelper) {
					if (instance == null) {
						if (mainThreadID == -1 || Thread.CurrentThread.ManagedThreadId == mainThreadID) {
							GameObject go = new GameObject ("TcpListenerHelper");
							instance = go.AddComponent<TcpListenerHelper> ();
							if (mainThreadID == -1) {
								mainThreadID = Thread.CurrentThread.ManagedThreadId;
							}
						}
					}
				
				}
			}
			return instance;
		}
	}
	void Awake()
	{
		DontDestroyOnLoad (this);
	}
	#endregion

	TcpListener server;

	#region listen client connect
	public ClientsContainer clientsContainer = new ClientsContainer();
	private IPAddress serverIP;
	public string ServerIP{
		get{return serverIP.ToString();}
	}
	private int port;
	public int Port{
		get{ return port;}
	}
	Thread connectThread;
	bool isConnectThreadAlive;
	object connectThreadLock = new object();

	public bool StartListen(IPAddress ip, int port)
	{
		try{
			serverIP = ip;
			this.port = port;
			server = new TcpListener(ip, port);
			server.Start ();
		}catch(Exception err) {
			Debug.Log ("start server failed."+err.Message);
			return false;
		}
		Debug.Log("Waiting for a client...");
		isConnectThreadAlive = true;
		connectThread = new Thread (new ThreadStart(WaitForConcept));
		connectThread.Start ();
		//
		isSendTickThreadAlive = true;
		sendTickThread = new Thread (new ThreadStart(SendTickToClient));
		sendTickThread.Start ();
		return true;
	}
	public void Close()
	{
		clientsContainer.CloseAllClient ();
		if (null != connectThread) {
			lock (connectThreadLock) {
				isConnectThreadAlive = false;
			}
		}
		if (null != sendTickThread) {
			lock (sendTickThreadLock) {
				isSendTickThreadAlive = false;
			}
		}
		if(null != server)
			server.Stop();
	}
		
	void WaitForConcept()
	{
		while(isConnectThreadAlive)
		{
			while(isConnectThreadAlive && !server.Pending())
			{
				Thread.Sleep(1000);
			}
			TcpClient client = server.AcceptTcpClient();

			ClientData data;
			clientsContainer.RemoveClient(client);
			
			ClientData newConnect = new ClientData(server, client);

			clientsContainer.AddClient (newConnect);
			Debug.Log("concept a new client:"+newConnect.id+","+newConnect.client.Client.RemoteEndPoint.ToString());

		}
	}
	#endregion

	#region do client request 
	public class ReceiveClientData
	{
		public int player_id;
		public byte[] data;
		public ReceiveClientData(){}
		public ReceiveClientData(int player_id, byte[] data){
			this.player_id = player_id;
			this.data = data;
		}
	}
	private LinkedList<ReceiveClientData> receiveDataBufferList = new LinkedList<ReceiveClientData>();
	private LinkedList<ReceiveClientData> receiveDataProcessList = new LinkedList<ReceiveClientData>();
	private object receiveDataLock = new object();

	public void AddReceiveDataToBufferList(int player_id, byte[] data)
	{
		lock (receiveDataLock) {
			receiveDataBufferList.AddLast (new ReceiveClientData(player_id, data));
		}
	}
	//should be called by main thread
	void DealReceiveData()
	{

		if (receiveDataBufferList.Count > 0) {
			lock (receiveDataLock) {
				foreach (ReceiveClientData item in receiveDataBufferList) {
					receiveDataProcessList.AddLast (item);
				}
				receiveDataBufferList.Clear ();
			}
			foreach (ReceiveClientData item in receiveDataProcessList) {
				//call by other model
				if (item != null) {
					ServerDealSystem.DoClientReq (item);
				}
			}
			receiveDataProcessList.Clear ();
		}

	}

	#endregion

	#region send tick to clients
	private Thread sendTickThread;
	private bool isSendTickThreadAlive;
	private object sendTickThreadLock = new object();
	void SendTickToClient()
	{
		while (isSendTickThreadAlive) {
			
			//check timestamp
//			ulong timestamp = GameUtils.GetUTCTimeStamp();
//			int disconnect = -1;
//			lock (m_lockClient) {
//				for (int i = clientList.Count - 1; i >= 0; i--) {
//					if (clientList [i] != null) {
//						if (timestamp - clientList [i].timestamp >= 10000) {
//							disconnect = clientList [i].id;
//						}
//					}
//				}
//			}
//			if (disconnect >= 0) {
//				TcpListenerHelper.Instance.CloseClient(disconnect);
//			}
			//
			Debug.Log("send tick");
			try{
				heartbeat_req hb = new heartbeat_req ();
				hb.server_timestamp = GameUtils.GetUTCTimeStamp ();

				clientsContainer.SendToAllClient<heartbeat_req> (NET_CMD.HEARTBEAT_REQ_CMD, hb);
			}catch(Exception err) {
				Debug.Log ("send tick to client error:"+err.Message);
				lock (sendTickThreadLock) {
					isSendTickThreadAlive = false;
				}
			}

			Thread.Sleep (1000);
		}
	}

	#endregion

	void Update()
	{
		DealReceiveData();
	}

	void OnApplicationQuit()
	{
		Debug.Log("quit");
		Close();
	}
}
