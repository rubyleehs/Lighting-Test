Shader "Custom/RadialLightShader"
{
	Properties
	{
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

        _FalloffBase ("FalloffBase", float) = 0.5
		_FalloffExponent("FalloffExponent", float) = 0.5

	}


	SubShader
	{
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
		
        Pass
		{

			CGPROGRAM

			#pragma vertex MyVertexProgram alpha
			#pragma fragment MyFragmentProgram alpha

			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            sampler2D _Texture1;

			float _FalloffBase;
            float _FalloffExponent;


			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};
			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);

				i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				return i;
			}
			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
                float4 result = tex2D(_MainTex, i.uv) * _Tint;
                float sqrDeltaFromCenter = (pow(i.uv.x - 0.5,2) + pow(i.uv.y - 0.5,2));

				result.a = 1 - (1 / pow(sqrDeltaFromCenter * _FalloffBase, _FalloffExponent));
                return result;
            }
			ENDCG
		}
	}
}
