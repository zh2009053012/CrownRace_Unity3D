using UnityEngine;
using System.Collections;

public class StateMachine {

	private GameStateBase m_owner;
	private IStateBase m_currentState;
	private IStateBase m_previousState;
	private IStateBase m_globalState;

	public StateMachine(GameStateBase owner)
	{
		m_owner = owner;
		m_currentState = null;
		m_previousState = null;
		m_globalState = null;
	}

	public GameStateBase Owner
	{
		get{ return m_owner;}
	}

	public IStateBase CurrentState
	{
		get{ return m_currentState;}
		set{ m_currentState = value;}
	}
	public IStateBase PreviousState
	{
		get{ return m_previousState;}
		set{ m_previousState = value;}
	}
	public IStateBase GlobalState
	{
		get{ return m_globalState;}
		set{ m_globalState = value;}
	}
	public StateMachine Clone()
	{
		StateMachine FSM = new StateMachine (Owner);
		FSM.CurrentState = CurrentState;
		FSM.PreviousState = PreviousState;
		FSM.GlobalState = GlobalState;

		return FSM;
	}

	public void Update()
	{
		if (GlobalState != null) {
			GlobalState.Execute (Owner);
		}
		if (CurrentState != null) {
			CurrentState.Execute (Owner);
		}
	}

	public void ChangeState(IStateBase newState)
	{
		if (null == newState) {
			return;
		}
		PreviousState = CurrentState;
		if(null != CurrentState)
			CurrentState.Exit (Owner);
		CurrentState = newState;
		CurrentState.Enter (Owner);
	}

	public void ReverToPreviousState()
	{
		ChangeState (PreviousState);
	}
	public bool IsInState(IStateBase t)
	{
		if (t == CurrentState)
			return true;
		else
			return false;
	}
}
