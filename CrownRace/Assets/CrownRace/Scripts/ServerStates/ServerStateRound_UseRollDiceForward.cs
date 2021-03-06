﻿using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using UnityEngine;

public class ServerStateRound_UseRollDiceForward : Singleton<ServerStateRound_UseRollDiceForward>, IStateBase {

	private int playerId;
	private roll_dice_req req;
	private RollTheDice m_diceCtr;
	//
	private float m_waitSecond = 1;
	private float m_time;
	private bool m_isAfterCall = false;
	private int m_diceNum;
	public void Enter(GameStateBase owner)
	{
		Debug.Log("ServerStateRound_RollDice");
		m_isAfterCall = false;

		req = NetUtils.Deserialize<roll_dice_req> (ServerRoundData.Data);
		playerId = req.player_id;

		m_diceCtr = ServerRoundData.DiceCtr;
		m_diceCtr.RegisterRollOverNotify (RollDiceCallback);
		//
		m_diceCtr.IsKinematic = false;
		m_diceCtr.Roll (15000);
	}
	public void Execute(GameStateBase owner)
	{

		if (m_isAfterCall) {
			if (Time.time - m_time > m_waitSecond) {
				//移动玩家位置 目标玩家
				for(int i=0; i<ServerRoundData.TargetList.Count; i++){
					ServerRoundData.MovePlayerData data = new ServerRoundData.MovePlayerData();
					data.playerId = ServerRoundData.TargetList[i].player_id;
					data.num = m_diceNum;
					ServerRoundData.MoveDataList.Add (data);
				}
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);
			}
		} else {
			if (null != m_diceCtr && null != m_diceCtr.Dice) {
				ServerRoundData.ServerSyncDiceNtf (m_diceCtr.Dice.activeSelf, 
					m_diceCtr.Dice.transform.position, m_diceCtr.Dice.transform.rotation);
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
	void RollDiceCallback(uint num){
		ServerRoundData.ServerSyncDiceNtf (false, m_diceCtr.Dice.transform.position, m_diceCtr.Dice.transform.rotation);

		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
		for(int i=0; i<list.Count; i++){
			string playerName = "";
			for(int j=0; j<ServerRoundData.TargetList.Count; j++){
				playerName += GetPlayerName(list[i].player_id, ServerRoundData.TargetList[j].player_id)+" ";
			}
			ServerRoundData.ServerMessageNtf(list[i].player_id, playerName + "向前移动"+num+"格");
		}
		m_isAfterCall = true;
		m_diceNum = (int)num;
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
