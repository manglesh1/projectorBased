Shader "Malbers/Color4x4" {
	Properties {
		_Tint ("Tint", Vector) = (1,1,1,1)
		[Header(Albedo (A Gradient))] _Color1 ("Color 1", Vector) = (1,0.1544118,0.1544118,0.291)
		_Color2 ("Color 2", Vector) = (1,0.1544118,0.8017241,0.253)
		_Color3 ("Color 3", Vector) = (0.2535501,0.1544118,1,0.541)
		_Color4 ("Color 4", Vector) = (0.1544118,0.5451319,1,0.253)
		[Space(10)] _Color5 ("Color 5", Vector) = (0.9533468,1,0.1544118,0.553)
		_Color6 ("Color 6", Vector) = (0.2720588,0.1294625,0,0.097)
		_Color7 ("Color 7", Vector) = (0.1544118,0.6151115,1,0.178)
		_Color8 ("Color 8", Vector) = (0.4849697,0.5008695,0.5073529,0.078)
		[Space(10)] _Color9 ("Color 9", Vector) = (0.3164301,0,0.7058823,0.134)
		_Color10 ("Color 10", Vector) = (0.362069,0.4411765,0,0.759)
		_Color11 ("Color 11", Vector) = (0.6691177,0.6691177,0.6691177,0.647)
		_Color12 ("Color 12", Vector) = (0.5073529,0.1574544,0,0.128)
		[Space(10)] _Color13 ("Color 13", Vector) = (1,0.5586207,0,0.272)
		_Color14 ("Color 14", Vector) = (0,0.8025862,0.875,0.047)
		_Color15 ("Color 15", Vector) = (1,0,0,0.391)
		_Color16 ("Color 16", Vector) = (0.4080882,0.75,0.4811866,0.134)
		[Header(Metallic(R) Rough(G) Emmission(B))] _MRE1 ("MRE 1", Vector) = (0,1,0,0)
		_MRE2 ("MRE 2", Vector) = (0,1,0,0)
		_MRE3 ("MRE 3", Vector) = (0,1,0,0)
		_MRE4 ("MRE 4", Vector) = (0,1,0,0)
		[Space(10)] _MRE5 ("MRE 5", Vector) = (0,1,0,0)
		_MRE6 ("MRE 6", Vector) = (0,1,0,0)
		_MRE7 ("MRE 7", Vector) = (0,1,0,0)
		_MRE8 ("MRE 8", Vector) = (0,1,0,0)
		[Space(10)] _MRE9 ("MRE 9", Vector) = (0,1,0,0)
		_MRE10 ("MRE 10", Vector) = (0,1,0,0)
		_MRE11 ("MRE 11", Vector) = (0,1,0,0)
		_MRE12 ("MRE 12", Vector) = (0,1,0,0)
		[Space(10)] _MRE13 ("MRE 13", Vector) = (0,1,0,0)
		_MRE14 ("MRE 14", Vector) = (0,1,0,0)
		_MRE15 ("MRE 15", Vector) = (0,1,0,0)
		_MRE16 ("MRE 16", Vector) = (0,1,0,0)
		[Header(Emmision)] _EmissionPower ("Emission Power", Float) = 1
		[SingleLineTexture] [Header(Gradient)] _Gradient ("Gradient", 2D) = "white" {}
		_GradientIntensity ("Gradient Intensity", Range(0, 1)) = 1
		_GradientColor ("Gradient Color", Vector) = (0,0,0,0)
		_GradientScale ("Gradient Scale", Float) = 1
		_GradientOffset ("Gradient Offset", Float) = 0
		_GradientPower ("Gradient Power", Float) = 1
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}