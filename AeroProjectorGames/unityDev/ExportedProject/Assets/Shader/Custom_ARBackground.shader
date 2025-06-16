Shader "Custom/ARBackground" {
	Properties {
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _DepthTex ("Depth", 2D) = "black" {}
		_BgColor ("Background", Vector) = (0.003921569,0.7098039,0.9333333,1)
		_MinRange ("Min Range(m)", Range(0, 10)) = 0.15
		_MaxRange ("Max Range(m)", Range(0, 20)) = 10
		[HideInInspector] _Colormaps ("Colormaps", 2D) = "" {}
		[KeywordEnum(Viridis,Plasma,Inferno,Jet,Rainbow,Coolwarm,Flag,Gray)] _Colormap ("Colormap", Float) = 0
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