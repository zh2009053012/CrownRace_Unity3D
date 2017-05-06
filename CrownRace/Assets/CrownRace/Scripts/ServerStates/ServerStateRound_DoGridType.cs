using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_DoGridType : Singleton<ServerStateRound_DoGridType>, IStateBase {
	private ServerRoundData.MovePlayerData moveData;
	private PlayerRoundData roundData;
	//
	private float m_waitSecond = 1;
	private float m_time;
	private bool m_isAfterCall = false;

	public void Enter(GameStateBase owner)
	{
		m_isAfterCall = false;
		moveData = ServerRoundData.CurMoveData;
		roundData = GameGlobalData.GetServerPlayerData (moveData.playerId);
		DoGridType ();
	}
	public void Execute(GameStateBase owner)
	{
		if (m_isAfterCall) {
			if (Time.time - m_time > m_waitSecond) {
				DoMoveOver ();
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
	void DoGridType(){
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
		string targetPlayer = "";

		switch (roundData.stay_grid.CellEffect) {
		case CELL_EFFECT.NONE:
			DoMoveOver ();

			break;
		case CELL_EFFECT.BACK:
			ServerRoundData.MovePlayerData data = new ServerRoundData.MovePlayerData();
			data.playerId = moveData.playerId;
			data.num = (int)roundData.stay_grid.EffectKeepRound*-1;
			ServerRoundData.MoveDataList.Add (data);
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);

			break;
		case CELL_EFFECT.FORWARD:

			ServerRoundData.MovePlayerData data2 = new ServerRoundData.MovePlayerData();
			data2.playerId = moveData.playerId;
			data2.num = (int)roundData.stay_grid.EffectKeepRound;
			ServerRoundData.MoveDataList.Add (data2);
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);

			break;
		case CELL_EFFECT.PAUSE:
			roundData.PauseNum += (int)roundData.stay_grid.EffectKeepRound;
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list [i].player_id, roundData.player_id);
				ServerRoundData.ServerMessageNtf (list [i].player_id, targetPlayer + "暂停" + roundData.stay_grid.EffectKeepRound + "回合");
			}

			m_isAfterCall = true;
			m_time = Time.time;
			m_waitSecond = 2;

			break;
		case CELL_EFFECT.ROLL_CARD:
			int cardNum = 1;
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list [i].player_id, roundData.player_id);
				ServerRoundData.ServerMessageNtf (list [i].player_id, targetPlayer + "抽取1张卡牌");
			}

			object[] pr = new object[3 + cardNum * 2];
			pr [0] = (object)roundData.player_id;
			pr [1] = (object)cardNum;

			for (int i = 0; i < cardNum; i++) {
				int config_id = Random.Range (0, GameGlobalData.CardList.Length);
				pr [3 + i] = (object)(config_id);
				CardEffect config = GameGlobalData.CardList [config_id];
				//
				pr [3 + cardNum + i] = (object)CardEffect.ID;
				CardEffect instance = new CardEffect (config_id, config.effect, config.effect_value, 
					                      config.effect2, config.effect_value2, config.select_target, config.select_value, config.name, config.desc);
				roundData.card_list.Add (instance);
			}
			pr [2] = (object)roundData.card_list.Count;

			RollCard (pr);

			m_isAfterCall = true;
			m_time = Time.time;
			m_waitSecond = 8;

			break;
//		case CELL_EFFECT.ROLL_DICE:
//			for(int i=0; i < list.Count; i++){
//				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
//				ServerMessageNtf (list[i].player_id, targetPlayer+"额外投掷一次骰子");
//			}
//			ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, true);
//			break;
		}
	}

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
	void DoMoveOver(){
		if (ServerRoundData.MoveDataList.Count > 0) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);
		} else {
			PlayerRoundData m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
			ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
			ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
			ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择结束回合或者使用卡牌");
			ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_WaitUseCardOrRollDice.Instance);
		}
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
