using UnityEngine;
using System.Collections;

public class GameStateMainSceneGlobal : IStateBase {

	private GameStateMainSceneGlobal()
	{}

	private static GameStateMainSceneGlobal m_instance;
	private static object m_lockHelper = new object();
	public static GameStateMainSceneGlobal Instance()
	{
		if (null == m_instance) {
			lock (m_lockHelper) {
				if (null == m_instance) {
					m_instance = new GameStateMainSceneGlobal ();
				}
			}
		}
		return m_instance;
	}
	//
	private GameMainScene m_owner;
	private Player m_localPlayer;
	private bool m_isMovingOver = true;
	public void Enter(GameStateBase owner)
	{
		Debug.Log("GamestateMainScene enter");
		m_owner = (GameMainScene)owner;
		m_owner.RollDiceCtr.RegisterRollOverNotify(RollCallback);

		GameObject prefab = Resources.Load("Player")as GameObject;
		GameObject go = GameObject.Instantiate(prefab);
		m_localPlayer = go.GetComponent<Player>();
		m_localPlayer.CurMapGrid = m_owner.GameMap.StartGrid;
		m_localPlayer.transform.position = m_localPlayer.CurMapGrid.CenterPos;

		m_owner.CameraCtr.FollowTarget = m_localPlayer.transform;
	}

	public void Execute(GameStateBase owner)
	{

		if(m_isMovingOver && Input.GetKeyUp(KeyCode.R)){
			m_owner.RollDiceCtr.Roll(15000);
			m_isMovingOver = false;
		}
	}

	public void Exit(GameStateBase owner)
	{

	}

	public void Message(string message, object[] parameters)
	{
		Debug.Log("GameStateMainScene::Message:"+message);
		if(message.Equals("MovingOver"))
		{
			m_isMovingOver = true;
		}
	}

	void RollCallback(uint num){
		Debug.Log("rollcallback:"+num);
		MapGrid target = m_localPlayer.CurMapGrid;
		for(int i=0; i<num; i++)
		{
			target = target.NextGrid;
		}
		m_localPlayer.CurMapGrid = target;
		m_localPlayer.GotoMapGrid(target);
	}
}
