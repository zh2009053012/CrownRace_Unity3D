﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;

public enum CARD_EFFECT{
	FORWARD=1,
	BACK=2,
	PAUSE=3,
	GOD_TIME=4,
	DOUBLE_DICE_NUM=5,
}

public class CardEffect{
	private static int id=0;
	public static int ID{
		get{return id++;}
	}
	public int instance_id;
	public CARD_EFFECT effect;
	public int effect_value;
	public string name;
	public string desc;
	public CardEffect(){
		instance_id=0;
		effect = CARD_EFFECT.FORWARD;
		effect_value = 0;
		name="";
		desc="";
	}
	public CardEffect(int id, CARD_EFFECT effect, int effect_value, string name, string desc){
		this.instance_id = id;
		this.effect = effect;
		this.effect_value = effect_value;
		this.name = name;
		this.desc = desc;
	}
}

public class PlayerRoundData{
	public int player_id;
	public string res_name;
	public bool is_round_over;
	public bool is_move_over;
	public List<CardEffect> card_list = new List<CardEffect>();
	public CardEffect GetCardEffect(int id){
		foreach(CardEffect ce in card_list){
			if (ce.instance_id == id){
				return ce;
			}
		}
		return null;
	}
	public bool RemoveCardEffect(int id){
		CardEffect ce = GetCardEffect(id);
		if(ce != null)
			return card_list.Remove(ce);
		return false;
	}

	private int pause_num;
	public int PauseNum{
		get{ return pause_num;}
		set{ pause_num = value;}
	}
	public void MinusPauseNum(){
		pause_num--;
		pause_num = pause_num < 0 ? 0 : pause_num;
	}

	public PlayerRoundData(){}
	public PlayerRoundData(int id, string res, bool isRoundOver, bool isMoveOver){
		player_id = id;
		res_name = res;
		is_round_over = isRoundOver;
		is_move_over = isMoveOver;
		pause_num = 0;
	}
	public PlayerRoundData(int id, string res){
		player_id = id;
		res_name = res;
		is_round_over = false;
		is_move_over = false;
		pause_num = 0;
	}

}

public class GameGlobalData {

	public static int PlayerID=-1;
	public static string PlayerResName="";
	#region scene name define
	public static string LoginSceneName = "LoginScene";
	public static string GameSceneName = "GameScene";
	public static string LoadSceneName = "LoadScene";
	public static string ShowCardSceneName = "ShowCardScene";
	#endregion

	#region client player data list
	private static List<PlayerRoundData> m_clientPlayerList = new List<PlayerRoundData> ();
	public static List<PlayerRoundData> GetClientPlayerDataList()
	{
		List<PlayerRoundData> list = new List<PlayerRoundData> ();
		foreach (PlayerRoundData data in m_clientPlayerList) {
			list.Add (data);
		}
		return list;
	}
	public static bool AddClientPlayerData(PlayerRoundData data)
	{
		if (null == data)
			return false;
		if (null == GetClientPlayerData (data.player_id)) {
			m_clientPlayerList.Add (data);
			return true;
		}
		return false;
	}
	public static PlayerRoundData GetClientPlayerData(int player_id){
		foreach (PlayerRoundData data in m_clientPlayerList) {
			if (data.player_id == player_id) {
				return data;
			}
		}
		return null;
	}
	public static bool RemoveClientPlayerData(int player_id){
		PlayerRoundData data = GetClientPlayerData (player_id);
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
	public static void InitResName(){
		PlayerResNameList.Clear ();
		UsedPlayerResNameList.Clear ();
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
			InitResName ();
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
	private static List<PlayerRoundData> m_serverPlayerList = new List<PlayerRoundData>();
	private static int index=0;
	public static void ClearServerPlayerData()
	{
		m_serverPlayerList.Clear ();
	}
	public static List<PlayerRoundData> GetServerAllPlayerData()
	{
		List<PlayerRoundData> list = new List<PlayerRoundData> ();
		foreach (PlayerRoundData data in m_serverPlayerList) {
			list.Add (data);
		}
		return list;
	}
	public static PlayerRoundData GetServerNextPlayerData()
	{
		index = index >= m_serverPlayerList.Count ? 0 : index;
		PlayerRoundData target = m_serverPlayerList [index];
		index++;
		index = index >= m_serverPlayerList.Count ? 0 : index;
		return target;
	}
	public static PlayerRoundData GetServerPlayerData(int player_id)
	{
		foreach (PlayerRoundData item in m_serverPlayerList) {
			if (item.player_id == player_id)
				return item;
		}
		return null;
	}
	public static bool AddServerPlayerData(PlayerRoundData data)
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
		PlayerRoundData data = GetServerPlayerData (player_id);
		if (data == null)
			return false;
		m_serverPlayerList.Remove (data);
		return true;
	}

	public static bool SetPlayerRoundEnd(int player_id, bool is_over)
	{
		PlayerRoundData data = GetServerPlayerData (player_id);
		if (data != null) {
			data.is_round_over = is_over;
			return true;
		}
		return false;
	}
	public static void ResetPlayerRoundEnd()
	{
		foreach (PlayerRoundData data in m_serverPlayerList) {
			data.is_round_over = false;	
		}
	}
	public static bool IsAllPlayerRoundEnd()
	{
		foreach (PlayerRoundData data in m_serverPlayerList) {
			if (!data.is_round_over) {
				return false;
			}
		}
		return true;
	}
	public static bool SetPlayerMoveMove(int player_id, bool is_move_over)
	{
		PlayerRoundData data = GetServerPlayerData (player_id);
		if (data != null) {
			data.is_move_over = is_move_over;
			return true;
		}
		return false;
	}
	public static void ResetPlayerMoveOver()
	{
		foreach (PlayerRoundData data in m_serverPlayerList) {
			data.is_move_over = false;	
		}
	}
	public static bool IsAllPlayerMoveOver()
	{
		foreach (PlayerRoundData data in m_serverPlayerList) {
			if (!data.is_move_over) {
				return false;
			}
		}
		return true;
	}
	#endregion
	//int id, CARD_EFFECT effect, int effect_value, int round_num, string name, string desc
	public static CardEffect[] CardList = new CardEffect[]{
		new CardEffect(0, CARD_EFFECT.FORWARD, 1, "阿克蒙的诱导", "使目标后退三格"),
		new CardEffect(1, CARD_EFFECT.FORWARD, 2, "白鸟的忠诚", "后边的玩家前进一格"),
		new CardEffect(2, CARD_EFFECT.FORWARD, 3, "冰冻之心", "其他玩家投掷一次骰子"),
		new CardEffect(3, CARD_EFFECT.BACK, 1, "盾牌", "格挡特殊格子的效果，持续一回合"),
		new CardEffect(4, CARD_EFFECT.BACK, 2, "狡猾的矮人货商", "使目标后退一格"),
		new CardEffect(5, CARD_EFFECT.BACK, 3, "荆棘之花", "其他玩家失去手中的第一张牌"),
		new CardEffect(6, CARD_EFFECT.PAUSE, 1, "礼物", "与目标玩家交换位置"),
		new CardEffect(7, CARD_EFFECT.PAUSE, 1, "乱葬岗", "前方三格范围内的玩家暂停一回合"),
		new CardEffect(8, CARD_EFFECT.PAUSE, 1, "命运", "额外投掷一次骰子"),
		new CardEffect(9, CARD_EFFECT.PAUSE, 1, "魔法旅行者", "手中没有卡牌的玩家前进一格"),
		new CardEffect(10, CARD_EFFECT.PAUSE, 1, "魔法仆从的欺骗", "拥有两张以上卡牌的玩家后退一格"),
		new CardEffect(11, CARD_EFFECT.PAUSE, 1, "魔法球", "抽取两张卡牌"),
		new CardEffect(12, CARD_EFFECT.PAUSE, 1, "魔镜", "反弹其他玩家的卡牌效果，持续一回合。"),
		new CardEffect(13, CARD_EFFECT.PAUSE, 1, "森林之友", "后退两格并额外投掷一次骰子"),
		new CardEffect(14, CARD_EFFECT.PAUSE, 1, "神秘信封", "额外投掷一次骰子"),
		new CardEffect(15, CARD_EFFECT.PAUSE, 1, "树藤", "使目标角色暂停1回合"),
		new CardEffect(16, CARD_EFFECT.PAUSE, 1, "死亡之手", "抽取任意一位玩家手上的第一张牌"),
		new CardEffect(17, CARD_EFFECT.PAUSE, 1, "死亡之瞳", "落在有标记格子的玩家后退一格并暂停一回合"),
		new CardEffect(18, CARD_EFFECT.PAUSE, 1, "巫师帽", "投掷一次骰子，投掷点数决定目标角色后退格数"),
		new CardEffect(19, CARD_EFFECT.PAUSE, 1, "武士之剑", "解除身上其他玩家的卡牌效果"),
		new CardEffect(20, CARD_EFFECT.PAUSE, 1, "漩涡", "格挡其他玩家施放的卡牌效果，持续一回合"),
		new CardEffect(21, CARD_EFFECT.PAUSE, 1, "诱惑之石", "投掷一次骰子并后退两格"),
		new CardEffect(22, CARD_EFFECT.PAUSE, 1, "鱼人的选择", "后退两格来获取一次额外投掷骰子的机会"),
		new CardEffect(23, CARD_EFFECT.PAUSE, 1, "预见", "其他玩家前进一格"),
		new CardEffect(24, CARD_EFFECT.PAUSE, 1, "遇袭", "所有玩家后退一格"),
		new CardEffect(25, CARD_EFFECT.PAUSE, 1, "运气", "前面的玩家前进一格"),
		new CardEffect(26, CARD_EFFECT.PAUSE, 1, "指南针", "前面和后面的玩家互换位置"),
	};
}
