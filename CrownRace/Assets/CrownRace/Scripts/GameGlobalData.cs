using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData{
	public int player_id;
	public string res_name;
	public PlayerData(){
	}
	public PlayerData(int id, string res){
		player_id = id;
		res_name = res;
	}
}

public class GameGlobalData {

	public static int PlayerID=-1;
	public static string PlayerResName="";
	#region player res name
	static LinkedList<string> PlayerResNameList = new LinkedList<string> ();
	static LinkedList<string> UsedPlayerResNameList = new LinkedList<string> ();
	static void Init(){
		PlayerResNameList.AddLast ("PlayerA");
		PlayerResNameList.AddLast ("PlayerB");
		PlayerResNameList.AddLast ("PlayerC");
		PlayerResNameList.AddLast ("PlayerD");
	}
	public static string AllocatePlayerResName()
	{
		if (UsedPlayerResNameList.Count == 4) {
			return "";
		}
		if (PlayerResNameList.Count == 0) {
			Init ();
		}
		string player = PlayerResNameList.First.Value;
		PlayerResNameList.RemoveFirst ();
		UsedPlayerResNameList.AddLast (player);
		return player;
	}
	public static bool ResyclePlayerResName(string playerResName){
		LinkedListNode<string> node = UsedPlayerResNameList.Find(playerResName);
		if (null != node) {
			UsedPlayerResNameList.Remove (node);
			PlayerResNameList.AddLast (playerResName);
			return true;
		}
		return false;
	}
	#endregion
	#region player data
	private static List<PlayerData> playerList = new List<PlayerData>();
	private static int index=0;
	public static PlayerData GetNextPlayerData()
	{
		PlayerData target = playerList [index];
		index++;
		index = index >= playerList.Count ? 0 : index;
		return target;
	}
	public static PlayerData GetPlayerData(int player_id)
	{
		foreach (PlayerData item in playerList) {
			if (item.player_id == player_id)
				return item;
		}
		return null;
	}
	public static bool AddPlayerData(PlayerData data)
	{
		if (null == data)
			return false;
		if (GetPlayerData (data.player_id) == null) {
			playerList.Add (data);
			return true;
		}
		return false;
	}
	public static bool RemovePlayerData(int player_id)
	{
		PlayerData data = GetPlayerData (player_id);
		if (data == null)
			return false;
		playerList.Remove (data);
		return true;
	}

	#endregion
}
