using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_UseCard : Singleton<ServerStateRound_UseCard>, IStateBase {

	PlayerRoundData userRoundData;
	CardEffect useCard;
	List<PlayerRoundData> targets = new List<PlayerRoundData>();
	//
	private float m_waitSecond = 1;
	private float m_time;
	private bool m_isAfterCall = false;
	//
	CARD_EFFECT m_effectType;
	int m_effectValue;

	public void Enter(GameStateBase owner)
	{
		Debug.Log("ServerStateRound_UseCard");
		m_isAfterCall = false;
		targets.Clear();
		//
		use_card_req req = NetUtils.Deserialize<use_card_req>(ServerRoundData.Data);

		userRoundData = GameGlobalData.GetServerPlayerData(req.use_player_id);
		useCard = userRoundData.GetCardEffect(req.card_instance_id);
		//如果是null，说明卡牌已经被移除，第一个效果已经处理完毕
		if(null == useCard){
			useCard = ServerRoundData.UseCardEffect;
			if(null == useCard){
				Debug.LogError("no card");
			}
			ServerRoundData.UseCardEffect = null;
		}else{//否则说明是处理第一个效果,将卡牌存起来,方便第二个效果调用
			ServerRoundData.UseCardEffect = useCard;
		}
		//检查第二个卡牌效果是否为none，若是none则结束
		if(ServerRoundData.DoCardEffectIndex == 1){
			if(useCard.effect2 == CARD_EFFECT.NONE){
				EndUseCard();
				return;
			}
		}
		//
		if(ServerRoundData.DoCardEffectIndex == 0){
			m_effectType = useCard.effect;
			m_effectValue = useCard.effect_value;
		}else if(ServerRoundData.DoCardEffectIndex == 1){
			m_effectType = useCard.effect2;
			m_effectValue = useCard.effect_value2;
		}
		//广播使用卡牌的消息
		if(ServerRoundData.DoCardEffectIndex == 0){
			List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData();
			for(int i=0; i<allPlayer.Count; i++){
				string playerName = GetPlayerName(allPlayer[i].player_id, userRoundData.player_id);
				string cardName = useCard.name;
				if(useCard.effect == CARD_EFFECT.BOUNCE_CARD_EFFECT || 
					useCard.effect == CARD_EFFECT.BLOCK_CARD_EFFECT){
					if(allPlayer[i].player_id != userRoundData.player_id)
						cardName = "陷阱卡";
				}
				ServerRoundData.ServerMessageNtf(allPlayer[i].player_id, playerName + "使用了"+cardName);
			}
		}
		//
		userRoundData.RemoveCardEffect(req.card_instance_id);
		//销毁玩家使用的卡牌
		if(ServerRoundData.DoCardEffectIndex == 0){
			ServerRoundData.ServerRemovePlayerCardNtf(req.use_player_id, new int[]{useCard.instance_id}, userRoundData.card_list.Count);
		}
		//
		m_isAfterCall = true;
		m_waitSecond = 2;
		m_time = Time.time;
		//筛选目标
		targets.Clear();
		targets = TryGetTargets(useCard, userRoundData, req.target_player_id);
	}
	public void Execute(GameStateBase owner)
	{
		if(m_isAfterCall){
			if(Time.time - m_time > m_waitSecond){
				m_isAfterCall = false;
				//执行卡牌效果
				DoCardEffect(targets, useCard, userRoundData);
			}
		}
	}
	public void Exit(GameStateBase owner)
	{
		
	}
	public void Message(string message, object[] parameters)
	{

	}
	#region card effect
	void EndUseCard(){
		ServerRoundData.DoCardEffectIndex = 0;
		ServerRoundData.UseCardTime = false;
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_NextStep.Instance);
	}
	bool DoCardEffect(List<PlayerRoundData> list, CardEffect ce, PlayerRoundData user){

		switch(m_effectType){
		case CARD_EFFECT.BACK:
			DoBackEffect(list, ce, user);
			break;
		case CARD_EFFECT.BLOCK_CARD_EFFECT:
			break;
		case CARD_EFFECT.BLOCK_GRID_EFFECT:
			break;
		case CARD_EFFECT.BOUNCE_CARD_EFFECT:
			break;
		case CARD_EFFECT.EXCHANGE_POSITION:
			break;
		case CARD_EFFECT.FORWARD:
			DoForwardEffect(list, ce, user);
			break;
		case CARD_EFFECT.LOST_FIRST_CARD:
			break;
		case CARD_EFFECT.PAUSE:
			break;
		case CARD_EFFECT.REMOVE_CARD_EFFECT:
			break;
		case CARD_EFFECT.ROLL_CARD:
			break;
		case CARD_EFFECT.ROLL_DICE_BACK:
			break;
		case CARD_EFFECT.ROLL_DICE_FORWARD:
			break;
		case CARD_EFFECT.ROLL_PLAYER_CARD:
			break;
		}
		ServerRoundData.DoCardEffectIndex += 1;
		return true;
	}
	void DoForwardEffect(List<PlayerRoundData> list, CardEffect ce, PlayerRoundData user){
		//ServerRoundData.ServerUseCardAck(user.player_id, user.card_list.Count, true, ce.instance_id);
		if(list.Count <= 0){
			List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData();
			for(int i=0; i<allPlayer.Count; i++){
				ServerRoundData.ServerMessageNtf(allPlayer[i].player_id, "什么都没有发生");
			}
			EndUseCard();
			return;
		}
		//
		ServerRoundData.MoveDataList.Clear();
		for(int i=0; i<list.Count; i++){
			ServerRoundData.MovePlayerData moveData = new ServerRoundData.MovePlayerData();
			moveData.playerId = list[i].player_id;
			moveData.num = m_effectValue;
			ServerRoundData.MoveDataList.Add(moveData);
		}
		//
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_MovePlayer.Instance);
	}

	void DoBackEffect(List<PlayerRoundData> list, CardEffect ce, PlayerRoundData user){
		//ServerRoundData.ServerUseCardAck(user.player_id, user.card_list.Count, true, ce.instance_id);
		if(list.Count <= 0){
			List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData();
			for(int i=0; i<allPlayer.Count; i++){
				ServerRoundData.ServerMessageNtf(allPlayer[i].player_id, "什么都没有发生");
			}
			return;
		}
		//
		ServerRoundData.MoveDataList.Clear();
		for(int i=0; i<list.Count; i++){
			ServerRoundData.MovePlayerData moveData = new ServerRoundData.MovePlayerData();
			moveData.playerId = list[i].player_id;
			moveData.num = -1*m_effectValue;
			ServerRoundData.MoveDataList.Add(moveData);
		}
		//
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_MovePlayer.Instance);
	}
	#endregion
	//
	List<PlayerRoundData> TryGetTargets(CardEffect ce, PlayerRoundData userRoundData, int targetId){
		List<PlayerRoundData> list = new List<PlayerRoundData>();
		List<PlayerRoundData> allPlayer = GameGlobalData.GetServerAllPlayerData();
		switch(ce.select_target){
		case SELECT_TARGET.BACK:
			list = GetBackTargets(ce, userRoundData, allPlayer);
			break;
		case SELECT_TARGET.ALL_PLAYER:
			return allPlayer;
			break;
		case SELECT_TARGET.FORWARD:
			list = GetForwardTargets(ce, userRoundData, allPlayer);
			break;
		case SELECT_TARGET.FORWARD_BACK:
			list = GetForwardBackTargets(userRoundData, allPlayer);
			break;
		case SELECT_TARGET.HAVE_CARD_EQUAL:
			list = GetHaveCardEqual(ce, allPlayer);
			break;
		case SELECT_TARGET.HAVE_CARD_LARGER:
			list = GetHaveCardLarger(ce, allPlayer);
			break;
		case SELECT_TARGET.HAVE_CARD_LARGER_EQUAL:
			list = GetHaveCardLargerEqual(ce, allPlayer);
			break;
		case SELECT_TARGET.HAVE_CARD_LESS:
			list = GetHaveCardLess(ce, allPlayer);
			break;
		case SELECT_TARGET.HAVE_CARD_LESS_EQUAL:
			list = GetHaveCardLessEqual(ce, allPlayer);
			break;
		case SELECT_TARGET.MANUAL:
			list.Add(GameGlobalData.GetServerPlayerData(targetId));
			break;
		case SELECT_TARGET.OTHER:
			list = GetOthers(allPlayer, userRoundData);
			break;
		case SELECT_TARGET.PLAYER_IN_SPECIAL_GRID:
			list = GetPlayerInSpecialGrid(allPlayer);
			break;
		case SELECT_TARGET.RANDOM:
			list = GetRandomPlayer(ce, allPlayer);
			break;
		case SELECT_TARGET.SELF:
			list.Add(userRoundData);
			break;
		}
		return list;
	}
	List<PlayerRoundData> GetRandomPlayer(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count > 0){
				result.Add(list[i]);
			}
		}
		if(result.Count > 1){
			int random = Random.Range(0, result.Count);
			PlayerRoundData target = result[random];
			result.Clear();
			result.Add(target);
		}
		return result;
	}
	List<PlayerRoundData> GetPlayerInSpecialGrid(List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].stay_grid.CellEffect != CELL_EFFECT.NONE ||
				list[i].stay_grid.CellEffect != CELL_EFFECT.START || 
				list[i].stay_grid.CellEffect != CELL_EFFECT.END){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetOthers(List<PlayerRoundData> list, PlayerRoundData user){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].player_id != user.player_id){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetHaveCardLargerEqual(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count >= ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetHaveCardLarger(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count > ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetHaveCardLessEqual(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count <= ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetHaveCardLess(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count < ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetHaveCardEqual(CardEffect ce, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].card_list.Count == ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetForwardBackTargets(PlayerRoundData user, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].stay_grid.ID < user.stay_grid.ID){
				result.Add(list[i]);
				break;
			}
		}
		for(int i=0; i<list.Count; i++){
			if(list[i].stay_grid.ID > user.stay_grid.ID){
				result.Add(list[i]);
				break;
			}
		}
		return result;
	}
	List<PlayerRoundData> GetForwardTargets(CardEffect ce, PlayerRoundData user, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].stay_grid.ID - user.stay_grid.ID > ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
	}
	List<PlayerRoundData> GetBackTargets(CardEffect ce, PlayerRoundData user, List<PlayerRoundData> list){
		List<PlayerRoundData> result = new List<PlayerRoundData>();
		for(int i=0; i<list.Count; i++){
			if(list[i].stay_grid.ID - user.stay_grid.ID < ce.select_value){
				result.Add(list[i]);
			}
		}
		return result;
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
