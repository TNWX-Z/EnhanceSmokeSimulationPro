//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Pressure"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		Tex_Pressure ("Tex_Pressure", 2D) = "black" {}
		Tex_Divergence ("Tex_Divergence", 2D) = "black" {}
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

			uniform sampler2D_half Tex_Pressure;
			uniform sampler2D_half Tex_Divergence;

			half4 frag(BaseV2F i):SV_Target{
				half L = tex2D(Tex_Pressure,saturate(i.uvLeft)).x;	
				half R = tex2D(Tex_Pressure,saturate(i.uvRight)).x;
				half T = tex2D(Tex_Pressure,saturate(i.uvTop)).x;
				half B = tex2D(Tex_Pressure,saturate(i.uvDown)).x;

				half C = tex2D(Tex_Pressure,i.uv).x;
				
				half divergence = tex2D(Tex_Divergence,i.uv).x;
				half pressure = (L + R + B + T - divergence) * 0.25;
				return half4(pressure,0.,0.,1.);
			}

			ENDCG
		}
	}
}
