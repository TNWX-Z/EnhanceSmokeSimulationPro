//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Vorticity"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		Tex_Velocity ("Tex_Velocity", 2D) = "black" {}
		Tex_Curl ("Tex_Curl", 2D) = "black" {}
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

			uniform sampler2D_half Tex_Velocity;
			uniform sampler2D_half Tex_Curl;
			uniform half curl;
			uniform half dt_time;

			half4 frag(BaseV2F i):SV_Target{
				half L = tex2D(Tex_Curl,i.uvLeft).y;	
				half R = tex2D(Tex_Curl,i.uvRight).y;
				half T = tex2D(Tex_Curl,i.uvTop).x;
				half B = tex2D(Tex_Curl,i.uvDown).x;

				half C = tex2D(Tex_Curl,i.uv).x;

				half2 force = half2(abs(T)-abs(B),abs(R)-abs(L));
				force *= 1./length(force + 0.00001) * curl * C;
				half2 vel = tex2D(Tex_Velocity, i.uv).xy;
				return half4(vel + force * dt_time,0.,1.);
			}

			ENDCG
		}
	}
}
