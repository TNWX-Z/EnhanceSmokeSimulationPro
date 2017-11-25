// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LogoEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneminusSrcAlpha
			CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert 
				#pragma fragment frag 
				#include "UnityCG.cginc"

				sampler2D _MainTex;

				struct V2F{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				V2F vert(float4 vertex:POSITION,float2 coord:TEXCOORD0){
					V2F o;
						o.pos = UnityObjectToClipPos(vertex);
						o.uv = coord;
					return o;
				}
				#define time _Time.z
				fixed4 frag(V2F i):SV_Target{
					fixed4 c = tex2D(_MainTex,i.uv);
					
					i.uv = abs(i.uv);
					float2 uvEffect = i.uv + 1.2 + float2(sin(time)*0.7,sin(time));

					return fixed4(uvEffect/2.,0.7,c.a);
				}
			ENDCG
		}
	}
}
