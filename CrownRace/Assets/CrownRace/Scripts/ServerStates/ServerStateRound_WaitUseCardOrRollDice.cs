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
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_REQ_CMD, ClientRollDiceReq, "");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq, "");
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

	}
	//
	void ClientUseCardReq(int playerId, byte[] data){
		ServerRoundData.PlayerId = playerId;
		ServerRoundData.Data = data;
	}
	void ClientRollDiceReq(int playerId,byte[] data){
		Debug.Log ("ClientRollDiceReq:"+playerId);
		ServerRoundData.PlayerId = playerId;
		ServerRoundData.Data = data;
		TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_RollDice.Instance);
	}
}
