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
	private PlayerRoundData m_curRoundPlayer;
	public void Enter(GameStateBase owner)
	{
		Debug.Log("enter ServerStateRound");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient, "SyncDicePosRotationFormClient");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromClient, "RollDiceOverNtfFromClient");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_MOVE_OVER_NTF_CMD, PlayerMoveOver, "PlayerMoveOver");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.CELL_EFFECT_ACK_CMD, CellEffectAck, "CellEffectAck");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, MoveToEndNtf, "MoveToEndNtf");
		GameGlobalData.ResetPlayerRoundEnd ();
		NotifyNextRoundPlayer ();
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log("exit ServerStateRound");
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromClient);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_MOVE_OVER_NTF_CMD, PlayerMoveOver);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.CELL_EFFECT_ACK_CMD, CellEffectAck);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, MoveToEndNtf);
		GameGlobalData.ResetPlayerRoundEnd ();
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

		if (m_curRoundPlayer.player_id == player_id) {
			GameGlobalData.RemoveServerPlayerData (player_id);
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
		} else {
//			PlayerRoundData data = GameGlobalData.GetServerPlayerData (player_id);
//			List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData ();
//			list.Remove (data);
//			bool isOthersRoundOver = true;
//			bool isOthersMoveOver = true;
//			foreach(PlayerRoundData player in list)
//			{
//				if (!player.is_move_over)
//					isOthersMoveOver = false;
//				if (!player.is_round_over)
//					isOthersRoundOver = false;
//			}
//			if (isOthersRoundOver && !data.is_round_over) {
//				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
//			} else if (isOthersMoveOver && !data.is_move_over) {
//				cell_effect_req req = new cell_effect_req ();
//				TcpListenerHelper.Instance.clientsContainer.SendToClient<cell_effect_req> (m_curRoundPlayer.player_id, NET_CMD.CELL_EFFECT_REQ_CMD, req);
//			}
			GameGlobalData.RemoveServerPlayerData (player_id);
		}
	}
	void NotifyNextRoundPlayer()
	{
		Debug.Log ("NotifyNextRoundPlayer");
		m_curRoundPlayer = GameGlobalData.GetServerNextPlayerData ();
		if (m_curRoundPlayer.PauseNum > 0) {
			m_curRoundPlayer.MinusPauseNum ();
			player_pause_ntf ntf = new player_pause_ntf ();
			ntf.player_id = m_curRoundPlayer.player_id;
			ntf.left_pause_round = m_curRoundPlayer.PauseNum;
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_pause_ntf> (NET_CMD.PLAYER_PAUSE_NTF_CMD, ntf);
		} else {
			player_roll_dice_ntf ntf = new player_roll_dice_ntf ();
			ntf.player_id = m_curRoundPlayer.player_id;
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_roll_dice_ntf> (NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, ntf);
		}
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
		Debug.Log ("RollDiceOverNtfFromClient:"+player_id);
		roll_dice_over_ntf ntf = NetUtils.Deserialize<roll_dice_over_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<roll_dice_over_ntf> (NET_CMD.ROLL_DICE_OVER_NTF_CMD, ntf);
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
	void PlayerMoveOver(int player_id, byte[] data)
	{
		player_move_over_ntf ntf = NetUtils.Deserialize<player_move_over_ntf> (data);
		if (player_id == ntf.player_id) {
			Debug.Log ("PlayerMoveOver:"+player_id);
			GameGlobalData.SetPlayerMoveMove (player_id, true);
			if (GameGlobalData.IsAllPlayerMoveOver ()) {
				cell_effect_req req = new cell_effect_req ();
				TcpListenerHelper.Instance.clientsContainer.SendToClient<cell_effect_req> (m_curRoundPlayer.player_id, NET_CMD.CELL_EFFECT_REQ_CMD, req);
			}
		}
	}
	void CellEffectAck(int player_id, byte[] data){
		GameGlobalData.ResetPlayerMoveOver ();
		cell_effect_ack ack = NetUtils.Deserialize<cell_effect_ack> (data);
		Debug.Log ("CellEffectAck:"+ack.player_id+","+ack.cell_effect);
		if (ack.player_id != player_id)
			return;
		switch (ack.cell_effect) {
		case CELL_EFFECT.NONE:
			break;
		case CELL_EFFECT.BACK:
			break;
		case CELL_EFFECT.FORWARD:
			break;
		case CELL_EFFECT.PAUSE:
			m_curRoundPlayer.PauseNum = ack.effect_num;
			break;
		case CELL_EFFECT.ROLL_CARD:
			break;
		case CELL_EFFECT.ROLL_DICE:
			break;
		}
		cell_effect_ntf ntf = new cell_effect_ntf ();
		ntf.player_id = player_id;
		ntf.cell_effect = ack.cell_effect;
		ntf.effect_num = ack.effect_num;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<cell_effect_ntf> (NET_CMD.CELL_EFFECT_NTF_CMD, ntf);
	}
	void MoveToEndNtf(int player_id, byte[] data)
	{
		move_to_end_ntf ntf = NetUtils.Deserialize<move_to_end_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateEndGame.Instance());
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<move_to_end_ntf> (NET_CMD.MOVE_TO_END_NTF_CMD, ntf);
		}
	}
}
