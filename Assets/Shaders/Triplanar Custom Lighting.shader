// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Triplanar Custom Lighting Model"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Height("Height", 2D) = "gray" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 1
		_Smoothness("Smoothness", Range( 0.01 , 1)) = 0.5
		_Heightmidlevel("Height mid-level", Float) = 0
		_HeightScale("Height Scale", Float) = 0
		_offset("Offset", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		sampler2D _Height;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float3 _offset;
		uniform float _Heightmidlevel;
		uniform float _HeightScale;
		sampler2D _Normal;
		uniform float _NormalScale;
		uniform float _Smoothness;


		inline float4 TriplanarSampling251( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float3 TriplanarSampling241( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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


		inline float4 TriplanarSampling240( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
			float3 temp_output_259_0 = ( ase_worldPos + _offset );
			float4 triplanar251 = TriplanarSampling251( _Height, temp_output_259_0, ase_worldNormal, 1.0, _Albedo_ST.xy, 1.0, 0 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ( triplanar251.x - _Heightmidlevel ) * ase_vertexNormal * _HeightScale );
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColor215 = ( ase_lightAtten * ase_lightColor.rgb );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 temp_output_259_0 = ( ase_worldPos + _offset );
			float3 triplanar241 = TriplanarSampling241( _Normal, temp_output_259_0, ase_worldNormal, 1.0, _Albedo_ST.xy, _NormalScale, 0 );
			float3 normal101 = triplanar241;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult66 = dot( normal101 , ase_worldlightDir );
			UnityGI gi79 = gi;
			float3 diffNorm79 = normal101;
			gi79 = UnityGI_Base( data, 1, diffNorm79 );
			float3 indirectDiffuse79 = gi79.indirect.diffuse + diffNorm79 * 0.0001;
			float4 triplanar240 = TriplanarSampling240( _Albedo, temp_output_259_0, ase_worldNormal, 1.0, _Albedo_ST.xy, 1.0, 0 );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult201 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult91 = dot( normalizeResult201 , normal101 );
			float smoothness218 = _Smoothness;
			float3 directSpecular213 = ( LightColor215 * pow( max( dotResult91 , 0.0 ) , exp2( (smoothness218*10.0 + 1.0) ) ) );
			float3 indirectNormal81 = normal101;
			Unity_GlossyEnvironmentData g81 = UnityGlossyEnvironmentSetup( smoothness218, data.worldViewDir, indirectNormal81, float3(0,0,0));
			float3 indirectSpecular81 = UnityGI_IndirectSpecular( data, 1.0, indirectNormal81, g81 );
			float fresnelNdotV88 = dot( normal101, ase_worldViewDir );
			float fresnelNode88 = ( 0.04 + smoothness218 * pow( 1.0 - fresnelNdotV88, 5.0 ) );
			float3 lerpResult89 = lerp( ( ( ( LightColor215 * max( dotResult66 , 0.0 ) ) + indirectDiffuse79 ) * (triplanar240).xyz ) , ( directSpecular213 + indirectSpecular81 ) , saturate( fresnelNode88 ));
			c.rgb = lerpResult89;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
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
236;73;1192;527;2950.706;-545.2919;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;245;-1232,688;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;None;e32e44524ae673c4f9f75c14592cfd07;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector3Node;261;-2336,816;Inherit;False;Property;_offset;Offset;8;0;Create;False;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;258;-2339.815,668.7952;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;259;-2022.749,690.6423;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-2912,80;Inherit;False;Property;_NormalScale;Normal Scale;3;0;Create;True;0;0;0;False;0;False;1;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureTransformNode;250;-1232,896;Inherit;False;-1;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;199;-1728,-1024;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;202;-1776,-864;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TriplanarNode;241;-2640,78.51177;Inherit;True;Spherical;World;True;Normal;_Normal;bump;2;Assets/Models/Panda/Panda.fbm/Body_Normal_OpenGL.png;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;World;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-2368,-128;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0.5;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-2048,80;Inherit;True;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;218;-2080,-128;Inherit;False;smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;200;-1472,-960;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;102;-1312,-848;Inherit;False;101;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;201;-1312,-960;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;76;-2446.992,457.916;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;221;-1248,-640;Inherit;False;218;smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;74;-2490.992,378.916;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;91;-1056,-896;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;206;-1024,-640;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;10;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2222.992,425.916;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;67;-1360,240;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMaxOpNode;93;-832,-896;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Exp2OpNode;204;-800,-640;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;215;-2080,432;Inherit;False;LightColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-1344,144;Inherit;False;101;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;203;-640,-800;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;217;-784,-1024;Inherit;False;215;LightColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;66;-1120,176;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-448,-1040;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;70;-880,176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;-913.2597,64;Inherit;False;215;LightColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-896,304;Inherit;False;101;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;240;-720,718.4993;Inherit;True;Spherical;World;False;_;_;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-672,160;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;213;-272,-1040;Inherit;False;directSpecular;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;79;-688,272;Inherit;True;World;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-544,-144;Inherit;False;101;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;219;-545.1395,-267.0066;Inherit;False;218;smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;220;-544,16;Inherit;False;218;smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;257;112,880;Inherit;False;Property;_Heightmidlevel;Height mid-level;6;0;Create;True;0;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;78;-416,160;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IndirectSpecularLight;81;-192,-272;Inherit;True;World;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;251;-720,928;Inherit;True;Spherical;World;False;Height;_Height;gray;1;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;214;-128,-432;Inherit;False;213;directSpecular;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;-304,256;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;88;-192,-48;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0.04;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;254;608,832;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;97;240,-336;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;159;112,-48;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;255;608,976;Inherit;False;Property;_HeightScale;Height Scale;7;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-94.46504,176.0001;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;256;112,768;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;226;480,512;Inherit;False;Property;_Keyword0;Keyword 0;4;0;Create;True;0;0;0;False;0;False;0;0;0;False;UNITY_PASS_FORWARDBASE;Toggle;2;Key0;Key1;Fetch;False;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;228;-122.3667,1990.419;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;236;95.06097,1668.899;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;157;639.4968,641.8533;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;150.3472,1831.263;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;401.6222,1642.752;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;233;-106.3667,1894.42;Inherit;False;101;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;597.306,1994.752;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomStandardSurface;237;179.1788,1384.999;Inherit;False;Metallic;World;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;89;512,176;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;153;851.7639,552.0026;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;160;256,464;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;234;411.6719,2191.975;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;848,688;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;230;117.6333,1926.419;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;235;160.55,2285.154;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;231;357.6333,1926.419;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;154;16,464;Inherit;False;Constant;_FogColor;Fog Color;6;0;Create;True;0;0;0;False;0;False;0.5849056,0.5849056,0.5849056,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;155;383.497,641.8533;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;156;112,640;Inherit;False;Property;_FogLengthOffset;Fog Length Offset;5;0;Create;True;0;0;0;False;0;False;2,5;2,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;232;200.855,2157.071;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;225;1150,323;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Triplanar Custom Lighting Model;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;259;0;258;0
WireConnection;259;1;261;0
WireConnection;250;0;245;0
WireConnection;241;9;259;0
WireConnection;241;8;80;0
WireConnection;241;3;250;0
WireConnection;101;0;241;0
WireConnection;218;0;82;0
WireConnection;200;0;199;0
WireConnection;200;1;202;0
WireConnection;201;0;200;0
WireConnection;91;0;201;0
WireConnection;91;1;102;0
WireConnection;206;0;221;0
WireConnection;77;0;74;0
WireConnection;77;1;76;1
WireConnection;93;0;91;0
WireConnection;204;0;206;0
WireConnection;215;0;77;0
WireConnection;203;0;93;0
WireConnection;203;1;204;0
WireConnection;66;0;105;0
WireConnection;66;1;67;0
WireConnection;207;0;217;0
WireConnection;207;1;203;0
WireConnection;70;0;66;0
WireConnection;240;0;245;0
WireConnection;240;9;259;0
WireConnection;240;3;250;0
WireConnection;75;0;216;0
WireConnection;75;1;70;0
WireConnection;213;0;207;0
WireConnection;79;0;104;0
WireConnection;78;0;75;0
WireConnection;78;1;79;0
WireConnection;81;0;103;0
WireConnection;81;1;219;0
WireConnection;251;9;259;0
WireConnection;251;3;250;0
WireConnection;131;0;240;0
WireConnection;88;0;103;0
WireConnection;88;2;220;0
WireConnection;97;0;214;0
WireConnection;97;1;81;0
WireConnection;159;0;88;0
WireConnection;73;0;78;0
WireConnection;73;1;131;0
WireConnection;256;0;251;1
WireConnection;256;1;257;0
WireConnection;226;0;160;0
WireConnection;157;0;155;0
WireConnection;227;0;231;0
WireConnection;227;1;234;0
WireConnection;89;0;73;0
WireConnection;89;1;97;0
WireConnection;89;2;159;0
WireConnection;153;0;89;0
WireConnection;153;1;226;0
WireConnection;153;2;157;0
WireConnection;160;0;154;0
WireConnection;234;0;232;1
WireConnection;234;1;235;0
WireConnection;252;0;256;0
WireConnection;252;1;254;0
WireConnection;252;2;255;0
WireConnection;230;0;233;0
WireConnection;230;1;228;0
WireConnection;231;0;230;0
WireConnection;155;0;156;1
WireConnection;155;1;156;2
WireConnection;225;13;89;0
WireConnection;225;11;252;0
ASEEND*/
//CHKSM=3720953D255DB36AFB5EDB27CF8B4208DE2BCA3E