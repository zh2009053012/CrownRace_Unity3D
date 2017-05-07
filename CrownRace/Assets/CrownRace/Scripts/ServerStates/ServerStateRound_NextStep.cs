using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_NextStep : Singleton<ServerStateRound_NextStep>, IStateBase {

	public void Enter(GameStateBase owner)
	{
		Debug.Log("ServerStateRound_NextStep");
		//先判断move data是否还有数据，若有，继续移动(优先级比use card大)
		if (ServerRoundData.MoveDataList.Count > 0) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);
		} //再判断card effect的第二个效果
		else if(ServerRoundData.DoCardEffectIndex == 1){
			TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_UseCard.Instance);
		}else {
			//roll dice time
			if(ServerRoundData.RollDiceTime){
				//结束回合或者使用卡牌
				PlayerRoundData m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
				ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
				ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
				ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择结束回合或者使用卡牌");
				ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
				Debug.Log("---------------------------");
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_WaitEndRoundOrUseCard.Instance);
			}else //if(ServerRoundData.UseCardTime){
			{
				//结束回合或者使用卡牌
				if(ServerRoundData.HasRollDice){
					PlayerRoundData m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
					ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
					ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
					ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择结束回合或者使用卡牌");
					ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
					TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_WaitEndRoundOrUseCard.Instance);
				}else{//投掷骰子或者使用卡牌
					PlayerRoundData m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
					ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
					ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, true);
					ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择投掷骰子或者使用卡牌");
					ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, false);
					TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_WaitUseCardOrRollDice.Instance);
				}
			}
		}


	}
	public void Execute(GameStateBase owner)
	{

	}
	public void Exit(GameStateBase owner)
	{

	}
	public void Message(string message, object[] parameters)
	{

	}
}
