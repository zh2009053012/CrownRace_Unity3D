using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_WaitUseCardOrRollDice : Singleton<ServerStateRound_WaitUseCardOrRollDice>, IStateBase {

	public void Enter(GameStateBase owner)
	{
		Debug.Log("ServerStateRound_WaitUseCardOrRollDice");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_REQ_CMD, ClientRollDiceReq, "");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq, "");
		//disconnect 
		CheckConnectState();
	}
	public void Execute(GameStateBase owner)
	{

	}
	public void Exit(GameStateBase owner)
	{
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_REQ_CMD, ClientRollDiceReq);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq);
	}
	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("ClientDisconnect")) {
			int playerId = (int)parameters [0];
			if (playerId == ServerRoundData.CurRoundPlayer.player_id) {
				CheckConnectState ();
			}
		}
	}
	//
	void CheckConnectState(){
		if (!ServerRoundData.CurRoundPlayer.is_connect) {
			roll_dice_req req = new roll_dice_req ();
			req.player_id = ServerRoundData.CurRoundPlayer.player_id;

			ClientRollDiceReq (ServerRoundData.CurRoundPlayer.player_id, NetUtils.Serialize (req));
		}
	}
	//
	void ClientUseCardReq(int playerId, byte[] data){
		ServerRoundData.PlayerId = playerId;
		ServerRoundData.Data = data;
		ServerRoundData.UseCardTime = true;
		ServerRoundData.RollDiceTime = false;
		TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_UseCard.Instance);

	}
	void ClientRollDiceReq(int playerId,byte[] data){
		Debug.Log ("ClientRollDiceReq:"+playerId);
		ServerRoundData.PlayerId = playerId;
		ServerRoundData.Data = data;
		ServerRoundData.UseCardTime = false;
		ServerRoundData.RollDiceTime = true;
		ServerRoundData.HasRollDice = true;
		TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_RollDice.Instance);
	}
}
