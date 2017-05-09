using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_UseExchangePosition : Singleton<ServerStateRound_UseExchangePosition>, IStateBase {

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

		//如果targetList.Count < 1，则什么都不发生
		if (targetList.Count < 1) {
			for (int i = 0; i < allPlayer.Count; i++) {
				string msg = "什么都没有发生";
				ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, msg);
			}
			//
			isAfterCall = true;
			m_time = Time.time;
			return;
		}
		string bounceMsg = "";
		//如果目标只有一个，则是和使用者交换位置
		if(targetList.Count == 1){
			//如果目标就是使用者，什么都没发生
			if(user.player_id == targetList[0].player_id){
				for (int i = 0; i < allPlayer.Count; i++) {
					string msg = "什么都没有发生";
					ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, msg);
				}
				//
				isAfterCall = true;
				m_time = Time.time;
				return;
			}else{
				//否则检查目标身上是否有抵消buff
				if(HasBlockBuff()){
					return;
				}
				//是否有反弹buff，有则也抵消
				if(HasBounceBuff()){
					return;
				}
				//最后添加使用者到list，产生效果
				targetList.Add(user);
			}
		}else{
			//如果目标玩家中有buff，检查是否反弹或者抵消
			if(HasBlockBuff()){
				return;
			}
			//

			if (HasBounceBuff ()) {
				//反弹两次，什么都没发生
				if (targetList [0].player_id == targetList [1].player_id) {
					for (int i = 0; i < allPlayer.Count; i++) {
						string msg = "卡牌效果被反弹了，但什么都没有发生";
						ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, msg);
					}
					//
					isAfterCall = true;
					m_time = Time.time;
					return;
				} else {
					bounceMsg = "卡牌效果被反弹了";
				}
			}
		}
		//卡牌产生效果
		for (int i = 0; i < allPlayer.Count; i++) {
			string msg = "交换位置";
			string playerA = GetPlayerName (allPlayer[i].player_id, targetList[0].player_id);
			string playerB = GetPlayerName (allPlayer[i].player_id, targetList[1].player_id);
			ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, bounceMsg+playerA+"和"+playerB+msg);
		}
		MapGrid AGrid = targetList [0].stay_grid;
		MapGrid BGrid = targetList [1].stay_grid;
		targetList [0].stay_grid = BGrid;
		targetList[0].position = BGrid.PlayerPos(targetList[0].res_name);
		targetList [1].stay_grid = AGrid;
		targetList[1].position = AGrid.PlayerPos(targetList[1].res_name);
		ServerRoundData.ServerMovePlayerNtf (targetList [0].player_id, BGrid.PlayerPos(targetList [0].res_name), Quaternion.identity);
		ServerRoundData.ServerMovePlayerNtf (targetList [1].player_id, AGrid.PlayerPos(targetList [1].res_name), Quaternion.identity);

		isAfterCall = true;
		m_time = Time.time;
		return;
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
	//
	bool HasBlockBuff(){
		bool hasBlock = false;
		//检查是否有抵消buff
		for (int i = 0; i < targetList.Count; i++) {
			buff_data buff = targetList [i].HasBuff (BUFF_EFFECT.BUFF_BLOCK_CARD);
			if (null != buff) {
				hasBlock = true;
				targetList [i].MinusBuffRound (BUFF_EFFECT.BUFF_BLOCK_CARD);
				ServerRoundData.ServerUpdateBuffDataNtf (targetList [i].player_id, targetList [i].buff);
			}
		}
		if (hasBlock) {
			List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData ();
			for (int i = 0; i < allPlayer.Count; i++) {
				string msg = "卡牌效果被抵消了";
				ServerRoundData.ServerMessageNtf (allPlayer [i].player_id, msg);
			}
			isAfterCall = true;
			m_time = Time.time;
		}
		return hasBlock;
	}
	bool HasBounceBuff(){
		//检查是否有反弹buff
		bool hasBounce = false;
		for (int i = 0; i < targetList.Count; i++) {
			buff_data buff = targetList [i].HasBuff (BUFF_EFFECT.BUFF_BLOCK_CARD);
			if (null != buff) {
				hasBounce = true;
				targetList [i].MinusBuffRound (BUFF_EFFECT.BUFF_BLOCK_CARD);
				ServerRoundData.ServerUpdateBuffDataNtf (targetList [i].player_id, targetList [i].buff);
				targetList [i] = user;
			}
		}
		return hasBounce;
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
