// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Triplanar Debug"
{
	Properties
	{
		_BaseColor1("Base Color 1", 2D) = "white" {}
		[NoScaleOffset]_BaseColor2("Base Color 2", 2D) = "white" {}
		_RougnessScale("Rougness Scale", Range( 0 , 1)) = 1
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[NoScaleOffset]_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		[NoScaleOffset]_Height("Height", 2D) = "gray" {}
		_MidLevel("MidLevel", Range( 0 , 1)) = 0.5
		_OffsetScale("Offset Scale", Float) = 1
		_WeightStrength("Weight Strength", Float) = 1
		_Falloff("Falloff", Float) = 7.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Height;
		uniform sampler2D _BaseColor1;
		uniform float4 _BaseColor1_ST;
		uniform float _MidLevel;
		uniform float _OffsetScale;
		uniform sampler2D _Normal;
		uniform float _NormalIntensity;
		uniform float _WeightStrength;
		uniform float _Falloff;
		uniform sampler2D _BaseColor2;
		uniform float _RougnessScale;
		uniform sampler2D _AmbientOcclusion;


		inline float4 TriplanarSampling10( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.zy * float2(  nsign.x, 1.0 ), 0, 0) );
			yNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.xz * float2(  nsign.y, 1.0 ), 0, 0) );
			zNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.xy * float2( -nsign.z, 1.0 ), 0, 0) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float3 TriplanarSampling16( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			xNorm.xyz  = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2(  nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz  = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2(  nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz  = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSampling3( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		inline float4 TriplanarSampling15( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 temp_output_28_0 = ( ase_worldPos + float3( _BaseColor1_ST.zw ,  0.0 ) );
			float4 triplanar10 = TriplanarSampling10( _Height, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor1_ST.xy, 1.0, 0 );
			float4 temp_cast_1 = (_MidLevel).xxxx;
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( triplanar10 - temp_cast_1 ) * float4( ase_vertexNormal , 0.0 ) * _OffsetScale ).xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 temp_output_28_0 = ( ase_worldPos + float3( _BaseColor1_ST.zw ,  0.0 ) );
			float3 triplanar16 = TriplanarSampling16( _Normal, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor1_ST.xy, _NormalIntensity, 0 );
			float3 tanTriplanarNormal16 = mul( ase_worldToTangent, triplanar16 );
			o.Normal = tanTriplanarNormal16;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 break7_g20 = ( ( ase_worldPos * float3( _BaseColor1_ST.xy ,  0.0 ) ) + float3( _BaseColor1_ST.zw ,  0.0 ) );
			float2 appendResult10_g20 = (float2(break7_g20.x , break7_g20.z));
			float2 appendResult11_g20 = (float2(break7_g20.x , break7_g20.y));
			float2 appendResult8_g20 = (float2(break7_g20.y , break7_g20.z));
			float3 appendResult70 = (float3((tex2D( _Height, appendResult10_g20 )).r , (tex2D( _Height, appendResult11_g20 )).r , (tex2D( _Height, appendResult8_g20 )).r));
			float3 lerpResult16_g25 = lerp( float3( 1,1,1 ) , appendResult70 , _WeightStrength);
			float3 temp_cast_3 = (_Falloff).xxx;
			float3 temp_output_17_0_g25 = pow( ( abs( ase_vertexNormal ) * lerpResult16_g25 ) , temp_cast_3 );
			float3 break20_g25 = temp_output_17_0_g25;
			float3 break24_g25 = ( temp_output_17_0_g25 / ( break20_g25.x + break20_g25.y + break20_g25.z ) );
			float3 newWorldNormal6_g25 = (WorldNormalVector( i , ase_vertexNormal ));
			float3 break75_g25 = newWorldNormal6_g25;
			float3 break7_g26 = ( ( ase_worldPos * float3( _BaseColor1_ST.xy ,  0.0 ) ) + float3( _BaseColor1_ST.zw ,  0.0 ) );
			float2 appendResult10_g26 = (float2(break7_g26.x , break7_g26.z));
			float2 temp_output_94_9_g25 = appendResult10_g26;
			float2 appendResult11_g26 = (float2(break7_g26.x , break7_g26.y));
			float2 temp_output_94_12_g25 = appendResult11_g26;
			float2 appendResult8_g26 = (float2(break7_g26.y , break7_g26.z));
			float2 temp_output_94_0_g25 = appendResult8_g26;
			o.Albedo = ( ( break24_g25.x * ( break75_g25.x > 0.0 ? tex2D( _BaseColor2, temp_output_94_9_g25 ) : tex2D( _BaseColor2, temp_output_94_9_g25 ) ) ) + ( break24_g25.y * ( break75_g25.y > 0.0 ? tex2D( _BaseColor1, temp_output_94_12_g25 ) : tex2D( _BaseColor2, temp_output_94_12_g25 ) ) ) + ( break24_g25.z * ( break75_g25.z > 0.0 ? tex2D( _BaseColor2, temp_output_94_0_g25 ) : tex2D( _BaseColor2, temp_output_94_0_g25 ) ) ) ).rgb;
			float4 triplanar3 = TriplanarSampling3( _BaseColor1, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor1_ST.xy, 1.0, 0 );
			o.Smoothness = ( ( 1.0 - triplanar3.w ) * _RougnessScale );
			float4 triplanar15 = TriplanarSampling15( _AmbientOcclusion, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor1_ST.xy, 1.0, 0 );
			o.Occlusion = triplanar15.x;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
363;73;1101;611;1908.037;-611.2628;1.489006;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;6;-1374.484,-13.87401;Inherit;True;Property;_BaseColor1;Base Color 1;0;0;Create;True;0;0;0;False;0;False;None;e32e44524ae673c4f9f75c14592cfd07;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1373.907,200.3708;Inherit;True;Property;_Height;Height;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;7667d5f868e7b354ebb75350c7e91003;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureTransformNode;26;-1024,0;Inherit;False;-1;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.WorldPosInputsNode;27;-1392,-160;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-1024,112;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;80;-1376,864;Inherit;False;TriplanarMapper;-1;;20;1cfb65ae48b8000498d29bd30af5e2a9;3,21,1,20,1,19,1;5;5;FLOAT3;0,0,0;False;13;FLOAT2;1,1;False;16;FLOAT2;0,0;False;24;SAMPLER2D;;False;25;SAMPLERSTATE;;False;3;COLOR;9;COLOR;12;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;72;-1184,832;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;73;-1184,912;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;74;-1184,992;Inherit;False;FLOAT;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;3;-689,-1.61441;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-688,-160;Inherit;False;Property;_MidLevel;MidLevel;7;0;Create;True;0;0;0;False;0;False;0.5;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;10;-689,192;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;0;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;76;-1264,1072;Inherit;True;Property;_BaseColor2;Base Color 2;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;0333c8759b07a7d43a83a05f9287a735;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;13;-1373.907,576;Inherit;True;Property;_Normal;Normal;3;2;[NoScaleOffset];[Normal];Create;True;1;Normal;0;0;False;0;False;None;ea826e7e8cff81f478f2e23bc397de20;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;77;-928,1136;Inherit;False;Property;_WeightStrength;Weight Strength;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1376,386.0927;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;538f34e1f1f43ac4e9f4ea6cdbc5f3d2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.OneMinusNode;23;-288,0;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-320,192;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-928,1216;Inherit;False;Property;_Falloff;Falloff;10;0;Create;True;0;0;0;False;0;False;7.5;60;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-688,-80;Inherit;False;Property;_OffsetScale;Offset Scale;8;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;20;-688,-304;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;70;-1027.995,867.9876;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-400,-160;Inherit;False;Property;_RougnessScale;Rougness Scale;2;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1376,768;Inherit;False;Property;_NormalIntensity;Normal Intensity;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;16;-688,576;Inherit;True;Spherical;World;True;Top Texture 3;_TopTexture3;white;0;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;15;-688,384;Inherit;True;Spherical;World;False;Top Texture 2;_TopTexture2;white;0;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-192,192;Inherit;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-128,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;83;-690.3737,871.6922;Inherit;False;WeightedTriplanarSampler;-1;;25;9be65e99d99333e44970195c4f5ec7cf;2,12,2,4,0;20;84;FLOAT3;0,0,0;False;38;COLOR;1,0,0,0;False;39;COLOR;0,1,0,0;False;40;COLOR;0,0,1,0;False;13;SAMPLER2D;;False;49;SAMPLER2D;;False;55;SAMPLER2D;;False;58;SAMPLER2D;;False;59;SAMPLER2D;;False;60;SAMPLER2D;;False;61;SAMPLER2D;;False;65;SAMPLER2D;;False;66;SAMPLER2D;;False;67;SAMPLER2D;;False;88;FLOAT2;0,0;False;89;FLOAT2;0,0;False;78;SAMPLERSTATE;;False;7;FLOAT3;1,1,1;False;8;FLOAT;1;False;18;FLOAT;7.5;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1,0;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Triplanar Debug;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;3;32;10;25;True;1;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;6;0
WireConnection;28;0;27;0
WireConnection;28;1;26;1
WireConnection;80;5;27;0
WireConnection;80;13;26;0
WireConnection;80;16;26;1
WireConnection;80;24;11;0
WireConnection;72;0;80;9
WireConnection;73;0;80;12
WireConnection;74;0;80;0
WireConnection;3;0;6;0
WireConnection;3;9;28;0
WireConnection;3;3;26;0
WireConnection;10;0;11;0
WireConnection;10;9;28;0
WireConnection;10;3;26;0
WireConnection;23;0;3;4
WireConnection;22;0;10;0
WireConnection;22;1;21;0
WireConnection;70;0;72;0
WireConnection;70;1;73;0
WireConnection;70;2;74;0
WireConnection;16;0;13;0
WireConnection;16;9;28;0
WireConnection;16;8;29;0
WireConnection;16;3;26;0
WireConnection;15;0;12;0
WireConnection;15;9;28;0
WireConnection;15;3;26;0
WireConnection;19;0;22;0
WireConnection;19;1;20;0
WireConnection;19;2;18;0
WireConnection;24;0;23;0
WireConnection;24;1;25;0
WireConnection;83;84;27;0
WireConnection;83;55;76;0
WireConnection;83;60;76;0
WireConnection;83;61;6;0
WireConnection;83;65;76;0
WireConnection;83;66;76;0
WireConnection;83;67;76;0
WireConnection;83;88;26;0
WireConnection;83;89;26;1
WireConnection;83;7;70;0
WireConnection;83;8;77;0
WireConnection;83;18;78;0
WireConnection;0;0;83;0
WireConnection;0;1;16;0
WireConnection;0;4;24;0
WireConnection;0;5;15;0
WireConnection;0;11;19;0
ASEEND*/
//CHKSM=1823EA3333ACA7E0A2C716A3DC3E5EF09E4A01BC