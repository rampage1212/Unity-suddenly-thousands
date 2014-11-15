// xray mouse pos shader test v1.0 - mgear - http://unitycoder.com/blog

Shader "mShaders/XRay1"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ObjPos ("ObjPos", Vector) = (1,1,1,1)
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_Radius ("HoleRadius", Range(0.1,5)) = 2
	}
	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Off // draw backfaces also, comment this line if no need for backfaces
		AlphaTest Greater [_Cutoff]
		
		CGPROGRAM
//		#pragma target 3.0
		#pragma surface surf Lambert 
//		#include "UnityCG.cginc"

		struct Input 
		{
			float2 uv_MainTex;
			
//			float3 depth;
//			float4 pos;
//			float3 viewDir;
			
			float3 worldPos;
//			float3 worldRefl;
//			float3 worldNormal;
//			float4 screenPos;
//			INTERNAL_DATA
		};
		
		sampler2D _MainTex;
		uniform float4 _ObjPos;
		uniform float _Radius;

		void surf (Input IN, inout SurfaceOutput o) 
		{
		
			half3 col = tex2D (_MainTex, IN.uv_MainTex).rgb;

			float dx = length(_ObjPos.x-IN.worldPos.x);
			float dy = length(_ObjPos.y-IN.worldPos.y);
			float dz = length(_ObjPos.z-IN.worldPos.z);
			float dist = (dx*dx+dy*dy+dz*dz)*_Radius;
			dist = clamp(dist,0.5,1);
			
			o.Albedo = col; // color is from texture
			o.Alpha = dist;  // alpha is from distance to the mouse
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
