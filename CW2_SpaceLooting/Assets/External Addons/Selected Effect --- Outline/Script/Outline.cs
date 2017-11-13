using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
	[Header("Parameters")]
	public Color m_OutlineColor = Color.green;
	[Range(0.01f, 0.1f)] public float m_OutlineWidth = 0.02f;
	[Range(0f, 1f)] public float m_OutlineFactor = 1f;
	public Color m_OverlayColor = Color.red;
	[Header("Auto")]
	public Material[] m_BackupMats;
	private Renderer m_Rd;

    public void Initialize ()
	{
		m_Rd = GetComponent<Renderer> ();
		m_BackupMats = m_Rd.materials;
	}
	public void SetMaterialsFloat (string name, float f)
	{
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i].SetFloat (name, f);
		}
	}
	public void UpdateSelfParameters ()
	{
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i].SetFloat ("_OutlineWidth", m_OutlineWidth);
			mats[i].SetColor ("_OutlineColor", m_OutlineColor);
			mats[i].SetFloat ("_OutlineFactor", m_OutlineFactor);
			mats[i].SetColor ("_OverlayColor", m_OverlayColor);
			mats[i].SetTexture ("_MainTex", m_BackupMats[i].GetTexture ("_MainTex"));
			mats[i].SetTextureOffset ("_MainTex", m_BackupMats[i].GetTextureOffset ("_MainTex"));
			mats[i].SetTextureScale ("_MainTex", m_BackupMats[i].GetTextureScale ("_MainTex"));
		}
	}
	public void MaterialChangeTo (Material mat)
	{
		int len = m_Rd.materials.Length;
		Material[] mats = new Material[len];
		for (int i = 0; i < len; i++)
		{
			mats[i] = mat;
		}
		m_Rd.materials = mats;
	}
	public void MaterialRevert ()
	{
		m_Rd.materials = m_BackupMats;
	}
}
