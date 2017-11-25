//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/MagicColor"
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

			uniform sampler2D_half _MainTex;

			fixed4 frag(BaseV2F i):SV_Target{
				half4 col = tex2D(_MainTex, i.uv);
				col = pow(col,half4(2.2,3.2,1.,1.));
				col = col/(col+1.);
				return col;
			}

			ENDCG
		}
	}
}
