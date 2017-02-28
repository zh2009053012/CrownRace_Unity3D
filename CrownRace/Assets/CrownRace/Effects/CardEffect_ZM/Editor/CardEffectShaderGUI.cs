using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class CardEffectShaderGUI : ShaderGUI {
	public enum BlendMode{
		Add,
		Minus,
		Multiply,
		Distort
	}
	private static class Styles{

		public static GUIContent albedoText = new GUIContent ("Albedo", "Albedo Texture");
		public static GUIContent motionText = new GUIContent("Motion Texture", "Motion Texture");
		public static GUIContent maskText = new GUIContent("Mask Texture", "Mask Texture. rgb for motion1/motion2/motion3");
		public static string distortParamText = "Distort Power";
		public static string moveParamText = "Move Direction";
		public static string rotateParamText = "Rotate Center&Speed";
	
		public static string primaryText = "Main Texture";
		public static string secondaryText = "Motion Texture 1";
		public static string  thirdText = "Motion Texture 2";
		public static string forthText = "Motion Texture 3";
		public static string fifthText = "Motion Texture 4";
	}
	MaterialProperty albedoMap = null;
	MaterialProperty albedoColor = null;
	MaterialProperty maskMap = null;

	MaterialProperty blendMode1 = null;
	MaterialProperty isMove1 = null;
	MaterialProperty isRotate1 = null;
	MaterialProperty motionMap1 = null;
	MaterialProperty motionColor1 = null;
	MaterialProperty colorParam1 = null;
	MaterialProperty distortParam1 = null;
	MaterialProperty moveParam1 = null;
	MaterialProperty rotateParam1 = null;

	MaterialProperty blendMode2 = null;
	MaterialProperty isMove2 = null;
	MaterialProperty isRotate2 = null;
	MaterialProperty motionMap2 = null;
	MaterialProperty motionColor2 = null;
	MaterialProperty colorParam2 = null;
	MaterialProperty distortParam2 = null;
	MaterialProperty moveParam2 = null;
	MaterialProperty rotateParam2 = null;

	MaterialProperty blendMode3 = null;
	MaterialProperty isMove3 = null;
	MaterialProperty isRotate3 = null;
	MaterialProperty motionMap3 = null;
	MaterialProperty motionColor3 = null;
	MaterialProperty colorParam3 = null;
	MaterialProperty distortParam3 = null;
	MaterialProperty moveParam3 = null;
	MaterialProperty rotateParam3 = null;

	MaterialProperty blendMode4 = null;
	MaterialProperty isMove4 = null;
	MaterialProperty isRotate4 = null;
	MaterialProperty motionMap4 = null;
	MaterialProperty motionColor4 = null;
	MaterialProperty colorParam4 = null;
	MaterialProperty distortParam4 = null;
	MaterialProperty moveParam4 = null;
	MaterialProperty rotateParam4 = null;

	MaterialEditor m_materialEditor;

	public void FindProperties(MaterialProperty[] props){
		albedoMap = FindProperty ("_MainTex", props);
		albedoColor = FindProperty ("_Color", props);
		maskMap = FindProperty ("_MaskTex", props);

		blendMode1 = FindProperty ("_BlendMode1", props);
		isMove1 = FindProperty ("_IsMove1", props);
		isRotate1 = FindProperty ("_IsRotate1", props);
		motionMap1 = FindProperty ("_MotionTex1", props);
		motionColor1 = FindProperty ("_MotionColor1", props);
		colorParam1 = FindProperty ("_ColorParam1", props);
		distortParam1 = FindProperty ("_DistortParam1", props);
		moveParam1 = FindProperty ("_MoveParam1", props);
		rotateParam1 = FindProperty ("_RotateParam1", props);

		blendMode2 = FindProperty ("_BlendMode2", props);
		isMove2 = FindProperty ("_IsMove2", props);
		isRotate2 = FindProperty ("_IsRotate2", props);
		motionMap2 = FindProperty ("_MotionTex2", props);
		motionColor2 = FindProperty ("_MotionColor2", props);
		colorParam2 = FindProperty ("_ColorParam2", props);
		distortParam2 = FindProperty ("_DistortParam2", props);
		moveParam2 = FindProperty ("_MoveParam2", props);
		rotateParam2 = FindProperty ("_RotateParam2", props);

		blendMode3 = FindProperty ("_BlendMode3", props);
		isMove3 = FindProperty ("_IsMove3", props);
		isRotate3 = FindProperty ("_IsRotate3", props);
		motionMap3 = FindProperty ("_MotionTex3", props);
		motionColor3 = FindProperty ("_MotionColor3", props);
		colorParam3 = FindProperty ("_ColorParam3", props);
		distortParam3 = FindProperty ("_DistortParam3", props);
		moveParam3 = FindProperty ("_MoveParam3", props);
		rotateParam3 = FindProperty ("_RotateParam3", props);

		blendMode4 = FindProperty ("_BlendMode4", props);
		isMove4 = FindProperty ("_IsMove4", props);
		isRotate4 = FindProperty ("_IsRotate4", props);
		motionMap4 = FindProperty ("_MotionTex4", props);
		motionColor4 = FindProperty ("_MotionColor4", props);
		colorParam4 = FindProperty ("_ColorParam4", props);
		distortParam4 = FindProperty ("_DistortParam4", props);
		moveParam4 = FindProperty ("_MoveParam4", props);
		rotateParam4 = FindProperty ("_RotateParam4", props);
	}

	public void Reset()
	{
		Debug.Log("reset");
	}

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties (props);
		m_materialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		ShaderPropertiesGUI (material);
	}

	public void ShaderPropertiesGUI(Material material){
		EditorGUIUtility.labelWidth = 0f;

		EditorGUI.BeginChangeCheck ();
		{
			GUILayout.Label (Styles.primaryText, EditorStyles.boldLabel);
			m_materialEditor.TexturePropertySingleLine (Styles.albedoText, albedoMap, albedoColor);
			m_materialEditor.TexturePropertySingleLine (Styles.maskText, maskMap);
			m_materialEditor.TextureScaleOffsetProperty (albedoMap);

			EditorGUILayout.Space ();

			GUILayout.Label (Styles.secondaryText, EditorStyles.boldLabel);
			MotionToggle (material, 1, blendMode1, distortParam1, moveParam1, rotateParam1, motionMap1, motionColor1, colorParam1);
			if(motionMap1.textureValue != null)
				m_materialEditor.TextureScaleOffsetProperty (motionMap1);
			//
			EditorGUILayout.Space ();

			GUILayout.Label (Styles.thirdText, EditorStyles.boldLabel);
			MotionToggle (material, 2, blendMode2, distortParam2, moveParam2, rotateParam2, motionMap2, motionColor2, colorParam2);
			if(motionMap2.textureValue != null)
				m_materialEditor.TextureScaleOffsetProperty (motionMap2);
			//
			EditorGUILayout.Space ();

			GUILayout.Label (Styles.forthText, EditorStyles.boldLabel);
			MotionToggle (material, 3, blendMode3, distortParam3, moveParam3, rotateParam3, motionMap3, motionColor3, colorParam3);
			if(motionMap3.textureValue != null)
				m_materialEditor.TextureScaleOffsetProperty (motionMap3);
			//
			EditorGUILayout.Space ();

			GUILayout.Label (Styles.fifthText, EditorStyles.boldLabel);
			MotionToggle (material, 4, blendMode4, distortParam4, moveParam4, rotateParam4, motionMap4, motionColor4, colorParam4);
			if(motionMap4.textureValue != null)
				m_materialEditor.TextureScaleOffsetProperty (motionMap4);
		}
		if (EditorGUI.EndChangeCheck ()) {
			foreach(Material mat in blendMode1.targets)
			{
				BlendMode blendMode = (BlendMode)blendMode1.floatValue;
				SetKeywords(mat, 1, blendMode);
			}
			foreach(Material mat in blendMode2.targets)
			{
				BlendMode blendMode = (BlendMode)blendMode2.floatValue;
				SetKeywords(mat, 2, blendMode);
			}
			foreach(Material mat in blendMode3.targets)
			{
				BlendMode blendMode = (BlendMode)blendMode3.floatValue;
				SetKeywords(mat, 3, blendMode);
			}
			foreach(Material mat in blendMode4.targets)
			{
				BlendMode blendMode = (BlendMode)blendMode4.floatValue;
				SetKeywords(mat, 4, blendMode);
			}
		}
	}

	void MotionToggle(Material mat, int num, MaterialProperty blendProp, MaterialProperty distortProp, MaterialProperty moveProp, 
		MaterialProperty rotateProp, MaterialProperty motionMap, MaterialProperty motionColor, MaterialProperty colorParam){
		bool isMotionOn = motionMap.textureValue != null;
		//
		BlendMode blendMode = (BlendMode)blendProp.floatValue;
		EditorGUI.BeginChangeCheck ();
		blendMode = (BlendMode)EditorGUILayout.Popup("Blend Mode", (int)blendMode, Enum.GetNames(typeof(BlendMode)));
		if (EditorGUI.EndChangeCheck ()) {
			blendProp.floatValue = (float)blendMode;
			m_materialEditor.RegisterPropertyChangeUndo("Blend Mode");
			//Debug.Log (blendMode);
			SetKeywords(mat, num, blendMode);
		}
		//
		if (isMotionOn) {
			switch (blendMode) {
			case BlendMode.Add:
				break;
			case BlendMode.Distort:
				m_materialEditor.ShaderProperty (distortProp, "    "+Styles.distortParamText);
				break;
			case BlendMode.Minus:
				break;
			case BlendMode.Multiply:
				break;
			}
		}
		//
		bool isMove = Array.IndexOf (mat.shaderKeywords, "_MOTION" + num + "_MOVE_ON") != -1;
		EditorGUI.BeginChangeCheck ();
		isMove = EditorGUILayout.Toggle ("Use Move", isMove);
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword (mat, "_MOTION" + num + "_MOVE_ON", isMove);
		}
		if (isMotionOn && isMove) {
			m_materialEditor.ShaderProperty (moveProp, "    " + Styles.moveParamText);
		}
		//
		bool isRotate = Array.IndexOf (mat.shaderKeywords, "_MOTION" + num + "_ROTATE_ON") != -1;
		EditorGUI.BeginChangeCheck ();
		isRotate = EditorGUILayout.Toggle ("Use Rotate", isRotate);
		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword (mat, "_MOTION" + num + "_ROTATE_ON", isRotate);
		}
		if (isMotionOn && isRotate) {
			m_materialEditor.ShaderProperty (rotateProp, "    " + Styles.rotateParamText);
		}
		//
		EditorGUI.BeginChangeCheck ();
		m_materialEditor.TexturePropertySingleLine (Styles.motionText, motionMap, motionColor);

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword (mat, "_MOTION"+num+"_ON", motionMap.textureValue != null);
		}
		//
		if(isMotionOn)
			m_materialEditor.ShaderProperty (colorParam, "Color Param[col=col*(x+y*max(0, sin(t*z+w)))]");
	}
	static void SetKeywords(Material mat, int num, BlendMode blendMode)
	{
		SetKeyword (mat, "_MOTION" + num + "_BLEND_DISTORT_ON", blendMode == BlendMode.Distort);
		SetKeyword (mat, "_MOTION" + num + "_BLEND_ADD_ON", blendMode == BlendMode.Add);
		SetKeyword (mat, "_MOTION" + num + "_BLEND_MINUS_ON", blendMode == BlendMode.Minus);
		SetKeyword (mat, "_MOTION" + num + "_BLEND_MULTIPLY_ON", blendMode == BlendMode.Multiply);
	}
	static void SetKeyword(Material mat, string keyword, bool state)
	{
		if (state) {
			mat.EnableKeyword (keyword);
		}else {
			mat.DisableKeyword (keyword);
		}
	}
}
