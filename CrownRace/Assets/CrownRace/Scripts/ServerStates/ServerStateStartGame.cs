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
		GameGlobalData.ResetPlayerRoundEnd ();
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
		NotifyClientPlayerData ();

	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd);
	}

	public void Message(string message, object[] parameters)
	{

	}
	void NotifyClientPlayerData()
	{
		Debug.Log ("server NotifyCLientPlayerData:");
		all_player_data_ntf ntf = GameGlobalData.GetServerAllPlayerData ();
		foreach (player_data data in ntf.all_player) {
			PlayerRoundData roundData = new PlayerRoundData (data.player_id, false);
			GameGlobalData.AddPlayerRoundData (roundData);
		}
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<all_player_data_ntf> (NET_CMD.ALL_PLAYER_DATA_NTF_CMD, ntf);
	}
	void PlayerRoundEnd(int player_id, byte[] data)
	{
		player_round_end_req req = NetUtils.Deserialize<player_round_end_req> (data);
		if (req.player_id == player_id) {
			Debug.Log ("PlayerRoundEnd:"+player_id);
			GameGlobalData.SetPlayerRoundEnd (player_id, true);
			if (GameGlobalData.IsAllPlayerRoundEnd ()) {
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
			}
		}
	}
}
