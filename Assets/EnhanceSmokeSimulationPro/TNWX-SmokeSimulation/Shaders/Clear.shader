//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Clear"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Tex ("_Tex", 2D) = "black" {}
	}
	SubShader
	{
		Pass
		{
			Blend Off
			Lighting Off
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex BaseVertexPragma
			#pragma fragment frag
			#include "BaseVertex.cginc"

			uniform half4 dissipate;
			uniform sampler2D_half _Tex;

			half4 frag(BaseV2F i):SV_Target{
				return dissipate*tex2D(_Tex,i.uv);
			}

			ENDCG
		}
	}
}
