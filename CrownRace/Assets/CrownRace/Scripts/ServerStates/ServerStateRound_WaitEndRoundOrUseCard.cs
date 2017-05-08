using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_WaitEndRoundOrUseCard : Singleton<ServerStateRound_WaitEndRoundOrUseCard>, IStateBase {

	public void Enter(GameStateBase owner)
	{
		Debug.Log("ServerStateRound_WaitEndRoundOrUseCard");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq, "");
		TcpListenerHelper.Instance.RegisterNetMsg(NET_CMD.END_ROUND_REQ_CMD, ClientEndRoundReq, "");
		//客户端离线状态，服务器托管
		if (!ServerRoundData.CurRoundPlayer.is_connect) {
			TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_SelectCurRoundPlayer.Instance);
		}
	}
	public void Execute(GameStateBase owner)
	{

	}
	public void Exit(GameStateBase owner)
	{
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq);
		TcpListenerHelper.Instance.UnregisterNetMsg(NET_CMD.END_ROUND_REQ_CMD, ClientEndRoundReq);
	}
	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("ClientDisconnect")) {
			int playerId = (int)parameters [0];
			if (playerId == ServerRoundData.CurRoundPlayer.player_id) {
				if (!ServerRoundData.CurRoundPlayer.is_connect) {
					TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_SelectCurRoundPlayer.Instance);
				}
			}
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
	//
	void ClientEndRoundReq(int playerId, byte[] data){
		end_round_req req = NetUtils.Deserialize<end_round_req>(data);
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_SelectCurRoundPlayer.Instance);
	}
}
