using UnityEngine;
using System.Collections;

public class GameMainScene : GameStateBase {
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
		FSM.GlobalState = GameGlobalState.Instance ();
		FSM.GlobalState.Enter (this);
		FSM.CurrentState = GameStateRound.Instance ();
		FSM.CurrentState.Enter (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (FSM != null) {
			FSM.Update ();
		}
	}

	void OnDestroy()
	{
		if (FSM != null) {
			FSM.GlobalState.Exit (this);
			FSM.CurrentState.Exit (this);
		}
	}
}
