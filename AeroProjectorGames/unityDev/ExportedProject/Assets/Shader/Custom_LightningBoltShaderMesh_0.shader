Shader "Custom/LightningBoltShaderMesh" {
	Properties {
		[PerRendererData] _MainTex ("Main Texture (RGBA)", 2D) = "white" {}
		[PerRendererData] _TintColor ("Tint Color (RGB)", Vector) = (1,1,1,1)
		[PerRendererData] _InvFade ("Soft Particles Factor", Range(0.01, 100)) = 1
		[PerRendererData] _JitterMultiplier ("Jitter Multiplier (Float)", Float) = 0
		[PerRendererData] _Turbulence ("Turbulence (Float)", Float) = 0
		[PerRendererData] _TurbulenceVelocity ("Turbulence Velocity (Vector)", Vector) = (0,0,0,0)
		[PerRendererData] _IntensityFlicker ("Intensity flicker (Vector)", Vector) = (0,0,0,0)
		[PerRendererData] _RenderMode ("Render Mode - 0 = perspective, 1 = orthoxy, 2 = orthoxz (Int)", Float) = 0
		[PerRendererData] _SrcBlendMode ("SrcBlendMode (Source Blend Mode)", Float) = 5
		[PerRendererData] _DstBlendMode ("DstBlendMode (Destination Blend Mode)", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y));
			}

			ENDHLSL
		}
	}
}