// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Selected Effect --- Outline/Sprite" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Color ("Tint", Color) = (1, 1, 1, 1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineThickness ("Outline Thickness", Float) = 1
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ OUTLINE_ONLY
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color, _OutlineColor, _MainTex_TexelSize;
			fixed _OutlineThickness;
			
			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color * _Color;
#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
#endif
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.texcoord);
				c.rgb *= c.a;
				
				fixed4 oc = _OutlineColor;
				oc.a *= ceil(c.a);
				oc.rgb *= oc.a;
 
				fixed up = tex2D(_MainTex,    i.texcoord + fixed2(0, _MainTex_TexelSize.y * _OutlineThickness)).a;
				fixed down = tex2D(_MainTex,  i.texcoord - fixed2(0, _MainTex_TexelSize.y * _OutlineThickness)).a;
				fixed right = tex2D(_MainTex, i.texcoord + fixed2(_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
				fixed left = tex2D(_MainTex,  i.texcoord - fixed2(_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
#if OUTLINE_ONLY
				c = fixed4(0, 0, 0, 0);
#endif
				return lerp(oc, c, ceil(up * down * right * left));
			}
			ENDCG
		}
	}
}