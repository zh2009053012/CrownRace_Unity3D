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
		Debug.Log("ServerStateRound_DoGridType");
		m_isAfterCall = false;
		moveData = ServerRoundData.CurMoveData;
		roundData = GameGlobalData.GetServerPlayerData (moveData.playerId);
		buff_data buff = roundData.HasBuff(BUFF_EFFECT.BUFF_BLOCK_GRID);
		if(null != buff && buff.keep_round > 0){
			DoMoveOver ();
		}else{
			DoGridType ();
		}
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
		case CELL_EFFECT.END:
			//胜利
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list [i].player_id, roundData.player_id);
				ServerRoundData.ServerMessageNtf (list [i].player_id, targetPlayer + "获得了胜利");
			}
			ServerRoundData.ServerVectoryNtf();
			break;
		case CELL_EFFECT.START:
			DoMoveOver();
			break;
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
			ServerRoundData.RollCardNum = 1;
			ServerRoundData.RollCardPlayerId = roundData.player_id;
			TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_RollCard.Instance);

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

	void DoMoveOver(){
		TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_NextStep.Instance);
//		if (ServerRoundData.MoveDataList.Count > 0) {
//			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_MovePlayer.Instance);
//		} else {
//			PlayerRoundData m_curRoundPlayer = ServerRoundData.CurRoundPlayer;
//			ServerRoundData.ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
//			ServerRoundData.ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
//			ServerRoundData.ServerMessageNtf(m_curRoundPlayer.player_id, "你可以选择结束回合或者使用卡牌");
//			ServerRoundData.ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
//			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_WaitUseCardOrRollDice.Instance);
//		}
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
