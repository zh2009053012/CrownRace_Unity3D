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

	private List<PlayerHeadUI> m_headUIList = new List<PlayerHeadUI>();
	private PlayerHeadUI GetHeadUI(int player_id){
		foreach(PlayerHeadUI ui in m_headUIList){
			if(ui.ID == player_id){
				return ui;
			}

		}
		return null;
	}

	public void Enter(GameStateBase owner)
	{
		Debug.Log ("enter GameStateRound");
		m_owner = (GameMainScene)owner;
		m_playerList.Clear ();
		//
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, PlayerRollDiceNtf, "PlayerRollDiceNtf");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFromServer, "SyncDicePosRotationFromServer");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromServer, "RollDiceOverNtfFromServer");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.LEAVE_GAME_NTF_CMD, PlayerLeaveNtf, "PlayerLeaveNtf");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.CELL_EFFECT_REQ_CMD, CellEffectReq, "CellEffectReq");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.CELL_EFFECT_NTF_CMD, CellEffectNtf, "CellEffectNtf");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, ReceiveMoveToEndFromServer, "ReceiveMoveToEndFromServer");
		TcpClientHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_PAUSE_NTF_CMD, PlayerPauseNtf, "PlayerPauseNtf");
		TcpClientHelper.Instance.RegisterNetMsg(NET_CMD.ROLL_CARD_NTF_CMD, RollCardNtf, "RollCardNtf");
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
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromServer);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.LEAVE_GAME_NTF_CMD, PlayerLeaveNtf);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.CELL_EFFECT_REQ_CMD, CellEffectReq);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.CELL_EFFECT_NTF_CMD, CellEffectNtf);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, ReceiveMoveToEndFromServer);
		TcpClientHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_PAUSE_NTF_CMD, PlayerPauseNtf);
		TcpClientHelper.Instance.UnregisterNetMsg(NET_CMD.ROLL_CARD_NTF_CMD, RollCardNtf);
		//
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
	#region message func
	public void Message(string message, object[] parameters)
	{
		Debug.Log("GameStateMainScene::Message:"+message);
		if (message.Equals ("MovingOver")) {
			DoMovingOver (parameters);
		} else if (message.Equals ("RollDiceBtn")) {
			DoRollDice ();
		}else if(message.Equals("HeadBarClick")){
			DoHeadBarClick(parameters);
		}
	}
	void DoHeadBarClick(object[] p){
		int player_id = (int)p[0];
		Debug.Log("DoHeadBarCLick:"+player_id);
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
	void DoMovingOver(object[] p)
	{
		m_roundUICtr.RollDiceBtn.interactable = false;
		m_isMovingOver = true;
		m_isSyncDice = false;
		SendMoveOver ();
	}
	#endregion

	#region server to client. client to server
	string PlayerName(int player_id){
		string msg = "";
		if (player_id == m_localPlayer.PlayerID) {
			msg = "你";
		} else {
			msg = "玩家"+GameGlobalData.GetClientPlayerData(player_id).res_name;
		}
		return msg;
	}
	void PlayerPauseNtf(byte[] data)
	{
		player_pause_ntf ntf = NetUtils.Deserialize<player_pause_ntf> (data);
		m_messageUICtr.ShowNotify (PlayerName(ntf.player_id) + "本回合无法投掷骰子", (x) => {
			SendServerRoundEnd();
		}, null, 1);
	}
	void SendMoveOver()
	{
		player_move_over_ntf ntf = new player_move_over_ntf ();
		ntf.player_id = m_localPlayer.PlayerID;
		TcpClientHelper.Instance.SendData<player_move_over_ntf> (NET_CMD.PLAYER_MOVE_OVER_NTF_CMD, ntf);
	}
	void CellEffectReq(byte[] data){
		cell_effect_ack ack = new cell_effect_ack ();
		ack.player_id = m_localPlayer.PlayerID;
		ack.cell_effect = m_localPlayer.CurMapGrid.CellEffect;
		ack.effect_num = (int)m_localPlayer.CurMapGrid.EffectKeepRound;
		TcpClientHelper.Instance.SendData<cell_effect_ack> (NET_CMD.CELL_EFFECT_ACK_CMD, ack);
	}
	void CellEffectNtf(byte[] data){
		cell_effect_ntf ntf = NetUtils.Deserialize<cell_effect_ntf> (data);
		string msg = PlayerName (ntf.player_id);
		Debug.Log ("CellEffectNtf:"+ntf.player_id);
		object[] p = new object[2];
		p [0] = (object)ntf.player_id;
		p [1] = (object)ntf.effect_num;
		switch (ntf.cell_effect) {
		case CELL_EFFECT.NONE:
			m_messageUICtr.ShowNotify (msg + "回合结束", (x)=>{SendServerRoundEnd();}, null,2);
			//
			PlayerHeadUI uiCtr = GetHeadUI(ntf.player_id);
			uiCtr.ShowDiceImage(false);
			break;
		case CELL_EFFECT.BACK:
			m_messageUICtr.ShowNotify (msg + "后退"+ntf.effect_num+"步", (x)=>{
				MovePlayerCellNum ((int)x [0], -(int)x [1]);
			}, p, 1);
			break;
		case CELL_EFFECT.FORWARD:
			m_messageUICtr.ShowNotify (msg + "前进"+ntf.effect_num+"步", (x)=>{
				MovePlayerCellNum ((int)x [0], (int)x [1]);
			}, p, 1);
			break;
		case CELL_EFFECT.PAUSE:
			m_messageUICtr.ShowNotify (msg + "暂停"+ntf.effect_num+"回合", (x)=>{SendServerRoundEnd();}, null, 1);
			break;
		case CELL_EFFECT.ROLL_CARD:
			m_messageUICtr.ShowNotify (msg + "获得抽卡机会", (x)=>{
				if(ntf.player_id == m_localPlayer.PlayerID)
					RollTheCard();
				}, null, 1);
			break;
		case CELL_EFFECT.ROLL_DICE:
			m_messageUICtr.ShowNotify (msg + "获得额外投掷骰子的机会",  (x)=>{
				if(ntf.player_id == m_localPlayer.PlayerID)
					m_roundUICtr.RollDiceBtn.interactable = true;
			}, null, 1);
			break;
		case CELL_EFFECT.END:
			SendMoveToEndToServer ();
			break;
		}
	}
	void RollTheCard(){
		roll_card_req req = new roll_card_req();
		req.player_id = GameGlobalData.PlayerID;
		TcpClientHelper.Instance.SendData<roll_card_req>(NET_CMD.ROLL_CARD_REQ_CMD, req);
	}
	void RollCardNtf(byte[] data){

		roll_card_ntf ntf = NetUtils.Deserialize<roll_card_ntf>(data);
		Debug.Log("RollCardNtf:"+ntf.player_id+","+GameGlobalData.PlayerID);
		if(ntf.player_id == GameGlobalData.PlayerID){
			m_owner.TransEffect.Play();
			m_owner.CardCtr.AddCard(ntf.card_id, ()=>{
				SendServerRoundEnd();
			});
		}else{
			PlayerHeadUI ui = GetHeadUI(ntf.player_id);
			Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, ui.transform.position);
			Debug.Log("RollCardNtf:screen pos:"+screenPos);
			m_owner.TransEffect.Play();
			m_owner.CardCtr.AddCardTo(ntf.card_id, new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane), null);
		}
		//
		PlayerHeadUI uiCtr = GetHeadUI(ntf.player_id);
		uiCtr.SetCardNum(ntf.have_card_num);
	}

	void SendMoveToEndToServer()
	{
		move_to_end_ntf ntf = new move_to_end_ntf ();
		ntf.player_id = m_localPlayer.PlayerID;
		TcpClientHelper.Instance.SendData<move_to_end_ntf> (NET_CMD.MOVE_TO_END_NTF_CMD, ntf);
	}
	void ReceiveMoveToEndFromServer(byte[] data)
	{
		move_to_end_ntf ntf = NetUtils.Deserialize<move_to_end_ntf> (data);
		string msg = PlayerName (ntf.player_id);

		m_messageUICtr.ShowMessage (msg+"获得了胜利", (x) => {
			SendServerRoundEnd ();
		}, null, 5);
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

	void SendServerRoundEnd()
	{
		Debug.Log ("SendServerRoundEnd"+ GameGlobalData.PlayerID+","+m_localPlayer.PlayerID);
		player_round_end_req req = new player_round_end_req ();
		req.player_id = GameGlobalData.PlayerID;
		TcpClientHelper.Instance.SendData<player_round_end_req> (NET_CMD.PLAYER_ROUND_END_REQ_CMD, req);

	}
	void RollCallback(uint num){
		Debug.Log("rollcallback:"+num);
		roll_dice_over_ntf ntf = new roll_dice_over_ntf ();
		ntf.player_id = m_localPlayer.PlayerID;
		ntf.dice_num = (int)num;
		TcpClientHelper.Instance.SendData<roll_dice_over_ntf> (NET_CMD.ROLL_DICE_OVER_NTF_CMD, ntf);
	}
	void RollDiceOverNtfFromServer(byte[] data)
	{
		roll_dice_over_ntf ntf = NetUtils.Deserialize<roll_dice_over_ntf> (data);
		Debug.Log ("RollDiceOverNtfFromServer:"+ntf.player_id);
		string msg = PlayerName (ntf.player_id);

		m_messageUICtr.ShowNotify (msg+"移动"+ntf.dice_num+"步", (x)=>{
			MovePlayerCellNum (ntf.player_id, ntf.dice_num);
		}, null, 1);
	}

	void PlayerRollDiceNtf(byte[] data)
	{
		Debug.Log ("client:PlayerRollDiceNtf");
		player_roll_dice_ntf ntf = NetUtils.Deserialize<player_roll_dice_ntf> (data);
		string msg = PlayerName (ntf.player_id);
	
		m_messageUICtr.ShowNotify ("轮到"+msg+"投掷骰子", (x)=>{
			if(ntf.player_id == m_localPlayer.PlayerID)
				m_roundUICtr.RollDiceBtn.interactable = true;
			//
			PlayerHeadUI uiCtr = GetHeadUI(ntf.player_id);
			uiCtr.ShowDiceImage(true);
		}, null, 2);	
	}
	void PlayerLeaveNtf(byte[] data)
	{
		leave_game_ntf ntf = NetUtils.Deserialize<leave_game_ntf> (data);
		if (ntf.player_id == m_localPlayer.PlayerID) {
			m_messageUICtr.ShowMessage ("你已经被服务器强制退出游戏", DoBackToLogin, null);
		} else {
			m_messageUICtr.ShowNotify ("玩家" + GameGlobalData.GetClientPlayerData (ntf.player_id).res_name + "退出游戏");
			Player player = GetPlayer (ntf.player_id);
			m_playerList.Remove (player);
			GameGlobalData.RemoveClientPlayerData (ntf.player_id);
			GameObject.Destroy (player.gameObject);
			//
			PlayerHeadUI uiCtr = GetHeadUI(ntf.player_id);
			m_headUIList.Remove(uiCtr);
			GameObject.Destroy(uiCtr.gameObject);
		}
	}
	#endregion

	void InitPlayers()
	{
		List<PlayerRoundData> list = GameGlobalData.GetClientPlayerDataList ();
		GameObject headPrefab = Resources.Load("UI/PlayerHeadUI")as GameObject;
		foreach (PlayerRoundData data in list) {
			GameObject prefab = Resources.Load ("Players/" + data.res_name)as GameObject;
			GameObject go = GameObject.Instantiate (prefab, m_owner.GameMap.StartGrid.PlayerPos(data.res_name), new Quaternion())as GameObject;
			Player ctr = go.GetComponent<Player> ();
			ctr.PlayerID = data.player_id;
			ctr.ResName = data.res_name;
			ctr.CurMapGrid = m_owner.GameMap.StartGrid;

			if (data.player_id == GameGlobalData.PlayerID) {
				m_localPlayer = ctr;
				m_owner.CameraCtr.FollowTarget = ctr.transform;
			}
			m_playerList.Add (ctr);
			//head
			GameObject head = GameObject.Instantiate(headPrefab);
			head.transform.parent = m_roundUICtr.HeadContent;
			head.transform.localScale = Vector3.one;
			PlayerHeadUI headCtr = head.GetComponent<PlayerHeadUI>();
			headCtr.SetInfo(data.player_id, data.res_name, 0);
			headCtr.ShowDiceImage(false);
			m_headUIList.Add(headCtr);
		}

	}

	void DoBackToLogin(object[] p)
	{
		TcpClientHelper.Instance.Close ();
		GameGlobalData.ClearClientPlayerData ();
		SceneLoading.LoadSceneName = GameGlobalData.LoginSceneName;
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (GameGlobalData.LoadSceneName);
		//GameStateManager.Instance ().FSM.ChangeState (GameStateClientConnect.Instance ());
	}
	void MovePlayerCellNum(int player_id, int num)
	{
		if (num == 0)
			return;
		Player player = GetPlayer (player_id);
		MapGrid target = player.CurMapGrid;
		List<MapGrid> list = new List<MapGrid> ();
		int left = 0;
		if (num > 0) {
			for(int i=0; i<num; i++)
			{
				if (target.CellEffect == CELL_EFFECT.END) {
					left = num - i;
					break;
				}
				if (null == target.NextGrid) {
					break;
				}
				target = target.NextGrid;
			}
		} else {
			for(int i=0; i<Mathf.Abs(num); i++)
			{
				if (null == target.PreGird) {
					break ;
				}
				target = target.PreGird;
			}
		}
		list.Add (target);
		//
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
		player.CurMapGrid = target;
		player.GotoMapGrid(list);
	}
}
