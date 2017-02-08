using UnityEngine;
using System.Collections;
using com.crownrace.msg;

[ExecuteInEditMode]
public class MapGrid : BaseGrid {
	[SerializeField]
	protected MapGrid m_normalNextGrid;
	[SerializeField]
	protected MapGrid m_preGrid;
	[SerializeField]
	protected MapGrid m_specNextGrid;
	[SerializeField]
	protected CELL_EFFECT m_cellEffect = CELL_EFFECT.NONE;
	public CELL_EFFECT CellEffect{
		get{ return m_cellEffect;}
	}
	[SerializeField]
	protected uint m_effectKeepRound = 1;
	public uint EffectKeepRound{
		get{ return m_effectKeepRound;}
	}

	public MapGrid NextGrid{
		get{return m_normalNextGrid;}
	}
	public MapGrid PreGird{
		get{ return m_preGrid;}
	}

	protected Renderer m_renderer;
	void Start()
	{
		
		m_renderer = this.GetComponent<Renderer> ();
		Material mat;
		if (m_cellEffect == CELL_EFFECT.BACK || m_cellEffect == CELL_EFFECT.FORWARD) {
			mat = Resources.Load ("HexagonMat/hexagon_" + m_cellEffect + "_" + Mathf.Abs (m_effectKeepRound))as Material;
		}else {
			mat = Resources.Load ("HexagonMat/hexagon_" + m_cellEffect)as Material;
		}
		m_renderer.sharedMaterial = mat;
	}
}
