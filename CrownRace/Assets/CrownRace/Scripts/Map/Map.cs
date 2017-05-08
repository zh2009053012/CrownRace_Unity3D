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

	public void Init(){
		uint id = 0;
		MapGrid temp = StartGrid;
		temp.ID = id++;
		while (temp.NextGrid != null) {
			temp = temp.NextGrid;
			temp.ID = id++;
		}
	}
	public override void Awake ()
	{
		
	}
}
