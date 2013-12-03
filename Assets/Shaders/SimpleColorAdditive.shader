Shader "Custom/SimpleColor Additive" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
        ZWrite Off
        Cull back
        CGPROGRAM
		#pragma surface surf Lambert decal:add
		fixed4 _Color;
		struct Input {
			float4 Color;
		};
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb * _Color.a;
		}
		ENDCG
	} 
}