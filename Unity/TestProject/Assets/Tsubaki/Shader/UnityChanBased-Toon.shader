// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ulien8/UnityChanBased-Toon" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
		_EdgeThickness ("Outline Thickness", Range(0,5)) = 1
		_MainTex ("Diffuse", 2D) = "white" {}
		_CelTex ("Cel Texture", 2D) = "white" {}
		_Falloff ("Falloff", 2D) = "white" {}
		_FalloffPower ("Falloff Power", Range(0,1)) = 0.3
		_Specular ("Specular", 2D) = "white" {}
		_SpecularPower ("Specular Power", Range(0,5)) = 1
		_RimLight ("Rim Light", 2D) = "white" {}
	}
	SubShader {
		Tags { 
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode"="ForwardBase"
		}
				
		Pass {
			Cull Back
			ZTest LEqual
		
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			#define ENABLE_CAST_SHADOWS
			
			float4 _Color;
			float4 _ShadowColor;
			float _EdgeThickness;
			float _FalloffPower;
			float _SpecularPower;
			float4 _LightColor0;
			sampler2D _MainTex;
			sampler2D _CelTex;
			sampler2D _Falloff;
			sampler2D _Specular;
			sampler2D _RimLight;
			
			#ifdef ENABLE_CAST_SHADOWS
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
			#else
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
			};
			#endif

			v2f vert( appdata_tan v ) {
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize( mul( unity_ObjectToWorld, half4( v.normal, 0 ) ).xyz );
				
				half4 worldPos = mul( unity_ObjectToWorld, v.vertex );
				o.viewDir = normalize( _WorldSpaceCameraPos.xyz - worldPos.xyz);
				o.lightDir = WorldSpaceLightDir( v.vertex );
				
				#ifdef ENABLE_CAST_SHADOWS
				TRANSFER_VERTEX_TO_FRAGMENT( o );
				#endif
				
				return o;
			}
			
			float4 frag(v2f i) : COLOR {
				half4 tex = tex2D( _MainTex, i.uv );
				half3 normalVec = i.normal;
				
				//Falloff
				half normalDotView = dot( normalVec, i.viewDir );
				half falloffU = clamp( 1.0f - abs(normalDotView), 0.02f, 0.98f );
				half4 falloffTexColor = _FalloffPower * tex2D( _Falloff, float2( falloffU, 0.25f) );
				half3 shadowColor = tex.rgb * tex.rgb;
				half3 combinedColor = lerp( tex.rgb, shadowColor, falloffTexColor.r );
				combinedColor *= 1.0f + falloffTexColor.rgb * falloffTexColor.a;
				
				//Specular
				half4 reflectionMaskColor = tex2D( _Specular, i.uv.xy );
				half specularDot = dot( normalVec, i.viewDir );
				half4 lighting = lit( normalDotView, specularDot, _SpecularPower );
				half3 specularColor = saturate( lighting.z ) * reflectionMaskColor.rgb * tex.rgb;
				combinedColor += specularColor;
				
				combinedColor *= _Color.rgb * _LightColor0.rgb;
				float opacity = tex.a * _Color.a * _LightColor0.a;
				
				#ifdef ENABLE_CAST_SHADOWS
					// Cast shadows
					shadowColor = _ShadowColor.rgb * combinedColor;
					half cel = dot( normalVec, i.lightDir ) * 0.5 + 0.5;
					half4 celTex = tex2D( _CelTex, half2(cel, 0.25) );
					celTex.rgb = min(celTex.rgb + shadowColor, half3(1.0,1.0,1.0));
					half attenuation = saturate( 2.0 * LIGHT_ATTENUATION( i ) - 1.0 );
					//combinedColor = lerp( shadowColor, combinedColor, attenuation );
					combinedColor *= celTex.rgb;
				#endif
				
				// Rimlight
				half rimlightDot = saturate( 0.5 * ( dot( normalVec, i.lightDir ) + 1.0 ) );
				falloffU = saturate( rimlightDot * falloffU );
				falloffU = tex2D( _RimLight, float2( falloffU, 0.25f ) ).r;
				half3 lightColor = tex.rgb;
				combinedColor += falloffU * lightColor;
	
				return float4(combinedColor, opacity);
			}
			ENDCG
		}
		
		Pass {
			Cull Front
			ZTest Less
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			float _EdgeThickness;
			float4 _EdgeColor;
			float4 _LightColor0;
			sampler2D _MainTex;
			
			// Outline thickness multiplier
			#define INV_EDGE_THICKNESS_DIVISOR 0.00285
			// Outline color parameters
			#define SATURATION_FACTOR 0.6
			#define BRIGHTNESS_FACTOR 0.8
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert( appdata_tan v ) {
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				half4 normal = normalize( UnityObjectToClipPos( half4( v.normal, 0 ) ) );
				half4 scaledNormal = _EdgeThickness * INV_EDGE_THICKNESS_DIVISOR * normal;
				scaledNormal.z += 0.00001;
				o.pos += scaledNormal;
				
				return o;
			}
			
			float4 frag ( v2f i ) : COLOR {
				half4 diffuseMapColor = tex2D( _MainTex, i.uv );

				half maxChan = max( max( diffuseMapColor.r, diffuseMapColor.g ), diffuseMapColor.b );
				half4 newMapColor = diffuseMapColor;

				maxChan -= ( 1.0 / 255.0 );
				half3 lerpVals = saturate( ( newMapColor.rgb - float3( maxChan, maxChan, maxChan ) ) * 255.0 );
				newMapColor.rgb = lerp( SATURATION_FACTOR * newMapColor.rgb, newMapColor.rgb, lerpVals );
				
				return float4( BRIGHTNESS_FACTOR * newMapColor.rgb * diffuseMapColor.rgb, diffuseMapColor.a ) * _EdgeColor * _LightColor0;
			}
			
			ENDCG
		}
	} 
	FallBack "Transparent/Cutout/Diffuse"
}
