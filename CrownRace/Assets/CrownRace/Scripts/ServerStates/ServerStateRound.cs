using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.crownrace.msg;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

public class ServerStateRound : IStateBase {

	private ServerStateRound()
	{}

	private static ServerStateRound m_instance;
	private static object m_lockHelper = new object();
	public static ServerStateRound Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerStateRound ();
				}
			}
		}
		return m_instance;
	}
	//
	private PlayerRoundData m_curRoundPlayer;
	private RollTheDice m_diceCtr;
	public void Enter(GameStateBase owner)
	{
		Debug.Log("enter ServerStateRound");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient, "SyncDicePosRotationFormClient");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromClient, "RollDiceOverNtfFromClient");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd, "PlayerRoundEnd");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.PLAYER_MOVE_OVER_NTF_CMD, PlayerMoveOver, "PlayerMoveOver");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.CELL_EFFECT_ACK_CMD, CellEffectAck, "CellEffectAck");
//		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, MoveToEndNtf, "MoveToEndNtf");
//		TcpListenerHelper.Instance.RegisterNetMsg(NET_CMD.ROLL_CARD_REQ_CMD, RollCardReq, "RollCardReq");
//		TcpListenerHelper.Instance.RegisterNetMsg(NET_CMD.USE_CARD_NTF_CMD, UseCardNtf, "UseCardNtf");
		//
		TcpListenerHelper.Instance.RegisterNetMsg(NET_CMD.END_ROUND_REQ_CMD, ClientEndRoundReq, "");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.ROLL_DICE_REQ_CMD, ClientRollDiceReq, "");
		TcpListenerHelper.Instance.RegisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq, "");

		GameObject dicePrefab = Resources.Load ("RollDice")as GameObject;
		GameObject diceGO = GameObject.Instantiate (dicePrefab);
		m_diceCtr = diceGO.GetComponent<RollTheDice> ();
		m_diceCtr.RegisterRollOverNotify (RollCallback);
		//
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
		for (int i = 0; i < list.Count; i++) {
			SetPlayerStartPos (list [i].player_id);
		}

		SelectCurRoundPlayer ();
//		GameGlobalData.ResetPlayerRoundEnd ();
//		NotifyNextRoundPlayer ();
	}
	void RollCallback(uint num){
		m_isDiceRolling = false;
		ServerSyncDiceNtf (false, m_diceCtr.Dice.transform.position, m_diceCtr.Dice.transform.rotation);

		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
		for(int i=0; i<list.Count; i++){
			string playerName = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
			ServerMessageNtf(list[i].player_id, playerName + "移动"+num+"格");
		}
		//
		object[] p = new object[2];
		p [0] = (object)m_curRoundPlayer.player_id;
		p [1] = (object)((int)num);
		CallFuncAfter (3, "MovePlayer", p);
	}
	void RollCard(object[] p)
	{
		Debug.Log("RollCard");
		add_player_card_ntf ntf = new add_player_card_ntf();

		int playerId = (int)p[0];
		int num = (int)p[1];
		int haveCardNum = (int)p[2];
		ntf.player_id = playerId;
		ntf.have_card_num = haveCardNum;
		for(int i=0; i<num; i++)
		{
			ntf.card_config_id.Add((int)p[3+i]);
		}
		for(int i=0; i<num; i++){
			ntf.card_instance_id.Add((int)p[3+num+i]);
		}
		ServerAddPlayerCardNtf(ntf);
	}
	void EndRoundMsg(object[] p){
		int playerId = (int)p[0];
		string targetPlayer = GetPlayerName(playerId, m_curRoundPlayer.player_id);
		ServerMessageNtf(m_curRoundPlayer.player_id, targetPlayer+"可以选择结束回合或者使用卡牌");
		ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
	}
	void MovePlayer(object[] p){
		int playerId = (int)p [0];
		int num = (int)p [1];
		Debug.Log ("MovePlayer:"+playerId+","+num);
		m_isMovingPlayer = true;
		m_movingPlayerId = playerId;

		PlayerRoundData player = GameGlobalData.GetServerPlayerData (playerId);

		MapGrid target = player.stay_grid;
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
		player.position = player.stay_grid.PlayerPos (player.res_name);
		player.stay_grid = target;
		//
		m_pathList.Clear();
		for(int i=0; i<list.Count; i++){
			NavMeshPath path = new NavMeshPath ();
			Vector3 srcPos = player.position;
			if(i != 0){
				srcPos = list[i-1].PlayerPos(player.res_name);
			}
			NavMesh.CalculatePath (srcPos, list[i].PlayerPos(player.res_name), NavMesh.AllAreas, path);
			Debug.Log("path:"+path.corners.Length);
			m_pathList.AddRange (path.corners);
		}
	}
	void SetPlayerStartPos(int playerId){
		PlayerRoundData data = GameGlobalData.GetServerPlayerData (playerId);
		data.stay_grid = Map.Instance.StartGrid;
		Vector3 pos = Map.Instance.StartGrid.PlayerPos (data.res_name);
		ServerMovePlayerNtf (playerId, pos, Quaternion.identity);
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
	void SelectCurRoundPlayer(){

		m_curRoundPlayer = GameGlobalData.GetServerNextPlayerData ();

		string targetPlayer = "";
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();

		if (m_curRoundPlayer.PauseNum > 0) {
			m_curRoundPlayer.MinusPauseNum ();
			//
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"本回合不可投掷骰子");
			}

			CallFuncAfter(1, "CustomCallback", null, ()=>{
				ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
				ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
				//
				ServerMessageNtf(m_curRoundPlayer.player_id, targetPlayer+"可以选择结束回合或者使用卡牌");
				ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
			});
		} else {
			
			for (int i = 0; i < list.Count; i++) {
				targetPlayer = GetPlayerName (list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"的回合");
				ServerSetUseCardStateNtf (list [i].player_id, list [i].player_id == m_curRoundPlayer.player_id);
				ServerSetDiceBtnStateNtf (list [i].player_id, list [i].player_id == m_curRoundPlayer.player_id);
				ServerSetEndRoundBtnNtf(list[i].player_id, false);
			}
//			player_roll_dice_ntf ntf = new player_roll_dice_ntf ();
//			ntf.player_id = m_curRoundPlayer.player_id;
//			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_roll_dice_ntf> (NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, ntf);
		}
	}
	//
	struct FuncData{
		public string funcName;
		public float second;
		public float startTime;
		public object[] parameters;
		public VoidEvent callback;
		public FuncData(float startTime, float second, string funcName, object[] p, VoidEvent callback=null){
			this.startTime = startTime;
			this.second = second;
			this.funcName = funcName;
			this.parameters = p;
			this.callback = callback;
		}
	}
	List<FuncData> m_funcList = new List<FuncData>();
	void CallFuncAfter(float second, string funcName, object[] parameters, VoidEvent callback=null){
		FuncData data = new FuncData (Time.time, second, funcName, parameters, callback);
		m_funcList.Add (data);
	}
	private bool m_isDiceRolling = false;
	private bool m_isMovingPlayer = false;
	private int m_movingPlayerId;
	private List<Vector3> m_pathList = new List<Vector3> ();
	private int m_pathIndex = 0;
	public void Execute(GameStateBase owner)
	{
		//check func
		int removeIndex = -1;
		for (int i = 0; i < m_funcList.Count; i++) {
			if (Time.time - m_funcList [i].startTime >= m_funcList [i].second) {
				DoCallFunc (m_funcList[i]);
				removeIndex = i;
				break;
			}
		}
		if (removeIndex >= 0) {
			m_funcList.RemoveAt (removeIndex);
		}
		//do sync dice
		if(m_isDiceRolling){
			ServerSyncDiceNtf (m_diceCtr.Dice.activeSelf, m_diceCtr.Dice.transform.position, m_diceCtr.Dice.transform.rotation);
		}
		//do sync player's pos
		if (m_isMovingPlayer) {
			PlayerRoundData player = GameGlobalData.GetServerPlayerData (m_movingPlayerId);
			float step = Time.deltaTime * 10;

			if (Vector3.Distance (m_pathList [m_pathIndex], player.position) <= step) {
				player.position = m_pathList [m_pathIndex];
				m_pathIndex++;
				//goto the end
				if (m_pathIndex >= m_pathList.Count) {
					m_isMovingPlayer = false;
					m_pathIndex = 0;
					GetPlayerGirdType();
				}
			} else {
				Vector3 dir = m_pathList [m_pathIndex] - player.position;
				player.position = player.position + dir.normalized * Time.deltaTime * 10;
			}
			ServerMovePlayerNtf (m_movingPlayerId, player.position, Quaternion.identity);

		}
	}

	void DoCallFunc(FuncData data){
		switch (data.funcName) {
		case "SelectCurRoundPlayer":
			SelectCurRoundPlayer ();
			break;
		case "MovePlayer":
			MovePlayer (data.parameters);
			break;
		case "CustomCallback":
			if(null != data.callback){
				data.callback.Invoke();
			}
			break;
		case "RollCard":
			RollCard(data.parameters);
			break;
		}
	}

	void GetPlayerGirdType(){
		PlayerRoundData data = GameGlobalData.GetServerPlayerData(m_curRoundPlayer.player_id);
		string targetPlayer = GetPlayerName (data.player_id, m_curRoundPlayer.player_id);
		List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();

		switch (data.stay_grid.CellEffect) {
		case CELL_EFFECT.NONE:
			ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
			ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
			ServerMessageNtf(m_curRoundPlayer.player_id, targetPlayer+"可以选择结束回合或者使用卡牌");
			ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
			break;
		case CELL_EFFECT.BACK:
			
			for(int i=0; i < list.Count; i++){
				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"向后移动"+data.stay_grid.EffectKeepRound+"格");
			}
			//
			object[] pb = new object[2];
			pb [0] = (object)m_curRoundPlayer.player_id;
			pb [1] = (object)((int)data.stay_grid.EffectKeepRound*-1);
			CallFuncAfter (3, "MovePlayer", pb);
			break;
		case CELL_EFFECT.FORWARD:
			
			for(int i=0; i < list.Count; i++){
				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"向前移动"+data.stay_grid.EffectKeepRound+"格");
			}
			//
			object[] pf = new object[2];
			pf [0] = (object)m_curRoundPlayer.player_id;
			pf [1] = (object)((int)data.stay_grid.EffectKeepRound);
			CallFuncAfter (3, "MovePlayer", pf);
			break;
		case CELL_EFFECT.PAUSE:
			data.PauseNum += (int)data.stay_grid.EffectKeepRound;
			for(int i=0; i < list.Count; i++){
				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"暂停"+data.stay_grid.EffectKeepRound+"回合");
			}

			CallFuncAfter(2, "CustomCallback", null, ()=>{
				ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
				ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
				//
				ServerMessageNtf(m_curRoundPlayer.player_id, targetPlayer+"可以选择结束回合或者使用卡牌");
				ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
			});

			break;
		case CELL_EFFECT.ROLL_CARD:
			int cardNum = (int)data.stay_grid.EffectKeepRound;
			for(int i=0; i < list.Count; i++){
				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"抽取"+cardNum+"张卡牌");
			}

			object[] pr = new object[3+cardNum*2];
			pr[0] = (object)m_curRoundPlayer.player_id;
			pr[1] = (object)cardNum;

			for(int i=0; i<cardNum; i++){
				int config_id = Random.Range(0, GameGlobalData.CardList.Length);
				pr[3+i] = (object)(config_id);
				CardEffect config = GameGlobalData.CardList[config_id];
				//
				int instanceId = CardEffect.ID;
				pr[3+cardNum+i] = (object)instanceId;
				CardEffect instance = new CardEffect(instanceId, config_id, config.effect, config.effect_value, 
					config.effect2, config.effect_value2, config.select_target, config.select_value, config.name, config.desc);
				m_curRoundPlayer.card_list.Add(instance);
			}
			pr[2] = (object)m_curRoundPlayer.card_list.Count;

			CallFuncAfter(2, "RollCard", pr);
			CallFuncAfter(8, "CustomCallback", null, ()=>{
				ServerSetUseCardStateNtf(m_curRoundPlayer.player_id, true);
				ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, false);
				ServerMessageNtf(m_curRoundPlayer.player_id, targetPlayer+"可以选择结束回合或者使用卡牌");
				ServerSetEndRoundBtnNtf(m_curRoundPlayer.player_id, true);
			});

			break;
		case CELL_EFFECT.ROLL_DICE:
			for(int i=0; i < list.Count; i++){
				targetPlayer = GetPlayerName(list[i].player_id, m_curRoundPlayer.player_id);
				ServerMessageNtf (list[i].player_id, targetPlayer+"额外投掷一次骰子");
			}
			ServerSetDiceBtnStateNtf(m_curRoundPlayer.player_id, true);
			break;
		}
	}

	public void Exit(GameStateBase owner)
	{
		Debug.Log("exit ServerStateRound");
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.DICE_SYNC_NTF_CMD, SyncDicePosRotationFormClient);
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_OVER_NTF_CMD, RollDiceOverNtfFromClient);
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_ROUND_END_REQ_CMD, PlayerRoundEnd);
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.PLAYER_MOVE_OVER_NTF_CMD, PlayerMoveOver);
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.CELL_EFFECT_ACK_CMD, CellEffectAck);
//		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.MOVE_TO_END_NTF_CMD, MoveToEndNtf);
//		TcpListenerHelper.Instance.UnregisterNetMsg(NET_CMD.ROLL_CARD_NTF_CMD, RollCardReq);
//		TcpListenerHelper.Instance.UnregisterNetMsg(NET_CMD.USE_CARD_NTF_CMD, UseCardNtf);
		//
		TcpListenerHelper.Instance.UnregisterNetMsg(NET_CMD.END_ROUND_REQ_CMD, ClientEndRoundReq);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.ROLL_DICE_REQ_CMD, ClientRollDiceReq);
		TcpListenerHelper.Instance.UnregisterNetMsg (NET_CMD.USE_CARD_REQ_CMD, ClientUseCardReq);


		GameGlobalData.ResetPlayerRoundEnd ();
	}

	public void Message(string message, object[] parameters)
	{
		if (message.Equals ("ClientDisconnect")) {
			DoClientDisconnect (parameters);	
		}
	}
	#region client req & server ntf/ack
	void ServerSetUseCardStateNtf(int playerId, bool canUseCard){
		set_use_card_state_ntf ntf = new set_use_card_state_ntf ();
		ntf.can_use_card = canUseCard;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_use_card_state_ntf> (playerId, NET_CMD.SET_USE_CARD_STATE_NTF_CMD, ntf);
	}
	void ServerMessageNtf(string msg){
		message_ntf ntf = new message_ntf ();
		ntf.msg = msg;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<message_ntf> (NET_CMD.MESSAGE_NTF_CMD, ntf);
	}
	void ServerMessageNtf(int playerId, string msg){
		message_ntf ntf = new message_ntf ();
		ntf.msg = msg;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<message_ntf> (playerId, NET_CMD.MESSAGE_NTF_CMD, ntf);
	}
	void ServerSetDiceBtnStateNtf(int playerId, bool canPressDice){
		set_dice_btn_state_ntf ntf = new set_dice_btn_state_ntf ();
		ntf.player_id = playerId;
		ntf.can_press = canPressDice;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_dice_btn_state_ntf> (playerId, NET_CMD.SET_DICE_BTN_STATE_NTF_CMD, ntf);
	}
	void ServerRemovePlayerCardNtf(int playerId, int[] cardInstanceId){
		remove_player_card_ntf ntf = new remove_player_card_ntf ();
		ntf.player_id = playerId;
		ntf.card_instance_id.AddRange(cardInstanceId);
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<remove_player_card_ntf> (NET_CMD.REMOVE_PLAYER_CARD_NTF_CMD, ntf);
	}
	void ServerAddPlayerCardNtf(add_player_card_ntf ntf){

		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<add_player_card_ntf> (NET_CMD.ADD_PLAYER_CARD_NTF_CMD, ntf);
	}
	void ServerMovePlayerNtf(int playerId, Vector3 position, Quaternion rotation){
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
	void ServerSyncDiceNtf(bool isActive, Vector3 position, Quaternion rotation){
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
	void ServerSetPlayerStateNtf(int playerId, PLAYER_STATE state, int stateRoundLeft){
		set_player_state_ntf ntf = new set_player_state_ntf ();
		ntf.player_id = playerId;
		ntf.player_state = state;
		ntf.state_round_left = stateRoundLeft;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<set_player_state_ntf> (NET_CMD.SET_PLAYER_STATE_NTF_CMD, ntf);
	}
	void ServerSetEndRoundBtnNtf(int playerId, bool canEndRound){
		set_end_round_btn_state_ntf ntf = new set_end_round_btn_state_ntf();
		ntf.can_end_round = canEndRound;
		TcpListenerHelper.Instance.clientsContainer.SendToClient<set_end_round_btn_state_ntf>(playerId, NET_CMD.SET_END_ROUND_BTN_STATE_NTF_CMD, ntf);
	}
	void ClientUseCardReq(int playerId, byte[] data){
		use_card_req req = NetUtils.Deserialize<use_card_req> (data);
	}
	void ClientRollDiceReq(int playerId,byte[] data){
		roll_dice_req req = NetUtils.Deserialize<roll_dice_req> (data);
		ServerSetUseCardStateNtf (playerId, false);
		m_diceCtr.IsKinematic = false;
		m_diceCtr.Roll (15000);

		m_isDiceRolling = true;

	}
	//
	void ClientEndRoundReq(int playerId,byte[] data){
		end_round_req req = NetUtils.Deserialize<end_round_req> (data);
		if(m_curRoundPlayer.player_id == req.player_id){

			List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData();
			for(int i=0; i<list.Count; i++){
				string playerName = GetPlayerName(list[i].player_id, req.player_id);
				ServerMessageNtf(list[i].player_id, playerName+"结束回合");
			}

			SelectCurRoundPlayer();
		}
	}

	#endregion

	void DoClientDisconnect(object[] p){
		
		int player_id = (int)p [0];
		Debug.Log ("Server DoClientDisconnect"+player_id);

		if (m_curRoundPlayer.player_id == player_id) {
			GameGlobalData.RemoveServerPlayerData (player_id);
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
		} else {
//			PlayerRoundData data = GameGlobalData.GetServerPlayerData (player_id);
//			List<PlayerRoundData> list = GameGlobalData.GetServerAllPlayerData ();
//			list.Remove (data);
//			bool isOthersRoundOver = true;
//			bool isOthersMoveOver = true;
//			foreach(PlayerRoundData player in list)
//			{
//				if (!player.is_move_over)
//					isOthersMoveOver = false;
//				if (!player.is_round_over)
//					isOthersRoundOver = false;
//			}
//			if (isOthersRoundOver && !data.is_round_over) {
//				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
//			} else if (isOthersMoveOver && !data.is_move_over) {
//				cell_effect_req req = new cell_effect_req ();
//				TcpListenerHelper.Instance.clientsContainer.SendToClient<cell_effect_req> (m_curRoundPlayer.player_id, NET_CMD.CELL_EFFECT_REQ_CMD, req);
//			}
			GameGlobalData.RemoveServerPlayerData (player_id);

			if (GameGlobalData.IsAllPlayerRoundEnd ()) {
				TcpListenerHelper.Instance.FSM.ChangeState (ServerStateRound.Instance ());
			}
			else if (GameGlobalData.IsAllPlayerMoveOver ()) {
				cell_effect_req req = new cell_effect_req ();
				TcpListenerHelper.Instance.clientsContainer.SendToClient<cell_effect_req> (m_curRoundPlayer.player_id, NET_CMD.CELL_EFFECT_REQ_CMD, req);
			}
		}
	}
	void NotifyNextRoundPlayer()
	{
		Debug.Log ("NotifyNextRoundPlayer");
		m_curRoundPlayer = GameGlobalData.GetServerNextPlayerData ();
		if (m_curRoundPlayer.PauseNum > 0) {
			m_curRoundPlayer.MinusPauseNum ();
			player_pause_ntf ntf = new player_pause_ntf ();
			ntf.player_id = m_curRoundPlayer.player_id;
			ntf.left_pause_round = m_curRoundPlayer.PauseNum;
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_pause_ntf> (NET_CMD.PLAYER_PAUSE_NTF_CMD, ntf);
		} else {
			player_roll_dice_ntf ntf = new player_roll_dice_ntf ();
			ntf.player_id = m_curRoundPlayer.player_id;
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<player_roll_dice_ntf> (NET_CMD.PLAYER_ROLL_DICE_NTF_CMD, ntf);
		}
	}
	void SyncDicePosRotationFormClient(int player_id, byte[] data)
	{
		dice_sync_ntf ntf = NetUtils.Deserialize<dice_sync_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SendToAllClientExcept<dice_sync_ntf> (player_id, NET_CMD.DICE_SYNC_NTF_CMD, ntf);
		}
	}
	void RollDiceOverNtfFromClient(int player_id, byte[] data)
	{
		Debug.Log ("RollDiceOverNtfFromClient:"+player_id);
		roll_dice_over_ntf ntf = NetUtils.Deserialize<roll_dice_over_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<roll_dice_over_ntf> (NET_CMD.ROLL_DICE_OVER_NTF_CMD, ntf);
		}
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
	void PlayerMoveOver(int player_id, byte[] data)
	{
		player_move_over_ntf ntf = NetUtils.Deserialize<player_move_over_ntf> (data);
		if (player_id == ntf.player_id) {
			Debug.Log ("PlayerMoveOver:"+player_id);
			GameGlobalData.SetPlayerMoveMove (player_id, true);
			if (GameGlobalData.IsAllPlayerMoveOver ()) {
				cell_effect_req req = new cell_effect_req ();
				TcpListenerHelper.Instance.clientsContainer.SendToClient<cell_effect_req> (ntf.move_player_id, NET_CMD.CELL_EFFECT_REQ_CMD, req);
			}
		}
	}
	void CellEffectAck(int player_id, byte[] data){
		GameGlobalData.ResetPlayerMoveOver ();
		cell_effect_ack ack = NetUtils.Deserialize<cell_effect_ack> (data);
		Debug.Log ("CellEffectAck:"+ack.player_id+","+ack.cell_effect);
		if (ack.player_id != player_id)
			return;
		switch (ack.cell_effect) {
		case CELL_EFFECT.NONE:
			break;
		case CELL_EFFECT.BACK:
			break;
		case CELL_EFFECT.FORWARD:
			break;
		case CELL_EFFECT.PAUSE:
			PlayerRoundData movePlayerData = GameGlobalData.GetServerPlayerData(ack.player_id);
			movePlayerData.PauseNum += ack.effect_num;
			break;
		case CELL_EFFECT.ROLL_CARD:
			break;
		case CELL_EFFECT.ROLL_DICE:
			if(ack.player_id != m_curRoundPlayer.player_id)
				ack.cell_effect = CELL_EFFECT.NONE;
			break;
		}
		cell_effect_ntf ntf = new cell_effect_ntf ();
		ntf.player_id = player_id;
		ntf.cell_effect = ack.cell_effect;
		ntf.effect_num = ack.effect_num;
		TcpListenerHelper.Instance.clientsContainer.SendToAllClient<cell_effect_ntf> (NET_CMD.CELL_EFFECT_NTF_CMD, ntf);
	}
	void MoveToEndNtf(int player_id, byte[] data)
	{
		move_to_end_ntf ntf = NetUtils.Deserialize<move_to_end_ntf> (data);
		if (player_id == ntf.player_id) {
			TcpListenerHelper.Instance.FSM.ChangeState (ServerStateEndGame.Instance());
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<move_to_end_ntf> (NET_CMD.MOVE_TO_END_NTF_CMD, ntf);
		}
	}
	void RollCardReq(int player_id, byte[] data){
		roll_card_req req = NetUtils.Deserialize<roll_card_req>(data);
		Debug.Log("RollCardReq:"+player_id+","+req.player_id);
		if(player_id == req.player_id){
			roll_card_ntf ntf = new roll_card_ntf();
			ntf.player_id = player_id;
			ntf.card_config_id = Random.Range(0, GameGlobalData.CardList.Length);
			ntf.card_instance_id = CardEffect.ID;
			//
			PlayerRoundData playerData = GameGlobalData.GetServerPlayerData(player_id);
			CardEffect config = GameGlobalData.CardList[ntf.card_config_id];
			CardEffect instance = new CardEffect(ntf.card_instance_id, ntf.card_config_id, config.effect, config.effect_value, 
				config.effect2, config.effect_value2, config.select_target, config.select_value, config.name, config.desc);
			playerData.card_list.Add(instance);

			ntf.have_card_num = playerData.card_list.Count;
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<roll_card_ntf>(NET_CMD.ROLL_CARD_NTF_CMD, ntf);
		}
	}
	void UseCardNtf(int player_id, byte[] data){
		Debug.Log("ServerStateRound::UseCardNtf:"+player_id);
		use_card_ntf ntf = NetUtils.Deserialize<use_card_ntf>(data);
		if(player_id == ntf.use_player_id){
			PlayerRoundData usePlayerData = GameGlobalData.GetServerPlayerData(ntf.use_player_id);
			PlayerRoundData targetPlayerData = GameGlobalData.GetServerPlayerData(ntf.target_player_id1);
			CardEffect ce = usePlayerData.GetCardEffect(ntf.card_instance_id);
			usePlayerData.RemoveCardEffect(ntf.card_instance_id);
			ntf.have_card_num = usePlayerData.card_list.Count;
			//
			switch(ce.effect){
			case CARD_EFFECT.FORWARD:
				break;
			case CARD_EFFECT.BACK:
				break;
//			case CARD_EFFECT.DOUBLE_DICE_NUM:
//				break;
//			case CARD_EFFECT.GOD_TIME:
//				break;
			case CARD_EFFECT.PAUSE:
				targetPlayerData.PauseNum += ce.effect_value;
				break;
			}
			//
			TcpListenerHelper.Instance.clientsContainer.SendToAllClient<use_card_ntf>( NET_CMD.USE_CARD_NTF_CMD, ntf);
		}
	}
}
