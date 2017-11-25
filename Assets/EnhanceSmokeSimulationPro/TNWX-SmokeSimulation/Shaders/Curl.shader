//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Curl"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		Tex_Velocity ("Tex_Velocity", 2D) = "black" {}
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

			half4 frag(BaseV2F i):SV_Target{
				half L = tex2D(Tex_Velocity,i.uvLeft).y;	
				half R = tex2D(Tex_Velocity,i.uvRight).y;
				half T = tex2D(Tex_Velocity,i.uvTop).x;
				half B = tex2D(Tex_Velocity,i.uvDown).x;
				half vorticity = R - L - T + B;
				return half4(vorticity,0.,0.,1.);
			}
			ENDCG
		}
	}
}
