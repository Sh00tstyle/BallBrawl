Shader "Custom/Shield" {
	Properties{
		_RampTex("Ramp texture", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_NoiseTex("Noise texture",2D) = "grey" {}
		_RampVal("Ramp offset", Range(-0.5, 0.5)) = 0
		_Color("Color", Color) = (1,1,1,1)
		_Speed("_Speed", Range(0, 50)) = 0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
	#pragma surface surf Lambert vertex:vert

	uniform float _Cutoff;
	sampler2D _NoiseTex;
	sampler2D _RampTex;
	fixed _RampVal;
	float4 _Color;
	float _Speed;

	struct Input {
		float2 uv_NoiseTex;
	};

	void vert(inout appdata_full v) {
		half noiseVal = tex2Dlod(_NoiseTex, float4(v.texcoord.xy, 0, 0)).r;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		half noiseVal = tex2D(_NoiseTex, IN.uv_NoiseTex).r + (sin(_Time.y)) / _Speed;
		half4 color = tex2D(_RampTex, float2(saturate(_RampVal + noiseVal), 1)) * _Color;
		if (color.a < _Cutoff)
		{
			discard;
		}


		o.Albedo = color.rgb;
		o.Emission = color.rgb;
	}
	ENDCG
	}
		FallBack "Diffuse"
}