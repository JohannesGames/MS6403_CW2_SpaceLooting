// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Selected Effect --- Outline/Normal Expansion/Outline Only" {
	Properties {
		_OutlineWidth ("Outline Width", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 1, 0, 0)
		_OutlineFactor ("Outline Factor", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
			Cull Back
			Blend Zero One
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			float4 frag (v2f i) : SV_Target
			{
				return float4(0, 0, 0, 0);
			}
			ENDCG
		}
		Pass {
			Cull Front
			Blend One OneMinusDstColor

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Outline.cginc"
			ENDCG
		}
	} 
	FallBack "VertexLit"
}
