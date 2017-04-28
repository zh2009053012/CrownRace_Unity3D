using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using ProtoBuf;
using System.Net;
using System.Net.Sockets;

public class ServerStateStartGame : IStateBase {

	private static ServerStateStartGame m_instance;
	private static object m_lockHelper = new object();
	public static ServerStateStartGame Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerStateStartGame ();
				}
			}
		}
		return m_instance;
	}
	//

	public void Enter(GameStateBase owner)
	{
		Debug.Log ("enter ServerStateStart");
		//GameGlobalData.ResetPlayerRoundEnd ();
		//TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
		GameGlobalData.ResetPlayerLoadGameOverData();
		GameGlobalData.IsServer = true;
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.GAME_LOAD_OVER_REQ, ClientGameLoadOverReq, "");
		NotifyClientPlayerData ();
		TcpListenerHelper.Instance.IsStopListen = true;
	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		//TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.GAME_LOAD_OVER_REQ, ClientGameLoadOverReq);
	}

	public void Message(string message, object[] parameters)
	{

	}
	void NotifyClientPlayerData()
	{
		Debug.Log ("server NotifyCLientPlayerData:");
		all_player_data_ntf ntf = new all_player_data_ntf ();
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData ();
		foreach (PlayerRoundData data in list) {
			player_data item = new player_data ();
			item.player_id = data.player_id;
			item.res_name = data.res_name;
			ntf.all_player.Add (item);
		}
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<all_player_data_ntf> (NET_CMD.ALL_PLAYER_DATA_NTF_CMD, ntf);
	}
	void ClientGameLoadOverReq(int playerId, byte[] data){
		game_load_over_req req = NetUtils.Deserialize<game_load_over_req> (data);
		Debug.Log("ClientGameLoadOverReq:"+playerId+","+req.player_id);
		GameGlobalData.SetPlayerLoadGameOver (req.player_id, true);
		if (GameGlobalData.IsAllPlayerLoadGameOver ()) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
		}
	}
	void PlayerRoundEnd(int player_id, byte[] data)
	{
		player_round_end_req req = NetUtils.Deserialize<player_round_end_req> (data);
		Debug.Log ("PlayerRoundEnd:"+player_id+","+req.player_id);
		if (req.player_id == player_id) {
			GameGlobalData.SetPlayerRoundEnd (player_id, true);
			if (GameGlobalData.IsAllPlayerRoundEnd ()) {
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
			}
		}
	}
}
