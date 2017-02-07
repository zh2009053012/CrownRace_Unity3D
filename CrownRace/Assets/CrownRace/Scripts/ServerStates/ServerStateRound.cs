using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound : IStateBase {

	private ServerStateRound()
	{}

	private static ServerStateRound m_instance;
	private static object m_lockHelper = new object();
	public static ServerStateRound Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerStateRound ();
				}
			}
		}
		return m_instance;
	}
	//
	private player_data m_curRoundPlayer;
	public void Enter(GameStateBase owner)
	{
		Debug.Log("enter ServerStateRound");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient, "SyncDicePosRotationFormClient");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF, RollDiceOverNtfFromClient, "RollDiceOverNtfFromClient");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
		GameGlobalData.ResetPlayerRoundEnd ();
		SendPlayerRollDiceNtf ();
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log("exit ServerStateRound");
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF, RollDiceOverNtfFromClient);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd);
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("ClientDisconnect")) {
			DoClientDisconnect (parameters);	
		}
	}
	void DoClientDisconnect(object[] p){
		
		int player_id = (int)p [0];
		Debug.Log ("Server DoClientDisconnect"+player_id);
		GameGlobalData.RemoveServerPlayerData (player_id);
		GameGlobalData.RemovePlayerRoundData (player_id);
		if (m_curRoundPlayer.player_id == player_id) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
		}
	}
	void SendPlayerRollDiceNtf()
	{
		Debug.Log ("SendPlayerRollDiceNtf");
		m_curRoundPlayer = GameGlobalData.GetServerNextPlayerData ();
		player_roll_dice_ntf ntf = new player_roll_dice_ntf ();
		ntf.player_id = m_curRoundPlayer.player_id;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_roll_dice_ntf> (NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, ntf);
	}
	void SyncDicePosRotationFormClient(int player_id, byte[] data)
	{
		dice_sync_ntf ntf = NetUtils.Deserialize<dice_sync_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SendToAllClientExcept<dice_sync_ntf> (player_id, NET_CMD.DICE_SYNC_NTF_CMD, ntf);
		}
	}
	void RollDiceOverNtfFromClient(int player_id, byte[] data)
	{
		roll_dice_over_ntf ntf = NetUtils.Deserialize<roll_dice_over_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<roll_dice_over_ntf> (NET_CMD.ROLL_DICE_OVER_NTF, ntf);
		}
	}
	void PlayerRoundEnd(int player_id, byte[] data)
	{
		player_round_end_req req = NetUtils.Deserialize<player_round_end_req> (data);
		if (req.player_id == player_id) {
			Debug.Log ("PlayerRoundEnd:"+player_id);
			GameGlobalData.SetPlayerRoundEnd (player_id, true);
			if (GameGlobalData.IsAllPlayerRoundEnd ()) {
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
			}
		}
	}
}
