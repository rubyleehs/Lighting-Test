Shader "Custom/LightEffectShader"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_LightTex ("LightTex", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			sampler2D _MainTex;
            sampler2D _LightTex;

			float4 _MainTex_ST;

			struct Interpolators
			{
				float4 mainVertex : POSITION;
				float2 mainUV: TEXCOORD0;
			};

			struct VertexData
			{
				float4 mainVertex : POSITION;
				float2 mainUV : TEXCOORD0;
			};

			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;

				i.mainVertex = UnityObjectToClipPos(v.mainVertex);
				i.mainUV = TRANSFORM_TEX(v.mainUV,_MainTex);

				return i;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
                return tex2D(_MainTex, i.mainUV) *  tex2D(_LightTex, i.mainUV);
			}

			ENDCG
		}
	}
}

