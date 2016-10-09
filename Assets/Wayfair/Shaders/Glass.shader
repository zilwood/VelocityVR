Shader "Wayfair/Glass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,0)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_SpecColor ("Specular", Color) = (0.2,0.2,0.2,1)
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="Transparent" "IgnoreProjector"="True" "ForceNoShadowCasting"="True" "PerformanceChecks"="False"}
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular alpha:auto exclude_path:deferred nofog nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;
		float _Glossiness;

		struct Input {
			fixed2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Alpha= c.a;
			o.Smoothness = _Glossiness;

			o.Specular = _SpecColor.rgb;
		}
		ENDCG
	}
	FallBack "StandardSpecular"
}
