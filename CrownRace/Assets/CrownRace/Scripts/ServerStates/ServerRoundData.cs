using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerRoundData  {
	public class MovePlayerData{
		public int playerId;
		public int num;
	}

	public static PlayerRoundData CurRoundPlayer;
	public static RollTheDice DiceCtr;
	public static bool UseCardTime = false;
	public static bool RollDiceTime = false;
	public static bool HasRollDice = false;
	//client msg
	public static int PlayerId;
	public static byte[] Data;
	//use card data
	public static int DoCardEffectIndex = 0;
	public static CardEffect UseCardEffect;
	//roll card data
	public static int RollCardPlayerId;
	public static int RollCardNum;

	//move player data
	public static List<MovePlayerData> MoveDataList = new List<MovePlayerData>();
	//现在正在移动的MovePlayerData
	public static MovePlayerData CurMoveData;

	public static void ServerUseCardAck(int playerId, int haveCardNum, bool isSuccess, int cardInstanceId){
		use_card_ack ack = new use_card_ack();
		ack.use_player_id = playerId;
		ack.is_use_success = isSuccess;
		ack.have_card_num = haveCardNum;
		ack.card_instance_id = cardInstanceId;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<use_card_ack>(playerId, NET_CMD.USE_CARD_ACK_CMD, ack);
	}
	public static void ServerSetUseCardStateNtf(int playerId, bool canUseCard){
		set_use_card_state_ntf ntf = new set_use_card_state_ntf ();
		ntf.can_use_card = canUseCard;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_use_card_state_ntf> (playerId, NET_CMD.SET_USE_CARD_STATE_NTF_CMD, ntf);
	}
	public static void ServerMessageNtf(string msg){
		message_ntf ntf = new message_ntf ();
		ntf.msg = msg;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<message_ntf> (NET_CMD.MESSAGE_NTF_CMD, ntf);
	}
	public static void ServerMessageNtf(int playerId, string msg){
		message_ntf ntf = new message_ntf ();
		ntf.msg = msg;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<message_ntf> (playerId, NET_CMD.MESSAGE_NTF_CMD, ntf);
	}
	public static void ServerSetDiceBtnStateNtf(int playerId, bool canPressDice){
		set_dice_btn_state_ntf ntf = new set_dice_btn_state_ntf ();
		ntf.player_id = playerId;
		ntf.can_press = canPressDice;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_dice_btn_state_ntf> (playerId, NET_CMD.SET_DICE_BTN_STATE_NTF_CMD, ntf);
	}
	public static void ServerRemovePlayerCardNtf(int playerId, int[] cardInstanceId, int haveCardNum){
		remove_player_card_ntf ntf = new remove_player_card_ntf ();
		ntf.player_id = playerId;
		ntf.card_instance_id.AddRange(cardInstanceId);
		ntf.have_card_num = haveCardNum;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<remove_player_card_ntf> (NET_CMD.REMOVE_PLAYER_CARD_NTF_CMD, ntf);
	}
	public static void ServerAddPlayerCardNtf(add_player_card_ntf ntf){

		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<add_player_card_ntf> (NET_CMD.ADD_PLAYER_CARD_NTF_CMD, ntf);
	}
	public static void ServerMovePlayerNtf(int playerId, Vector3 position, Quaternion rotation){
		move_player_ntf ntf = new move_player_ntf ();
		ntf.player_id = playerId;
		ntf.pos_x = position.x;
		ntf.pos_y = position.y;
		ntf.pos_z = position.z;
		ntf.rotation_x = rotation.x;
		ntf.rotation_y = rotation.y;
		ntf.rotation_z = rotation.z;
		ntf.rotation_w = rotation.w;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<move_player_ntf> (NET_CMD.MOVE_PLAYER_NTF_CMD, ntf);
	}
	public static void ServerSyncDiceNtf(bool isActive, Vector3 position, Quaternion rotation){
		sync_dice_ntf ntf = new sync_dice_ntf ();
		ntf.is_active = isActive;
		ntf.pos_x = position.x;
		ntf.pos_y = position.y;
		ntf.pos_z = position.z;
		ntf.rotation_x = rotation.x;
		ntf.rotation_y = rotation.y;
		ntf.rotation_z = rotation.z;
		ntf.rotation_w = rotation.w;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<sync_dice_ntf> (NET_CMD.SYNC_DICE_NTF_CMD, ntf);
	}
	public static void ServerSetPlayerStateNtf(int playerId, PLAYER_STATE state, int stateRoundLeft){
		set_player_state_ntf ntf = new set_player_state_ntf ();
		ntf.player_id = playerId;
		ntf.player_state = state;
		ntf.state_round_left = stateRoundLeft;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<set_player_state_ntf> (NET_CMD.SET_PLAYER_STATE_NTF_CMD, ntf);
	}
	public static void ServerSetEndRoundBtnNtf(int playerId, bool canEndRound){
		set_end_round_btn_state_ntf ntf = new set_end_round_btn_state_ntf();
		ntf.can_end_round = canEndRound;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_end_round_btn_state_ntf>(playerId, NET_CMD.SET_END_ROUND_BTN_STATE_NTF_CMD, ntf);
	}
}
