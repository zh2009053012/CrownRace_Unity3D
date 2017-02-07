using UnityEngine;
using System.Collections;

public class GameStart : GameStateBase {

	// Use this for initialization
	void Start () {
		FSM = new StateMachine (this);
		GameStateManager.Instance ().FSM = FSM;

		FSM.CurrentState = GameStateLaunch.Instance ();
		FSM.CurrentState.Enter (FSM.Owner);
		FSM.GlobalState = GameGlobalState.Instance ();
		FSM.GlobalState.Enter (FSM.Owner);
	}
	
	// Update is called once per frame
	void Update () {
		if (null != FSM)
			FSM.Update ();
	}
	void OnDestroy()
	{
		if (FSM != null && FSM.GlobalState != null) {
			FSM.GlobalState.Exit (FSM.Owner);
			FSM.CurrentState.Exit (FSM.Owner);
		}
	}
}
