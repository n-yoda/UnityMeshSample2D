Shader "Custom/SimpleColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
        ZWrite Off
        Cull back
        CGPROGRAM
		#pragma surface surf Lambert alpha
		fixed4 _Color;
		struct Input {
			float4 Color;
		};
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	} 
}
