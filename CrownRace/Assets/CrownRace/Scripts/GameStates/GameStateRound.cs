using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class GameStateRound : IStateBase {

	private GameStateRound()
	{}

	private static GameStateRound m_instance;
	private static object m_lockHelper = new object();
	public static GameStateRound Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateRound ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameMainScene m_owner;
	private MessageUI m_messageUICtr;
	private GameRoundUI m_roundUICtr;
	private RollTheDice m_diceCtr;
	private bool m_isMovingOver = true;
	private bool m_isSyncDice = false;

	private List<Player> m_playerList = new List<Player> ();
	private Player m_localPlayer;
	public Player GetPlayer(int player_id){
		foreach (Player item in m_playerList) {
			if (item.PlayerID == player_id) {
				return item;
			}
		}
		return null;
	}

	public void Enter(GameStateBase owner)
	{
		Debug.Log ("enter GameStateRound");
		m_owner = (GameMainScene)owner;
		//
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, PlayerRollDiceNtf, "PlayerRollDiceNtf");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFromServer, "SyncDicePosRotationFromServer");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF, RollDiceOverNtfFromServer, "RollDiceOverNtfFromServer");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.LEAVE_GAME_NTF_CMD, PlayerLeaveNtf, "PlayerLeaveNtf");
		//
		GameObject messageUIPrefab = Resources.Load ("UI/MessageUICanvas")as GameObject;
		GameObject messageUIGO = GameObject.Instantiate (messageUIPrefab);
		m_messageUICtr = messageUIGO.GetComponent<MessageUI> ();
		m_messageUICtr.Init ();
		//
		GameObject roundUIPrefab = Resources.Load("UI/GameStateRoundUICanvas")as GameObject;
		GameObject roundUIGO = GameObject.Instantiate (roundUIPrefab);
		m_roundUICtr = roundUIGO.GetComponent<GameRoundUI> ();
		m_roundUICtr.RollDiceBtn.interactable = false;
		//
		GameObject dicePrefab = Resources.Load ("RollDice")as GameObject;
		GameObject diceGO = GameObject.Instantiate (dicePrefab);
		m_diceCtr = diceGO.GetComponent<RollTheDice> ();
		m_diceCtr.RegisterRollOverNotify (RollCallback);
		//
		InitPlayers();
		SendServerRoundEnd ();
	}

	public void Execute(GameStateBase owner)
	{
		m_owner = (GameMainScene)owner;
		if (m_isSyncDice) {
			SyncDicePosRotationToServer ();
		}
	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log ("exit GameStateRound");
		m_owner = (GameMainScene)owner;
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, PlayerRollDiceNtf);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFromServer);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF, RollDiceOverNtfFromServer);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.LEAVE_GAME_NTF_CMD, PlayerLeaveNtf);
		//
		if (null != m_messageUICtr) {
			GameObject.Destroy (m_messageUICtr.gameObject);
		}
		if (null != m_roundUICtr) {
			GameObject.Destroy (m_roundUICtr.gameObject);
		}
		if (null != m_diceCtr) {
			GameObject.Destroy (m_diceCtr.gameObject);
		}
	}

	public void Message(string message, object[] parameters)
	{
		Debug.Log("GameStateMainScene::Message:"+message);
		if (message.Equals ("MovingOver")) {
			DoMovingOver (parameters);
		} else if (message.Equals ("RollDiceBtn")) {
			DoRollDice ();
		}
	}

	void DoMovingOver(object[] p)
	{
		m_roundUICtr.RollDiceBtn.interactable = false;
		m_isMovingOver = true;
		m_isSyncDice = false;
		SendServerRoundEnd ();
	}
	void DoRollDice()
	{
		if(m_isMovingOver){
			m_diceCtr.IsKinematic = false;
			m_diceCtr.Roll(15000);
			m_isMovingOver = false;
			m_isSyncDice = true;
		}
	}
	void SyncDicePosRotationFromServer(byte[] data)
	{
		dice_sync_ntf ntf = NetUtils.Deserialize<dice_sync_ntf> (data);
		m_diceCtr.IsKinematic = true;
		m_diceCtr.Dice.SetActive (ntf.is_active);
		Vector3 pos = new Vector3 (ntf.pos_x, ntf.pos_y, ntf.pos_z);
		Quaternion rotation = new Quaternion (ntf.rotation_x, ntf.rotation_y, ntf.rotation_z, ntf.rotation_w);
		m_diceCtr.Dice.transform.position = pos;
		m_diceCtr.Dice.transform.rotation = rotation;
	}
	void SyncDicePosRotationToServer()
	{
		dice_sync_ntf ntf = new dice_sync_ntf ();
		ntf.player_id = m_localPlayer.PlayerID;
		ntf.is_active = m_diceCtr.Dice.activeSelf;
		ntf.pos_x = m_diceCtr.Dice.transform.position.x;
		ntf.pos_y = m_diceCtr.Dice.transform.position.y;
		ntf.pos_z = m_diceCtr.Dice.transform.position.z;
		ntf.rotation_x = m_diceCtr.Dice.transform.rotation.x;
		ntf.rotation_y = m_diceCtr.Dice.transform.rotation.y;
		ntf.rotation_z = m_diceCtr.Dice.transform.rotation.z;
		ntf.rotation_w = m_diceCtr.Dice.transform.rotation.w;

		TcpClientHelper.Instance.SendData<dice_sync_ntf> (NET_CMD.DICE_SYNC_NTF_CMD, ntf);
	}

	void InitPlayers()
	{
		List<player_data> list = GameGlobalData.GetClientPlayerDataList ();
		foreach (player_data data in list) {
			GameObject prefab = Resources.Load ("Players/" + data.res_name)as GameObject;
			GameObject go = GameObject.Instantiate (prefab);
			Player ctr = go.GetComponent<Player> ();
			ctr.PlayerID = data.player_id;
			ctr.ResName = data.res_name;
			ctr.CurMapGrid = m_owner.GameMap.StartGrid;
			ctr.transform.position = ctr.CurMapGrid.PlayerPos(data.res_name);

			if (data.player_id == GameGlobalData.PlayerID) {
				m_localPlayer = ctr;
				m_owner.CameraCtr.FollowTarget = ctr.transform;
			}
			m_playerList.Add (ctr);
		}

	}
	void SendServerRoundEnd()
	{
		player_round_end_req req = new player_round_end_req ();
		req.player_id = GameGlobalData.PlayerID;
		TcpClientHelper.Instance.SendData<player_round_end_req> (NET_CMD.PLAYER_ROUND_END_REQ_CMD, req);
	}
	void RollCallback(uint num){
		Debug.Log("rollcallback:"+num);
		roll_dice_over_ntf ntf = new roll_dice_over_ntf ();
		ntf.player_id = m_localPlayer.PlayerID;
		ntf.dice_num = (int)num;
		TcpClientHelper.Instance.SendData<roll_dice_over_ntf> (NET_CMD.ROLL_DICE_OVER_NTF, ntf);
	}
	void RollDiceOverNtfFromServer(byte[] data)
	{
		roll_dice_over_ntf ntf = NetUtils.Deserialize<roll_dice_over_ntf> (data);
		if (ntf.player_id == m_localPlayer.PlayerID) {
			m_messageUICtr.ShowNotify ("你移动"+ntf.dice_num+"步");
		} else {
			m_messageUICtr.ShowNotify ("玩家" + GameGlobalData.GetClientPlayerData (ntf.player_id).res_name + "移动" + ntf.dice_num + "步");
		}
		Player player = GetPlayer (ntf.player_id);
		MapGrid target = player.CurMapGrid;
		for(int i=0; i<ntf.dice_num; i++)
		{
			target = target.NextGrid;
		}
		player.CurMapGrid = target;
		player.GotoMapGrid(target);
	}
	void PlayerRollDiceNtf(byte[] data)
	{
		Debug.Log ("client:PlayerRollDiceNtf");
		player_roll_dice_ntf ntf = NetUtils.Deserialize<player_roll_dice_ntf> (data);
		if (ntf.player_id == m_localPlayer.PlayerID) {
			m_messageUICtr.ShowNotify ("轮到你投掷骰子");	
			m_roundUICtr.RollDiceBtn.interactable = true;
		} else {
			m_messageUICtr.ShowNotify ("轮到玩家"+GameGlobalData.GetClientPlayerData(ntf.player_id).res_name+"投掷骰子");
			m_roundUICtr.RollDiceBtn.interactable = false;
		}
	}
	void PlayerLeaveNtf(byte[] data)
	{
		leave_game_ntf ntf = NetUtils.Deserialize<leave_game_ntf> (data);
		if (ntf.player_id == m_localPlayer.PlayerID) {
			m_messageUICtr.ShowMessage ("你已经被服务器强制退出游戏", DoBackToLogin);
		} else {
			m_messageUICtr.ShowNotify ("玩家" + GameGlobalData.GetClientPlayerData (ntf.player_id).res_name + "退出游戏");
			Player player = GetPlayer (ntf.player_id);
			m_playerList.Remove (player);
			GameGlobalData.RemoveClientPlayerData (ntf.player_id);
			GameObject.Destroy (player.gameObject);
		}
	}
	void DoBackToLogin()
	{
		TcpClientHelper.Instance.Close ();
		SceneLoading.LoadSceneName = GameGlobalData.LoginSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
		//GameStateManager.Instance ().FSM.ChangeState (GameStateClientConnect.Instance ());
	}
}
