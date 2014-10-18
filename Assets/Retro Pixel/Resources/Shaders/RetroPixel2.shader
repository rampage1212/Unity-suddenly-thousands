Shader "AlpacaSound/RetroPixel2"
{
	Properties
	{
		_Color0 ("Color 0", Color) = (0.00, 0.00, 0.00, 1.0)	
		_Color1 ("Color 1", Color) = (0.14, 0.14, 0.14, 1.0)	
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
	  		uniform sampler2D _MainTex;
	    
	  		fixed4 frag (v2f_img i) : COLOR
	  		{
	   			fixed3 original = tex2D (_MainTex, i.uv).rgb;

	   			fixed dist0 = distance (original, _Color0.rgb);
	   			fixed dist1 = distance (original, _Color1.rgb);
	   			
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
				
				return col;
	  		}
	  		
	  		ENDCG
	 	}
	}
	
	FallBack "Diffuse"
}
