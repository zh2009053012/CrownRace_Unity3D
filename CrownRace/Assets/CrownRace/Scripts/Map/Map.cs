using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : UnitySingleton<Map> {
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
		
}
