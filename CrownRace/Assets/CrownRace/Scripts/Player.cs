using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	[SerializeField]
	private NavMeshAgent m_agentCtr;
	public MapGrid CurMapGrid;
	private bool m_isMoving=false;
	private int m_player_id;
	public int PlayerID{
		get{ return m_player_id;}
		set{ m_player_id = value;}
	}
	private string m_res_name;
	public string ResName{
		get{ return m_res_name;}
		set{ m_res_name = value;}
	}

	void Awake(){
		if(null == m_agentCtr)
		{
			m_agentCtr = GetComponent<NavMeshAgent>();
		}
	}

	public bool GotoMapGrid(MapGrid target)
	{
		if(!m_isMoving && null == target)
			return false;
		m_isMoving = true;
		m_agentCtr.SetDestination(target.PlayerPos(ResName));
		//Debug.Log("GotoMapGrid "+target.CenterPos+","+m_agentCtr.remainingDistance);
		return true;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(m_isMoving && m_agentCtr.hasPath && m_agentCtr.remainingDistance <= m_agentCtr.speed*Time.deltaTime)
		{
			Debug.Log("player::Update:"+m_agentCtr.speed*Time.deltaTime+","+m_agentCtr.path);
			m_isMoving = false;
			object[] p = new object[1];
			p[0] = (object)PlayerID;
			GameStateManager.Instance().FSM.CurrentState.Message("MovingOver", p);
		}
	}
}
