//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Divergence"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
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

			half2 GetVelocity (half2 uv) {
				half2 reverse = half2(1.0, 1.0);
				if (uv.x < 0.0) { uv.x = 0.0; reverse.x = -1.0; }
				if (uv.x > 1.0) { uv.x = 1.0; reverse.x = -1.0; }
				if (uv.y < 0.0) { uv.y = 0.0; reverse.y = -1.0; }
				if (uv.y > 1.0) { uv.y = 1.0; reverse.y = -1.0; }
				return reverse * tex2D(Tex_Velocity, uv).xy;
			}

			half4 frag(BaseV2F i):SV_Target{
				half L = GetVelocity(i.uvLeft).x;
				half R = GetVelocity(i.uvRight).x;
				half T = GetVelocity(i.uvTop).y;
				half B = GetVelocity(i.uvDown).y;
				half div = 0.5*(R-L + T-B);
				return half4(div,0.,0.,1.);
			}
			ENDCG
		}
	}
}
