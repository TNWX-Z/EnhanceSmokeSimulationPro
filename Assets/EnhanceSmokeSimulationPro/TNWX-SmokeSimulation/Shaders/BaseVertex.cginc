//----------------------------------------------------------------------
//                    ESSP : Enhance Smoke Simulation Pro
//                      Copyright(c) 2017 TNWX(ÌñÄÉÎ¢Îú)
//----------------------------------------------------------------------

//-----------------------Base Vertex Shader---------------------
#include "UnityCG.cginc"
float2 _texelSize;
struct BaseV2F{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float2 uvLeft : TEXCOORD1;
	float2 uvRight : TEXCOORD2;
	float2 uvTop : TEXCOORD3;
	float2 uvDown : TEXCOORD4;
};
BaseV2F BaseVertexPragma(float4 vertex:POSITION,float2 coord:TEXCOORD0){
	BaseV2F o;
		o.pos = UnityObjectToClipPos(vertex);
		float4 screenPos = ComputeScreenPos(o.pos);
		o.uv = (o.pos.xy/o.pos.w) * 0.5 + 0.5;
		#ifdef UNITY_UV_STARTS_AT_TOP
			o.uv.y = 1.-o.uv.y;
		#endif

		o.uvLeft = o.uv - float2(_texelSize.x,0.);
		o.uvRight = o.uv + float2(_texelSize.x,0.);
		o.uvTop = o.uv + float2(0.,_texelSize.y);
		o.uvDown = o.uv - float2(0.,_texelSize.y);
	return o;
}