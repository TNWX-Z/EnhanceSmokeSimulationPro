//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/Splat"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		Tex_VelocityAndDesity ("Tex_VelocityAndDesity", 2D) = "black" {}
		pointpos ("pointpos", vector) = (0,0,0,0)
		radius ("radius", float) = 0
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

			uniform sampler2D_half Tex_VelocityAndDesity;
			uniform half3 color;
			uniform half2 pointpos;
			uniform half radius;

			half4 frag(BaseV2F i):SV_Target{
				half2 p = i.uv - pointpos.xy/_ScreenParams.xy;
				p.x *= (_ScreenParams.x/_ScreenParams.y);
				half3 splat = exp(-dot(p,p)/radius)*color;
				half3 base = tex2D(Tex_VelocityAndDesity,i.uv).rgb;
				return half4(base+splat,1.);
			}

			ENDCG
		}
	}
}
