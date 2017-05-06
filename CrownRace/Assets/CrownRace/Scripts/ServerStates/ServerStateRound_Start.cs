using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;


public class ServerStateRound_Start : Singleton<ServerStateRound_Start>, IStateBase {

	public void Enter(GameStateBase owner)
	{
		GameObject dicePrefab = Resources.Load ("RollDice")as GameObject;
		GameObject diceGO = GameObject.Instantiate (dicePrefab);
		ServerRoundData.DiceCtr = diceGO.GetComponent<RollTheDice> ();
		//ServerRoundData.DiceCtr.RegisterRollOverNotify (RollCallback);

		//
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
		for (int i = 0; i < list.Count; i++) {
			SetPlayerStartPos (list [i].player_id);
		}
		TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound_SelectCurRoundPlayer.Instance);
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
	//
	void SetPlayerStartPos(int playerId){
		PlayerRoundData data = GameGlobalData.GetServerPlayerData (playerId);
		data.stay_grid = Map.Instance.StartGrid;
		Vector3 pos = Map.Instance.StartGrid.PlayerPos (data.res_name);
		ServerRoundData.ServerMovePlayerNtf (playerId, pos, Quaternion.identity);
	}
}
