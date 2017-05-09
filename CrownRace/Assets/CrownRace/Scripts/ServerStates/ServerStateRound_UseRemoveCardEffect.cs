using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_UseRemoveCardEffect : Singleton<ServerStateRound_UseRemoveCardEffect>, IStateBase {
	List<PlayerRoundData> targetList;
	CARD_EFFECT cardEffect;
	int effectValue;
	PlayerRoundData user;
	List<PlayerRoundData> allPlayer;
	//
	bool isAfterCall = false;
	float m_time;
	float m_waitSeconds=2;
	public void Enter(GameStateBase owner)
	{
		isAfterCall = false;
		targetList = ServerRoundData.TargetList;
		cardEffect = ServerRoundData.UseCardEffect;
		effectValue = ServerRoundData.CardEffectValue;
		user = ServerRoundData.UserRoundData;
		allPlayer = GameGlobalData.GetServerAllPlayerData ();
		//
		bool isPaused = false;
		for(int i=0; i<targetList.Count; i++){
			if (targetList [i].PauseNum > 0) {
				isPaused = true;
				continue;
			}
			if(targetList[i].player_id == ServerRoundData.CurRoundPlayer.player_id && 
				ServerRoundData.CurRoundPlayer.is_paused){
				isPaused = true;
			}
		}
		//no targets
		if (targetList.Count <= 0 || !isPaused) {
			for (int i = 0; i < allPlayer.Count; i++) {
				//string msg = "什么都没有发生";
				ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, "什么都没有发生");
			}
			//
			isAfterCall = true;
			m_time = Time.time;
			return;
		}
		//do effect
		for (int i = 0; i < allPlayer.Count; i++) {
			string msg="";
			for(int j=0; j<targetList.Count; j++){
				if (targetList [j].PauseNum > 0) {
					msg += GetPlayerName (allPlayer [i].player_id, targetList [j].player_id) + " ";
				}else if(targetList[j].player_id == ServerRoundData.CurRoundPlayer.player_id && 
					ServerRoundData.CurRoundPlayer.is_paused){
					msg += GetPlayerName (allPlayer [i].player_id, targetList [j].player_id) + " ";
				}
			}
			ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, msg+"移除暂停效果");
		}
		for(int i=0; i<targetList.Count; i++){
			if (targetList [i].PauseNum > 0) {
				targetList [i].PauseNum = 0;
			}
			if(targetList[i].player_id == ServerRoundData.CurRoundPlayer.player_id && 
				ServerRoundData.CurRoundPlayer.is_paused){
				Debug.Log("++++++++++ remove");
				ServerRoundData.CurRoundPlayer.is_paused = false;
			}
		}
		isAfterCall = true;
		m_time = Time.time;
	}
	public void Execute(GameStateBase owner)
	{
		if (isAfterCall) {
			if (Time.time - m_time > m_waitSeconds) {
				isAfterCall = false;
				TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_NextStep.Instance);
			}
		}
	}
	public void Exit(GameStateBase owner)
	{
	}
	public void Message(string message, object[] parameters)
	{

	}
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
