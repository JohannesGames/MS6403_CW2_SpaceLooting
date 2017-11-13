// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Selected Effect --- Outline/Post Process/Depth Only" {
	Properties {
	}
	SubShader {
		Pass {
			Zwrite On
			ColorMask 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"			
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				return float4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
	FallBack Off
}