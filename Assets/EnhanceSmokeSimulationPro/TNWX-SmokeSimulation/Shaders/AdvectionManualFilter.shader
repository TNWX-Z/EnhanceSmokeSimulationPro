//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(恬纳微晰)
//----------------------------------------------------------------------
Shader "TNWX/AdvectionManualFilter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		Tex_Velocity ("Tex_Velocity", 2D) = "black" {}
		Tex_VelocityAndDesity ("Tex_VelocityAndDesity", 2D) = "black" {}
		dt_time ("dt_time", float) = 0
		Diffusion ("Diffusion", float) = 0
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
			uniform sampler2D_half Tex_VelocityAndDesity;
			uniform half dt_time;
			uniform half Diffusion;

			uniform half2 BufferSize;

			half4 bilerp(sampler2D_half sam,float2 p){
				float4 st;
				st.xy = floor(p-0.5) + 0.5;
				st.zw = st.xy + 1.0;

				half4 uv = st * _texelSize.xyxy;
				half4 a = tex2D(sam, uv.xy); //(0,0)
				half4 b = tex2D(sam, uv.zy); //(1,0)
				half4 c = tex2D(sam, uv.xw); //(0,1)
				half4 d = tex2D(sam, uv.zw); //(1,1)
				half2 f = p - st.xy;
				return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
			}
			half4 frag(BaseV2F i):SV_Target{
				half2 coord = i.uv.xy*BufferSize.xy - dt_time * tex2D(Tex_Velocity, i.uv.xy).xy;
				half4 col = Diffusion * bilerp(Tex_VelocityAndDesity, coord);
				col.a = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
