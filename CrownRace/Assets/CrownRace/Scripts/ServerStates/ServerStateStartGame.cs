using UnityEngine;
using System.Collections;

public class ServerStateStartGame : IStateBase {

	private static ServerStateStartGame m_instance;
	private static object m_lockHelper = new object();
	public static ServerStateStartGame Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new ServerStateStartGame ();
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
