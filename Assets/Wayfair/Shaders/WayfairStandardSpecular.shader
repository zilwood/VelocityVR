Shader "Wayfair/StandardSpecular" {
	Properties {
		 _Color ("Color", Color) = (1,1,1,1)
		 _MainTex ("Albedo", 2D) = "white" { }
		 _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
		 _Glossiness ("Smoothness", Range(0,1)) = 0.5
		 _SpecColor ("Specular", Color) = (0.2,0.2,0.2,1)
		 _SpecGlossMap ("Specular", 2D) = "white" { }
		 _BumpScale ("Scale", Float) = 1
		 _BumpMap ("Normal Map", 2D) = "bump" { }
		 _Parallax ("Height Scale", Range(0.005,0.08)) = 0.02
		 _ParallaxMap ("Height Map", 2D) = "black" { }
		 _OcclusionStrength ("Strength", Range(0,1)) = 1
		 _OcclusionMap ("Occlusion", 2D) = "white" { }
		 _EmissionColor ("Color", Color) = (0,0,0,1)
		 _EmissionMap ("Emission", 2D) = "white" { }
		 _DetailMask ("Detail Mask", 2D) = "white" { }
		 _DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" { }
		 _DetailNormalMapScale ("Scale", Float) = 1
		 _DetailNormalMap ("Normal Map", 2D) = "bump" { }
		[Enum(UV0,0,UV1,1)]  _UVSec ("UV Set for secondary textures", Float) = 0
		[HideInInspector]  _Mode ("__mode", Float) = 0
		[HideInInspector]  _SrcBlend ("__src", Float) = 1
		[HideInInspector]  _DstBlend ("__dst", Float) = 0
		[HideInInspector]  _ZWrite ("__zw", Float) = 1
	}
	SubShader {
		Tags {"RenderType"="Opaque" "IgnoreProjector"="True" "PerformanceChecks"="False"}
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular noshadows exclude_path:deferred nofog nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		fixed4 _Color;
		float _Glossiness;
		sampler2D _MainTex;

		struct Input {
			fixed2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha= c.a;
			o.Smoothness = _Glossiness;
			o.Specular = _SpecColor.rgb;
		}
		ENDCG
	}
	FallBack "StandardSpecular"
}