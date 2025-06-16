Shader "MFEP_Stream" {
	Properties {
		_Ripples_Displacement ("Ripples_Displacement", 2D) = "gray" {}
		_Ripples ("Ripples", 2D) = "bump" {}
		_Ripples2 ("Ripples2", 2D) = "bump" {}
		_Color ("Color", Vector) = (0,0,0,0)
		_Displacement ("Displacement", Range(0, 1)) = 0
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Base_Smoothness ("Base_Smoothness", Range(0, 1)) = 0
		_Speed ("Speed", Range(0, 1)) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
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

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}