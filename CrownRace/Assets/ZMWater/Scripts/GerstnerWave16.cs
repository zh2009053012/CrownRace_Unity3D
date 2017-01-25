using UnityEngine;
using System.Collections;

public class GerstnerWave16 : MonoBehaviour {

	const int WaveNum = 16;

	public float[] Steepness=new float[WaveNum/4];//value range:[0, 1]
	//Steepness*Amplitude
	public Vector4[] QAmp= new Vector4[WaveNum/4]{
		new Vector4(0.0094f,0.0113f,0.0137f,0.0164f),
		new Vector4(0.0196f,0.0239f,0.0268f,0.0341f),
		new Vector4(0.0411f,0.0494f,0.0592f,0.0709f),
		new Vector4(0.086f,0.101f,0.121f,0.149f)
	};
	public Vector4[] Amp= new Vector4[WaveNum/4]{
		new Vector4(0.0298f,0.0298f,0.0298f,0.0298f),
		new Vector4(0.0298f,0.0298f,0.0298f,0.0298f),
		new Vector4(0.0298f,0.0298f,0.0298f,0.0298f),
		new Vector4(0.0298f,0.0298f,0.0298f,0.0298f)};//amplitude
	public Vector4[] Length= new Vector4[WaveNum/4]{
		new Vector4(10,10,10,10),
		new Vector4(10,10,10,10),
		new Vector4(10,10,10,10),
		new Vector4(10,10,10,10)
	};//waveLength
	public Vector4[] Speed= new Vector4[WaveNum/4]{
		new Vector4(0.46f,0.42f,0.38f,0.32f),
		new Vector4(0.32f,0.27f,0.25f,0.25f),
		new Vector4(0.21f,0.188f,0.18f,0.15f),
		new Vector4(0.086f,0.105f,0.086f,0.117f)
	};

	public Vector2 Dir1=new Vector2(0.0f, 3.13f);
	public Vector2 Dir2=new Vector2(0.631f, 2.54f);
	public Vector2 Dir3=new Vector2(-1.23f,1.8f);
	public Vector2 Dir4=new Vector2(0.0f, 1.8f);

	public Vector2 Dir5=new Vector2(0.615f, 1.37f);
	public Vector2 Dir6=new Vector2(0.788f, 0.98f);
	public Vector2 Dir7=new Vector2(0.478f, 0.94f);
	public Vector2 Dir8=new Vector2(0.513f, 0.7f);

	public Vector2 Dir9=new Vector2(-0.26f, 0.682f);
	public Vector2 Dir10=new Vector2(-0.209f, 0.572f);
	public Vector2 Dir11=new Vector2(-0.358f, 0.356f);
	public Vector2 Dir12=new Vector2(-0.056f, 0.419f);

	public Vector2 Dir13=new Vector2(-0.115f, 0.333f);
	public Vector2 Dir14=new Vector2(-0.025f, 0.294f);
	public Vector2 Dir15=new Vector2(0.101f, 0.223f);
	public Vector2 Dir16=new Vector2(-0.1f, 0.176f);
	
	private static Vector4[] Dx = new Vector4[WaveNum/4];
	private static Vector4[] Dz = new Vector4[WaveNum/4];
	private static Vector4[] W = new Vector4[WaveNum/4];//W=2*PI/waveLength
	private static Vector4[] S = new Vector4[WaveNum/4];
	private static Vector4[] QA = new Vector4[WaveNum/4];
	private static Vector4[] A = new Vector4[WaveNum/4];
	
	public Renderer[] m_renderers;
	// Use this for initialization
	void Start () 
	{
		m_renderers = GetComponentsInChildren<Renderer> ();
		SetParams (m_renderers);
	}
	
	void SetParams(Renderer[] renderers)
	{
		if (null == renderers)
			return;
//		Dir1.Normalize ();
//		Dir2.Normalize ();
//		Dir3.Normalize ();
//		Dir4.Normalize ();

		Dx[0] = new Vector4 (Dir1.x, Dir2.x, Dir3.x, Dir4.x);
		Dx[1] = new Vector4 (Dir5.x, Dir6.x, Dir7.x, Dir8.x);
		Dx[2] = new Vector4 (Dir9.x, Dir10.x, Dir11.x, Dir12.x);
		Dx[3] = new Vector4 (Dir13.x, Dir14.x, Dir15.x, Dir16.x);
		Dz[0] = new Vector4 (Dir1.y, Dir2.y, Dir3.y, Dir4.y);
		Dz[1] = new Vector4 (Dir5.y, Dir6.y, Dir7.y, Dir8.y);
		Dz[2] = new Vector4 (Dir9.y, Dir10.y, Dir11.y, Dir12.y);
		Dz[3] = new Vector4 (Dir13.y, Dir14.y, Dir15.y, Dir16.y);
		
		//Q = new Vector4(Steepness/(W.x*A.x*4), Steepness/(W.y*A.y*4), Steepness/(W.z*A.z*4), Steepness/(W.w*A.w*4));

		for (int i = 0; i < renderers.Length; i++) {
			for(int n=0; n<4; n++)
			{
				W[n] = new Vector4(2 * Mathf.PI/ Length[n].x, 2 * Mathf.PI/ Length[n].y, 2 * Mathf.PI/ Length[n].z, 2 * Mathf.PI/ Length[n].w) ;
				S[n] = Speed[n]*5;
				QA[n] = QAmp[n];
				A[n] = Amp[n];

				renderers [i].sharedMaterial.SetVector ("_QA"+n.ToString(), QAmp[n]);
				renderers [i].sharedMaterial.SetVector ("_A"+n.ToString(), Amp[n]);
				renderers [i].sharedMaterial.SetVector ("_Dx"+n.ToString(), Dx[n]);
				renderers [i].sharedMaterial.SetVector ("_Dz"+n.ToString(), Dz[n]);
				renderers [i].sharedMaterial.SetVector ("_S"+n.ToString(), S[n]);
				renderers [i].sharedMaterial.SetVector ("_W"+n.ToString(), W[n]);
			}
		}
	}
	public static Vector4 Mul(Vector4 a, Vector4 b)
	{
		return new Vector4 (a.x*b.x, a.y*b.y, a.z*b.z, a.w*b.w);
	}
	public static Vector4 Sin(Vector4 x)
	{
		return new Vector4 (Mathf.Sin(x.x), Mathf.Sin(x.y), Mathf.Sin(x.z), Mathf.Sin(x.w));
	}
	public static Vector4 Cos(Vector4 x)
	{
		return new Vector4 (Mathf.Cos(x.x), Mathf.Cos(x.y), Mathf.Cos(x.z), Mathf.Cos(x.w));
	}

	public static Vector3 CalculateShipMovement(Vector3 worldPos)
	{
		Vector3 move = Vector3.zero;
		for(int n=0; n<4; n++)
		{
			Vector4 phase = Dx[n] * worldPos.x + Dz[n] * worldPos.z + S[n] * Time.time;
			Vector4 sinp = Vector4.zero, cosp = Vector4.zero;
			
			sinp = Sin ( phase);
			cosp = Cos ( phase);
			
			//displacement
			move.y += Vector4.Dot (A[n], sinp);
			//normal 
			move.x -= Vector4.Dot (cosp, Mul(Dx[n], QA[n]));
			move.z -= Vector4.Dot (cosp, Mul(Dz[n], QA[n]));
		}
		
		return move;
	}
	
	// Update is called once per frame
	void Update () {
		//can be removed
		//SetParams (m_renderers);
	}
}
