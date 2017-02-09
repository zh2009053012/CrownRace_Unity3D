using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using ProtoBuf;
using System.Net;
using System.Net.Sockets;

public class ServerGlobalState : IStateBase {

	private static ServerGlobalState m_instance;
	private static object m_lockHelper = new object();
	public static ServerGlobalState Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerGlobalState ();
				}
			}
		}
		return m_instance;
	}
	//
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
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.HEARTBEAT_ACK_CMD, DoHeartbeatAck, "DoHeartbeatAck");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.LOGIN_REQ_CMD, DoLoginReq, "DOLoginReq");
	}

	public void Execute(GameStateBase owner)
	{
		DealThreadMsg ();
	}

	public void Exit(GameStateBase owner)
	{
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.HEARTBEAT_ACK_CMD, DoHeartbeatAck);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.LOGIN_REQ_CMD, DoLoginReq);
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("NotifyClientLeave")) {
			NotifyClientLeave (parameters);
		} else if (message.Equals ("ClientDisconnect")) {
			if (GameStateManager.Instance ().FSM.CurrentState == GameStateServerWait.Instance ()) {
				GameStateManager.Instance ().FSM.CurrentState.Message ("ClientDisconnect", parameters);
			}else if(TcpListenerHelper.Instance.FSM.CurrentState == ServerStateRound.Instance())
			{
				ServerStateRound.Instance ().Message ("ClientDisconnect", parameters);
			}
		}
	}

	void DoHeartbeatAck(int player_id, byte[] data)
	{

		heartbeat_ack hb = NetUtils.Deserialize<heartbeat_ack> (data);
		//Debug.Log ("server:DoHeartbearAck:"+hb.player_id);
		if (player_id == hb.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SetClientTimeStamp(player_id, GameUtils.GetUTCTimeStamp ());
		}
	}

	void DoLoginReq(int player_id, byte[] data)
	{

		login_req req = NetUtils.Deserialize<login_req> (data);
		Debug.Log ("server:receive login req:");
		login_ack ack = new login_ack ();
		ack.data = new player_data ();
		if (TcpListenerHelper.Instance.IsStopListen) {
			ack.is_success = false;
		} else {
			ack.data.player_id = player_id;
			ack.data.res_name = GameGlobalData.AllocatePlayerResName ();
			ack.is_success = true;
			if (string.IsNullOrEmpty (ack.data.res_name))
				Debug.LogError ("res_name is null.");
			object[] p = new object[3];
			p [0] = (object)player_id;
			p [1] = (object)TcpListenerHelper.Instance.clientsContainer.GetClientIP (player_id);
			p [2] = (object)ack.data.res_name;
			GameStateManager.Instance ().FSM.CurrentState.Message ("NewClientAdd", p);

		}
		TcpListenerHelper.Instance.clientsContainer.SendToClient<login_ack>(player_id, NET_CMD.LOGIN_ACK_CMD, ack);

	}

	void NotifyClientLeave(object[] ps)
	{
		int player_id = (int)ps [0];
		Debug.Log ("Notify client leave."+player_id);
		//
		leave_game_ntf ntf = new leave_game_ntf ();
		ntf.player_id = player_id;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<leave_game_ntf> (NET_CMD.LEAVE_GAME_NTF_CMD, ntf);
		//if (GameStateManager.Instance ().FSM.CurrentState == GameStateServerWait.Instance ()) {
			object[] p = new object[1];
			p [0] = (object)player_id;
			ThreadMessage msg = new ThreadMessage ();
			msg.msg = "ClientDisconnect";
			msg.parameters = p;
			AddToMsgList (msg);
		//}
	}
}
