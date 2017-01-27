using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	[SerializeField]
	private NavMeshAgent m_agentCtr;
	public MapGrid CurMapGrid;
	private bool m_isMoving=false;

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
		m_agentCtr.SetDestination(target.CenterPos);
		Debug.Log("GotoMapGrid "+target.CenterPos+","+m_agentCtr.remainingDistance);
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
			GameStateManager.Instance().FSM.GlobalState.Message("MovingOver", null);
		}
	}
}
