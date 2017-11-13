Shader "Selected Effect --- Outline/Normal Expansion/Diffuse Transparent" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_OutlineWidth ("Outline Width", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 1, 0, 1)
		_OutlineFactor ("Outline Factor", Range(0, 1)) = 1
		_Overlay ("Overlay", Float) = 0
		_OverlayColor ("Overlay Color", Color) = (1, 1, 0, 1)
		_Transparent ("Transparent", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		
		// 1st pass --- depth only pass
		ZWrite On
		ColorMask Off
		CGPROGRAM
		#pragma surface surf Lambert
		sampler2D _MainTex;
		float4 _OverlayColor;
		float _Overlay;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, _OverlayColor.rgb, _Overlay);
			o.Alpha = c.a;
		}
		ENDCG
		
		// 2nd pass --- draw transparent stuff
		ZWrite Off
		ColorMask RGBA
		ZTest Equal
		CGPROGRAM
		#pragma surface surf Lambert alpha:blend
		sampler2D _MainTex;
		float4 _OverlayColor;
		float _Overlay, _Transparent;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, _OverlayColor.rgb, _Overlay);
			o.Alpha = c.a * _Transparent;
		}
		ENDCG
		
		// 3rd pass --- draw outline
		Pass {
			ZWrite On
			ColorMask RGB
			ZTest Less
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Outline.cginc"
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
