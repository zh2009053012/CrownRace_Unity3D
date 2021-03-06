﻿using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;
using UnityEngine;

public class ServerStateRound_UseRollCard : Singleton<ServerStateRound_UseRollCard>, IStateBase {

	private float m_waitSecond = 1;
	private float m_time;
	private bool m_isAfterCall = false;
	//
	int rollCardNum;
	private List<PlayerRoundData> targetList;

	public void Enter(GameStateBase owner)
	{
		targetList = ServerRoundData.TargetList;
		//ServerRoundData.UseCardEffect = m_effectType;
		rollCardNum = ServerRoundData.CardEffectValue;
		//ServerRoundData.UserRoundData = user;

		Debug.Log("ServerStateRound_UseRollCard");

		List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData();
		string targetPlayer="";
		for (int i = 0; i < allPlayer.Count; i++) {
			for(int j=0; j<targetList.Count; j++){
				targetPlayer += GetPlayerName (allPlayer [i].player_id, targetList[j].player_id)+" ";
			}
			ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, targetPlayer + "抽取"+rollCardNum+"张卡牌");
		}
		for(int m=0; m<targetList.Count; m++){
			PlayerRoundData roundData = GameGlobalData.GetServerPlayerData(targetList[m].player_id);
			object[] pr = new object[3 + rollCardNum * 2];
			pr [0] = (object)roundData.player_id;
			pr [1] = (object)rollCardNum;

			for (int i = 0; i < rollCardNum; i++) {
				int config_id = Random.Range (0, GameGlobalData.CardList.Length);
				//for testing
				//config_id = 11;
				pr [3 + i] = (object)(config_id);
				CardEffect config = GameGlobalData.CardList [config_id];
				//
				int instanceId = CardEffect.ID;
				pr [3 + rollCardNum + i] = (object)instanceId;
				CardEffect instance = new CardEffect (instanceId, config_id, config.effect, config.effect_value, 
					config.effect2, config.effect_value2, config.select_target, config.select_value, config.name, config.desc);
				roundData.card_list.Add (instance);
			}
			pr [2] = (object)roundData.card_list.Count;

			RollCard (pr);
		}
		m_isAfterCall = true;
		m_time = Time.time;
		m_waitSecond = 2;
	}
	public void Execute(GameStateBase owner)
	{
		if (m_isAfterCall) {
			if (Time.time - m_time > m_waitSecond) {
				//next step
				m_isAfterCall = false;
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
	//
	void RollCard(object[] p)
	{
		Debug.Log("RollCard");
		add_player_card_ntf ntf = new add_player_card_ntf();

		int playerId = (int)p[0];
		int num = (int)p[1];
		int haveCardNum = (int)p[2];
		ntf.player_id = playerId;
		ntf.have_card_num = haveCardNum;
		for(int i=0; i<num; i++)
		{
			ntf.card_config_id.Add((int)p[3+i]);
		}
		for(int i=0; i<num; i++){
			ntf.card_instance_id.Add((int)p[3+num+i]);
		}
		ServerRoundData.ServerAddPlayerCardNtf(ntf);
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
