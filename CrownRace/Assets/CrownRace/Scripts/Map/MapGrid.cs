using UnityEngine;
using System.Collections;

public class MapGrid : BaseGrid {
	[SerializeField]
	protected MapGrid m_normalNextGrid;
	[SerializeField]
	protected MapGrid m_preGrid;
	[SerializeField]
	protected MapGrid m_specNextGrid;

	public MapGrid NextGrid{
		get{return m_normalNextGrid;}
	}
}
