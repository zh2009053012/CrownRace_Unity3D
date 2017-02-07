using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;

public class PlayerRoundData{
	public int player_id;
	public bool is_round_over;
	public PlayerRoundData(){}
	public PlayerRoundData(int id, bool isOver){
		player_id = id;
		is_round_over = isOver;
	}
}

public class GameGlobalData {

	public static int PlayerID=-1;
	public static string PlayerResName="";
	#region scene name define
	public static string LoginSceneName = "LoginScene";
	public static string GameSceneName = "GameScene";
	public static string LoadSceneName = "LoadScene";
	#endregion

	#region client player data list
	private static List<player_data> m_clientPlayerList = new List<player_data> ();
	public static List<player_data> GetClientPlayerDataList()
	{
		List<player_data> list = new List<player_data> ();
		foreach (player_data data in m_clientPlayerList) {
			list.Add (data);
		}
		return list;
	}
	public static bool AddClientPlayerData(player_data data)
	{
		if (null == data)
			return false;
		if (null == GetClientPlayerData (data.player_id)) {
			m_clientPlayerList.Add (data);
			return true;
		}
		return false;
	}
	public static player_data GetClientPlayerData(int player_id){
		foreach (player_data data in m_clientPlayerList) {
			if (data.player_id == player_id) {
				return data;
			}
		}
		return null;
	}
	public static bool RemoveClientPlayerData(int player_id){
		player_data data = GetClientPlayerData (player_id);
		if (null != data) {
			return m_clientPlayerList.Remove (data);
		}
		return false;
	}
	public static void ClearClientPlayerData()
	{
		m_clientPlayerList.Clear ();
	}
	#endregion

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
	#region server player data
	private static List<player_data> m_serverPlayerList = new List<player_data>();
	private static int index=0;
	public static all_player_data_ntf GetServerAllPlayerData()
	{
		all_player_data_ntf ntf = new all_player_data_ntf ();
		foreach (player_data data in m_serverPlayerList) {
			ntf.all_player.Add (data);
		}
		return ntf;
	}
	public static player_data GetServerNextPlayerData()
	{
		index = index >= m_serverPlayerList.Count ? 0 : index;
		player_data target = m_serverPlayerList [index];
		index++;
		index = index >= m_serverPlayerList.Count ? 0 : index;
		return target;
	}
	public static player_data GetServerPlayerData(int player_id)
	{
		foreach (player_data item in m_serverPlayerList) {
			if (item.player_id == player_id)
				return item;
		}
		return null;
	}
	public static bool AddServerPlayerData(player_data data)
	{
		if (null == data)
			return false;
		if (GetServerPlayerData (data.player_id) == null) {
			m_serverPlayerList.Add (data);
			return true;
		}
		return false;
	}
	public static bool RemoveServerPlayerData(int player_id)
	{
		player_data data = GetServerPlayerData (player_id);
		if (data == null)
			return false;
		m_serverPlayerList.Remove (data);
		return true;
	}

	#endregion

	#region server round data
	private static List<PlayerRoundData> m_playerRoundDataList = new List<PlayerRoundData>();
	public static bool AddPlayerRoundData(PlayerRoundData data)
	{
		PlayerRoundData prd = GetPlayerRoundData (data.player_id);
		if (null == prd) {
			m_playerRoundDataList.Add (data);
			return true;
		}
		return false;
	}
	public static bool RemovePlayerRoundData(int player_id){
		PlayerRoundData data = GetPlayerRoundData (player_id);
		if (null != data) {
			return m_playerRoundDataList.Remove (data);
		}
		return false;
	}
	public static PlayerRoundData GetPlayerRoundData(int player_id)
	{
		foreach (PlayerRoundData data in m_playerRoundDataList) {
			if (data.player_id == player_id) {
				return data;
			}
		}
		return null;
	}
	public static bool SetPlayerRoundEnd(int player_id, bool is_over)
	{
		PlayerRoundData data = GetPlayerRoundData (player_id);
		if (data != null) {
			data.is_round_over = is_over;
			return true;
		}
		return false;
	}
	public static void ResetPlayerRoundEnd()
	{
		foreach (PlayerRoundData data in m_playerRoundDataList) {
			data.is_round_over = false;	
		}
	}
	public static bool IsAllPlayerRoundEnd()
	{
		foreach (PlayerRoundData data in m_playerRoundDataList) {
			if (!data.is_round_over) {
				return false;
			}
		}
		return true;
	}
	#endregion
}
