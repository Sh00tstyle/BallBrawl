Shader "Custom/Crowd" {
	Properties{
	_Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", Range(0,2)) = 0.3
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag

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

			output.pos = camDir ;
			output.tex = input.tex;

			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
		return _Color;
		}
			ENDCG
		}
	}
}