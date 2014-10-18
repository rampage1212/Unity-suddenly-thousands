Shader "AlpacaSound/RetroPixel6" 
{
	Properties
	{
		_Color0 ("Color 0", Color) = (0.00, 0.00, 0.00, 1.0)	
		_Color1 ("Color 1", Color) = (0.14, 0.14, 0.14, 1.0)	
		_Color2 ("Color 2", Color) = (0.29, 0.29, 0.29, 1.0)	
		_Color3 ("Color 3", Color) = (0.43, 0.43, 0.43, 1.0)	
		_Color4 ("Color 4", Color) = (0.57, 0.57, 0.57, 1.0)	
		_Color5 ("Color 5", Color) = (0.71, 0.71, 0.71, 1.0)	
	 	_MainTex ("", 2D) = "white" {}
	}
	 
	SubShader
	{
		Lighting Off
		ZTest Always
		Cull Off
		ZWrite Off
		Fog { Mode Off }
	 
	 	Pass
	 	{
	  		CGPROGRAM
	  		#pragma exclude_renderers flash
	  		#pragma vertex vert_img
	  		#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
	  		#include "UnityCG.cginc"
	    
			uniform fixed4 _Color0;
			uniform fixed4 _Color1;
			uniform fixed4 _Color2;
			uniform fixed4 _Color3;
			uniform fixed4 _Color4;
			uniform fixed4 _Color5;
	  		uniform sampler2D _MainTex;
	    
	  		fixed4 frag (v2f_img i) : COLOR
	  		{
	   			fixed3 original = tex2D (_MainTex, i.uv).rgb;

	   			fixed dist0 = distance (original, _Color0.rgb);
	   			fixed dist1 = distance (original, _Color1.rgb);
	   			fixed dist2 = distance (original, _Color2.rgb);
	   			fixed dist3 = distance (original, _Color3.rgb);
	   			fixed dist4 = distance (original, _Color4.rgb);
	   			fixed dist5 = distance (original, _Color5.rgb);
	   			
	   			fixed4 col = fixed4 (0,0,0,0);
	   			fixed dist = 10.0;

				if (dist0 < dist)
				{
					dist = dist0;
					col = _Color0;
				}

				if (dist1 < dist)
				{
					dist = dist1;
					col = _Color1;
				}
				
				if (dist2 < dist)
				{
					dist = dist2;
					col = _Color2;
				}
				
				if (dist3 < dist)
				{
					dist = dist3;
					col = _Color3;
				}

				if (dist4 < dist)
				{
					dist = dist4;
					col = _Color4;
				}

				if (dist5 < dist)
				{
					dist = dist5;
					col = _Color5;
				}

				return col;
	  		}
	  		
	  		ENDCG
	 	}
	}
	
	FallBack "Diffuse"
}
