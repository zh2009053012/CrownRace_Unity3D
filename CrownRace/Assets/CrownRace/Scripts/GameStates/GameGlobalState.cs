using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using ProtoBuf;
using System.Net;
using System.Net.Sockets;

public class GameGlobalState : IStateBase {

	private GameGlobalState()
	{}

	private static GameGlobalState m_instance;
	private static object m_lockHelper = new object();
	public static GameGlobalState Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameGlobalState ();
				}
			}
		}
		return m_instance;
	}
	//
	public void Enter(GameStateBase owner)
	{
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq, "HeartbeatReq");
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.HEARTBEAT_REQ_CMD, HeartbeatReq);
	}

	public void Message(string message, object[] parameters)
	{

	}
	void HeartbeatReq(byte[] data)
	{
		//Debug.Log("receive HeartbeatReq");
		heartbeat_req req = NetUtils.Deserialize<heartbeat_req>(data);
		if(GameGlobalData.PlayerID >= 0)
		{
			heartbeat_ack ack = new heartbeat_ack();
			ack.player_id = GameGlobalData.PlayerID;
			TcpClientHelper.Instance.SendData<heartbeat_ack>(NET_CMD.HEARTBEAT_ACK_CMD, ack);
		}
	}
}
