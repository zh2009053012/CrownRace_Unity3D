using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateEndGame : IStateBase {

	private ServerStateEndGame()
	{}

	private static ServerStateEndGame m_instance;
	private static object m_lockHelper = new object();
	public static ServerStateEndGame Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerStateEndGame ();
				}
			}
		}
		return m_instance;
	}
	//
	public void Enter(GameStateBase owner)
	{
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
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
	void PlayerRoundEnd(int player_id, byte[] data)
	{
		player_round_end_req req = NetUtils.Deserialize<player_round_end_req> (data);
		if (req.player_id == player_id) {
			Debug.Log ("PlayerRoundEnd:"+player_id);
			GameGlobalData.SetPlayerRoundEnd (player_id, true);
			if (GameGlobalData.IsAllPlayerRoundEnd ()) {
				
				TcpListenerHelper.Instance.Close ();
				GameGlobalData.ClearServerPlayerData ();
				GameGlobalData.ResetPlayerRoundEnd ();
				GameGlobalData.ResetPlayerMoveOver ();

				SceneLoading.LoadSceneName = GameGlobalData.LoginSceneName;
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
			}
		}
	}
}


