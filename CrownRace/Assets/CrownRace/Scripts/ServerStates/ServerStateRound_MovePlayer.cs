using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound_MovePlayer : Singleton<ServerStateRound_MovePlayer>, IStateBase {
	private ServerRoundData.MovePlayerData moveData;
	private PlayerRoundData roundData;
	private List<Vector3> path=new List<Vector3>();
	private int index = 0;
	private static float moveSpeed = 10;
	public void Enter(GameStateBase owner)
	{
		path.Clear ();
		index = 0;
		if (ServerRoundData.MoveDataList.Count <= 0) {
			Debug.LogError ("no move data");
		}
		moveData = ServerRoundData.MoveDataList [0];
		ServerRoundData.CurMoveData = moveData;
		ServerRoundData.MoveDataList.RemoveAt (0);
		//
		Debug.Log ("MovePlayer:"+moveData.playerId+","+moveData.num);

		roundData = GameGlobalData.GetServerPlayerData (moveData.playerId);
		CalculatePath ();
		//send msg
		SendMsg();
	}
	public void Execute(GameStateBase owner)
	{
		float step = Time.deltaTime * moveSpeed;

		if (Vector3.Distance (path [index], roundData.position) <= step) {
			roundData.position = path [index];
			index++;
			//goto the end
			if (index >= path.Count) {
				//获得当前格子类型
				TcpListenerHelper.Instance.FSM.ChangeState(ServerStateRound_DoGridType.Instance);
			}
		} else {
			Vector3 dir = path [index] - roundData.position;
			roundData.position = roundData.position + dir.normalized * Time.deltaTime * moveSpeed;
		}
		ServerRoundData.ServerMovePlayerNtf (moveData.playerId, roundData.position, Quaternion.identity);

	}
	public void Exit(GameStateBase owner)
	{

	}
	public void Message(string message, object[] parameters)
	{

	}
	//
	void SendMsg(){
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData ();
		string targetPlayer = "";
		string dirStr = "向前移动";
		if (moveData.num < 0) {
			dirStr = "向后移动";
		}
		for(int i=0; i < list.Count; i++){
			targetPlayer = GetPlayerName(list[i].player_id, moveData.playerId);
			ServerRoundData.ServerMessageNtf (list[i].player_id, targetPlayer+dirStr+moveData.num+"格");
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
	//
	void CalculatePath(){
		MapGrid target = roundData.stay_grid;
		List<MapGrid> list = new List<MapGrid> ();
		int left = 0;
		if (moveData.num > 0) {
			for(int i=0; i<moveData.num; i++)
			{
				if (target.CellEffect == CELL_EFFECT.END) {
					left = moveData.num - i;
					break;
				}
				if (null == target.NextGrid) {
					break;
				}
				target = target.NextGrid;
			}
		} else {
			for(int i=0; i<Mathf.Abs(moveData.num); i++)
			{
				if (null == target.PreGird) {
					break ;
				}
				target = target.PreGird;
			}
		}
		list.Add (target);
		//
		Debug.Log("target grid:"+target.gameObject.name);
		if (target.CellEffect == CELL_EFFECT.END)
			Debug.Log ("goto end "+left);
		if (target.CellEffect == CELL_EFFECT.END && left > 0) {
			for(int i=0; i<left; i++)
			{
				if (null == target.PreGird) {
					break ;
				}
				target = target.PreGird;
			}
			list.Add (target);
		}
		roundData.stay_grid = target;
		//
		path.Clear();
		for(int i=0; i<list.Count; i++){
			NavMeshPath navPath = new NavMeshPath ();
			Vector3 srcPos = roundData.position;
			if(i != 0){
				srcPos = list[i-1].PlayerPos(roundData.res_name);
			}
			NavMesh.CalculatePath (srcPos, list[i].PlayerPos(roundData.res_name), NavMesh.AllAreas, navPath);
			Debug.Log("navPath:"+navPath.corners.Length);
			path.AddRange (navPath.corners);
		}
	}
}
