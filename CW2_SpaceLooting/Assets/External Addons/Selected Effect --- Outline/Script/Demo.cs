using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class Demo : MonoBehaviour   //found on Asset Store
{
	public Material m_OutlineNormalExpansion;
	public enum ETech { ET_NormalExpansion, ET_PostProcess };
	public ETech m_Tech = ETech.ET_NormalExpansion;
	
	[Header("Normal Expansion")]
	[Range(0f, 0.6f)] public float m_Overlay = 0f;
	public bool m_OverlayFlash = false;
	[Range(1f, 6f)] public float m_OverlayFlashSpeed = 3f;
	public bool m_OutlineOnly = false;
	
	[Header("Post Process")]
	public Shader m_SdrGlowFlatColor;
	public Shader m_SdrDepthOnly;
	public Material m_MatGlowHalo;
	public Material m_MatGlowBlur;
	public bool m_Obstacle = false;
	[Range(1, 3)] public int m_IteratorTimes = 3;
	[Range(0.2f, 2.2f)] public float m_BlurPixelOffset = 1.2f;
	[Range(1f, 6f)] public float m_GlowIntensity = 3f;
	
	[Header("Auto")]
	public Outline[] m_Outlines;
	
	private ETech m_PrevTech;
	private Camera m_Camera;
	public GameObject m_PrevMouseOn;
	private Material m_BackupMaterial;
	private Camera m_RTCam;
	public string m_GlowLayerName = "Outline";  // use your own layer name
	
	void Start ()
	{
		m_PrevTech = m_Tech;
		m_Camera = GetComponent<Camera> ();
		QualitySettings.antiAliasing = 8;
		
		// camera for post process outline
		m_RTCam = new GameObject().AddComponent<Camera> ();
		m_RTCam.name = "RTCam";
		m_RTCam.transform.parent = m_Camera.gameObject.transform;
        m_RTCam.enabled = false;
		
		//m_Outlines = GameObject.FindObjectsOfType<Outline> ();
		for (int i = 0; i < m_Outlines.Length; i++)
			m_Outlines[i].Initialize ();
	}
	void Update ()
	{
		// when tech changed, do cleanup logic
		if (m_PrevTech != m_Tech)
		{
			if (m_PrevMouseOn)
			{
				// revert ET_NormalExpansion tech material
				Outline fx = m_PrevMouseOn.GetComponent<Outline> ();
				fx.MaterialRevert ();

				// revert ET_PostProcess layer
				m_PrevMouseOn.layer = LayerMask.NameToLayer ("Default");
				m_PrevMouseOn = null;
			}
			m_PrevTech = m_Tech;
		}
		
		if (Input.GetMouseButton (1))
		{
			RaycastHit hit;
			Ray ray = m_Camera.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit))
			{
				if (m_PrevMouseOn != hit.transform.gameObject)
					OnSelectedGameObjectChange (m_PrevMouseOn, hit.transform.gameObject);
				m_PrevMouseOn = hit.transform.gameObject;
			}
			else
			{
				OnSelectedGameObjectChange (m_PrevMouseOn, null);
				m_PrevMouseOn = null;
			}
		}
		
		// different tech use different logic
		if (m_Tech == ETech.ET_NormalExpansion)
		{
			if (m_PrevMouseOn)
			{
				if (m_OverlayFlash)
				{
					float curve = Mathf.Sin (Time.time * m_OverlayFlashSpeed) * 0.5f + 0.5f;
					m_Overlay = curve * 0.6f;
				}
				Outline fx = m_PrevMouseOn.GetComponent<Outline> ();
				fx.UpdateSelfParameters ();
				fx.SetMaterialsFloat ("_Overlay", m_Overlay);
			}
			if (m_OutlineOnly)
				m_OutlineNormalExpansion.shader = Shader.Find ("Selected Effect --- Outline/Normal Expansion/Outline Only");
			else
				m_OutlineNormalExpansion.shader = Shader.Find ("Selected Effect --- Outline/Normal Expansion/Diffuse");
		}
		else if (m_Tech == ETech.ET_PostProcess)
		{
			// nothing need to do
		}
	}
	void OnSelectedGameObjectChange (GameObject prev, GameObject curr)
	{
		OutlineApply (prev, curr);
	}
	void DoBlurPass (RenderTexture input, RenderTexture output, bool vertical)
	{
		if (vertical)
		{
			m_MatGlowBlur.SetVector ("_Offsets", new Vector4 (0f, m_BlurPixelOffset, 0f, 0f));
			Graphics.Blit (input, output, m_MatGlowBlur);
		}
		else
		{
			m_MatGlowBlur.SetVector ("_Offsets", new Vector4 (m_BlurPixelOffset, 0f, 0f, 0f));
			Graphics.Blit (input, output, m_MatGlowBlur);
		}
	}
	void OnRenderImage (RenderTexture src, RenderTexture dst)
    {
		if (m_Tech == ETech.ET_NormalExpansion)
		{
			m_RTCam.CopyFrom (m_Camera);  // if I don't do this, unity will complain "Screen position out of view frustum"...
			Graphics.Blit (src, dst);
		}
		else if (m_Tech == ETech.ET_PostProcess)
		{
			Graphics.Blit (src, dst);   // prepare main back framebuffer
		
			m_RTCam.CopyFrom (m_Camera);
			m_RTCam.clearFlags = CameraClearFlags.Color;
			m_RTCam.backgroundColor = Color.black;   // r as mask, should be black here

			RenderTexture tempRT = RenderTexture.GetTemporary (src.width, src.height, 16, RenderTextureFormat.R8);
			m_RTCam.targetTexture = tempRT;
		
			if (m_Obstacle)   // if obstacle enable, build depth buffer without glow object
			{
				m_RTCam.cullingMask = ~(1 << LayerMask.NameToLayer (m_GlowLayerName));
				m_RTCam.RenderWithShader (m_SdrDepthOnly, "");
				m_RTCam.clearFlags = CameraClearFlags.Nothing;  // don't clear, keep depth buffer
			}
			m_RTCam.cullingMask = 1 << LayerMask.NameToLayer (m_GlowLayerName);
			m_RTCam.RenderWithShader (m_SdrGlowFlatColor, "");

			// blur pass
			int shrink = 1;  // shrink == 1 for best quality, try 2 or 4 for low quality but better performance
			RenderTexture rt0 = RenderTexture.GetTemporary (Screen.width / shrink, Screen.height / shrink, 0);
			RenderTexture rt1 = RenderTexture.GetTemporary (Screen.width / shrink, Screen.height / shrink, 0);
			if (1 == m_IteratorTimes)
			{
				DoBlurPass (tempRT, rt0, true);
				DoBlurPass (rt0, rt1, false);
			}
			else if (2 == m_IteratorTimes)
			{
				DoBlurPass (tempRT, rt0, true);
				DoBlurPass (rt0, rt1, false);
				DoBlurPass (rt1, rt0, true);
				DoBlurPass (rt0, rt1, false);
			}
			else if (3 == m_IteratorTimes)
			{
				DoBlurPass (tempRT, rt0, true);
				DoBlurPass (rt0, rt1, false);
				DoBlurPass (rt1, rt0, true);
				DoBlurPass (rt0, rt1, false);
				DoBlurPass (rt1, rt0, true);
				DoBlurPass (rt0, rt1, false);
			}
			// copy the temporary RT to the final image
			m_MatGlowHalo.SetTexture ("_GlowObjectTex", tempRT);
			if (m_PrevMouseOn != null)
			{
				Outline fx = m_PrevMouseOn.GetComponent<Outline> ();
				m_MatGlowHalo.SetColor ("_GlowColor", fx.m_OutlineColor);
			}
			m_MatGlowHalo.SetFloat ("_GlowIntensity", m_GlowIntensity);
			Graphics.Blit(rt1, dst, m_MatGlowHalo);

			RenderTexture.ReleaseTemporary (tempRT);
			RenderTexture.ReleaseTemporary (rt0);
			RenderTexture.ReleaseTemporary (rt1);
		}
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void OutlineApply (GameObject prev, GameObject curr)
	{
		if (m_Tech == ETech.ET_NormalExpansion)
		{
			// revert prev to normal material
			if (prev != null)
			{
				Outline fx = prev.GetComponent<Outline> ();
				if (fx)
					fx.MaterialRevert ();
			}
			// replace current selected gameobject to outline material
			if (curr != null)
			{
				Outline fx = curr.GetComponent<Outline> ();
				if (fx)
					fx.MaterialChangeTo (m_OutlineNormalExpansion);
			}
		}
		else if (m_Tech == ETech.ET_PostProcess)
		{
			if (prev != null)
				prev.layer = LayerMask.NameToLayer ("Default");
			if (curr != null)
				curr.layer = LayerMask.NameToLayer (m_GlowLayerName);
		}
	}
}