﻿using UnityEngine;
using System.Collections;

public class GameMainScene : GameStateBase {
	[SerializeField]
	private RollTheDice m_rollDiceCtr;
	public RollTheDice RollDiceCtr{
		get{return m_rollDiceCtr;}
	}
	[SerializeField]
	private Map m_map;
	public Map GameMap{
		get{return m_map;}
	}
	[SerializeField]
	private CameraFollow m_cameraCtr;
	public CameraFollow CameraCtr{
		get{return m_cameraCtr;}
	}
	// Use this for initialization
	void Start () {
		FSM = new StateMachine (this);
		GameStateManager.Instance ().FSM = FSM;
		FSM.GlobalState = GameStateMainSceneGlobal.Instance ();
		FSM.GlobalState.Enter (this);
		FSM.CurrentState = GameStateNull.Instance ();
		FSM.CurrentState.Enter (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (FSM != null) {
			FSM.Update ();
		}
	}

	void OnDestory()
	{
		if (FSM != null) {
			FSM.GlobalState.Exit (this);
			FSM.CurrentState.Exit (this);
		}
	}
}