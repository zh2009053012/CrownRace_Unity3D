using UnityEngine;
using System.Collections;

public class GameStateNull : IStateBase {

	private GameStateNull()
	{}

	private static GameStateNull m_instance;
	private static object m_lockHelper = new object();
	public static GameStateNull Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateNull ();
				}
			}
		}
		return m_instance;
	}
	//
	public void Enter(GameStateBase owner)
	{

	}

	public void Execute(GameStateBase owner)
	{

	}

	public void Exit(GameStateBase owner)
	{

	}

	public void Message(string message, object[] parameters)
	{

	}
}
