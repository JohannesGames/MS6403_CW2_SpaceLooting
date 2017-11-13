using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class OutlineTransparent : MonoBehaviour
{
	[Header("Parameters")]
	[Range(0f, 1f)] public float m_Transparent = 0.5f;
	public Color m_OutlineColor = Color.green;
	[Range(0.01f, 0.1f)] public float m_OutlineWidth = 0.02f;
	[Range(0f, 1f)] public float m_OutlineFactor = 1f;
	[Range(0f, 0.6f)] public float m_Overlay = 0f;
	public Color m_OverlayColor = Color.red;
	[Header("Auto")]
	private Renderer m_Rd;

    void Start ()
	{
		m_Rd = GetComponent<Renderer> ();
	}
	void Update ()
	{
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i].SetFloat ("_Transparent", m_Transparent);
			mats[i].SetFloat ("_OutlineWidth", m_OutlineWidth);
			mats[i].SetColor ("_OutlineColor", m_OutlineColor);
			mats[i].SetFloat ("_OutlineFactor", m_OutlineFactor);
			mats[i].SetFloat ("_Overlay", m_Overlay);
			mats[i].SetColor ("_OverlayColor", m_OverlayColor);
		}
	}
}
