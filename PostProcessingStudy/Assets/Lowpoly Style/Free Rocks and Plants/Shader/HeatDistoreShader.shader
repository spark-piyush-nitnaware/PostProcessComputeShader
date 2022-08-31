Shader "Piyush/Heat Haze"
{
	Properties
	{
		[HideInInspector]
		_MainTex("Texture", 2D) = "white"{}
		_DistortTex("DistortTexture", 2D) = "bump" {}
		_DistortStrength("DistortStrength", Range(0, 1)) = 1.0
		_DistortSpeed("DistortSpeed", float) = 1
	}

	SubShader
	{
		HLSLINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		float4 _MainTex_ST;

		sampler2D _DistortTex;
		float4 _DistortTex_ST;

		float _DistortStrength;
		float _DistortSpeed;
		ENDHLSL

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : SV_TARGET
			{
				float timeSin = sin(_Time * _DistortSpeed);
				float2 uv = i.uv;
				float4 normalMap = tex2D(_DistortTex, uv);
				uv = i.uv;
				uv += (normalMap.r * _DistortStrength * timeSin);
				float4 color = tex2D(_MainTex, uv);
				return color;
			}
			ENDHLSL
		}
	}
}
