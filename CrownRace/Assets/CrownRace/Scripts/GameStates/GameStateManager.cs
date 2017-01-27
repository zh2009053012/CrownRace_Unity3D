using UnityEngine;
using System.Collections;

public class GameStateManager {

	private static GameStateManager m_instance;
	private static object m_lockHelper = new object();
	private GameStateManager(){}
	public static GameStateManager Instance()
	{
		if(null == m_instance)
		{
			lock(m_lockHelper)
			{
				if(null == m_instance)
				{
					m_instance = new GameStateManager();
				}
			}
		}
		return m_instance;
	}
	//store current FSM
	private StateMachine fsm;
	public StateMachine FSM 
	{
		get{return fsm;}
		set{fsm = value;}
	}
}
