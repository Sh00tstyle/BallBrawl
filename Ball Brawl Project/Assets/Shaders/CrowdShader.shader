Shader "Custom/Crowd" {
	Properties{
		_MainTex("Albedo", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", Range(0,2)) = 0.3
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		LOD 100
		Cull Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha


		Pass{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

		uniform sampler2D _MainTex;
		uniform float _Cutoff;
		float _Scale;
		float4 _Color;
		struct vertexInput {
			float4 vertex : POSITION;
			float4 tex : TEXCOORD0;
		};
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;
			float4 camDir = mul(UNITY_MATRIX_P,
				mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
				- float4(input.vertex.x * _Scale, input.vertex.z * _Scale, 0.0, 0.0));

			output.pos = camDir;
			output.tex = input.tex;

			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
			float2 tp = float2(input.tex.x, input.tex.y);
			float4 col = tex2D(_MainTex, tp) * _Color;

			if (col.a < _Cutoff)
			{
				discard;
			}
			return col;
		}
			ENDCG
		}
	}
}