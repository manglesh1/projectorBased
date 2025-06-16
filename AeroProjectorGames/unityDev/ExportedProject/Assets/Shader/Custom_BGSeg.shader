Shader "Custom/BGSeg" {
	Properties {
		[PerRendererData] [NoScaleOffset] _MainTex ("MainTex", 2D) = "black" {}
		[NoScaleOffset] _ColorTex ("Base (RGB)", 2D) = "white" {}
		_MinRange ("Min Range(m)", Float) = 0.5
		_MaxRange ("Max Range(m)", Float) = 2
		_Feather ("Feather Range(m)", Float) = 0.25
		_DepthScale ("Depth Multiplyer Factor to Meters", Float) = 0.001
		_Gamma ("Gamma", Float) = 1
		[Toggle] _HasColor ("Has Color?", Float) = 1
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