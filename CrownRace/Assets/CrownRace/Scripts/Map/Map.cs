using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	[SerializeField]
	private MapGrid m_start;
	public MapGrid StartGrid{
		get{return m_start;}
	}
	[SerializeField]
	private MapGrid m_end;
	public MapGrid EndGrid{
		get{return m_end;}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
