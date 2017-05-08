using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_SelectCurRoundPlayer : Singleton<ServerStateRound_SelectCurRoundPlayer>, IStateBase {

	PlayerRoundData m_curRoundPlayer;
	private bool m_canRollDice = true;
	private float m_waitSecond = 1;
	private float m_time;
	//

	public void Enter(GameStateBase owner)
	{
		ServerRoundData.UseCardTime = false;
		ServerRoundData.RollDiceTime = false;
		ServerRoundData.HasRollDice = false;
		//
		m_canRollDice = true;
		m_curRoundPlayer = null;

		ServerRoundData.CurRoundPlayer = GameGlobalData.GetServerNextPlayerData ();
		m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
		m_curRoundPlayer.MinusAllBuffRound();

		//更新当前玩家的buff 数据
		ServerRoundData.ServerUpdateBuffDataNtf(m_curRoundPlayer.player_id, m_curRoundPlayer.buff);

		string targetPlayer = "";
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();

		if (m_curRoundPlayer.PauseNum > 0) {
			//
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list[i].player_id, m_curRoundPlayer.player_id);
				ServerRoundData.ServerMessageNtf (list[i].player_id, targetPlayer+"本回合不可投掷骰子");
			}
			m_curRoundPlayer.MinusPauseNum ();
	
			m_canRollDice = false;
			m_time = Time.time;
		} else {

			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list[i].player_id, m_curRoundPlayer.player_id);
				ServerRoundData.ServerMessageNtf (list[i].player_id, targetPlayer+"的回合");
				ServerRoundData.ServerSetUseCardStateNtf (list [i].player_id, list [i].player_id == m_curRoundPlayer.player_id);
				ServerRoundData.ServerSetDiceBtnStateNtf (list [i].player_id, list [i].player_id == m_curRoundPlayer.player_id);
				ServerRoundData.ServerSetEndRoundBtnNtf(list[i].player_id, false);
			}
			//wait player roll dice or use card
			TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_WaitUseCardOrRollDice.Instance);
		}
	}
	public void Execute(GameStateBase owner)
	{
		if (!m_canRollDice) {
			if (Time.time - m_time > m_waitSecond) {
				ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
				ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
				//
				ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择结束回合或者使用卡牌");
				ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
				//wait player UseCard or EndRound
				m_canRollDice = true;
				TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_WaitEndRoundOrUseCard.Instance);
			}
		}
	}
	public void Exit(GameStateBase owner)
	{
	}
	public void Message(string message, object[] parameters)
	{

	}
	//
	string GetPlayerName(int sendPlayerId, int dstPlayerId){
		string targetPlayer = "";
		if(sendPlayerId == dstPlayerId){
			targetPlayer = "你";
		}else{
			targetPlayer = GameGlobalData.GetServerPlayerData(dstPlayerId).res_name;
		}

		return targetPlayer;
	}
}
