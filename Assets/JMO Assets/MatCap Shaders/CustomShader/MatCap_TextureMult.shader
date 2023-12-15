// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// MatCap Shader, (c) 2015 Jean Moreno

Shader "MatCap/Vertex/Textured Multiply_custom"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_DiffBlend("Diffuse Amount", Range(0, 1)) = 1
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MatSat("MatCap Saturation", Range(0, 1)) = 1
		_Blend("MatCap Amount", Range(0, 1)) = 1
		_MatCap ("MatCap (RGB)", 2D) = "white" {}
		_Mult("Multiplier", Range(1, 3)) = 1.1
		_LightmapBlend("Lightmap Amount", Range(0, 1)) = 0
		[Toggle(USE_LIGHTMAPS)] _UseBacking("Use Lightmaps", Int) = 0
	}
	
	Subshader
	{
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			Lighting Off
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma shader_feature USE_LIGHTMAPS

				#include "UnityCG.cginc"

				struct app_vertext
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord0 : TEXCOORD0;
					#if USE_LIGHTMAPS
					float4 texcoord1 : TEXCOORD1;
					#endif
				};

				struct app_fragment
				{
					float4 pos	: SV_POSITION;
					float2 uv0 	: TEXCOORD0;
					float2 cap	: TEXCOORD1;
					#if USE_LIGHTMAPS
					float2 uv1  : TEXCOORD2;
					#endif
				};
				
				uniform float4 _MainTex_ST;
				uniform float4 _Color;
				uniform float _Mult;
				uniform float _DiffBlend;
				uniform float _LightmapBlend;
				uniform float _MatSat;
				uniform float _Blend;
								
				app_fragment vert (app_vertext v)
				{
					app_fragment o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
					half2 capCoord;
					
					float3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
					worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
					o.cap.xy = worldNorm.xy * 0.5 + 0.5;

					#if USE_LIGHTMAPS
					o.uv1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _MatCap;

				static const float3 greyScaleConversion = { 0.3f, 0.59f, 0.11f };

				fixed4 frag (app_fragment i) : COLOR
				{
					fixed4 tex = lerp(_Color, tex2D(_MainTex, i.uv0) * _Color, _DiffBlend);
					fixed4 mc = tex2D(_MatCap, i.cap);

					float3 greyscale = dot(mc.rgb, greyScaleConversion);
					float3 lerped = lerp(greyscale, mc.rgb, _MatSat);
					mc = float4(lerped, mc.a);

					tex *= lerp(1, mc, _Blend);

					#if USE_LIGHTMAPS 
					float attenuation = 2 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1);
					tex.rgb = lerp(tex.rgb * (2 * attenuation), 1 - (2 - 2 * attenuation) * (1 - tex.rgb), attenuation * _LightmapBlend);					
					#endif

					return tex * _Mult;
				}
			ENDCG
		}
	}
	
	Fallback "Diffuse"
}
