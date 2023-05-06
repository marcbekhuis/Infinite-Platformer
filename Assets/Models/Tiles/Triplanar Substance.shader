// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Triplanar Substance"
{
	Properties
	{
		_BaseColor("Base Color", 2D) = "white" {}
		_RougnessScale("Rougness Scale", Range( 0 , 1)) = 1
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[NoScaleOffset]_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		[NoScaleOffset]_Height("Height", 2D) = "gray" {}
		_MidLevel("MidLevel", Range( 0 , 1)) = 0.5
		_OffsetScale("Offset Scale", Float) = 1
		[HideInInspector]_offset("3D Offset", Vector) = (0,0,0,0)
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
		uniform sampler2D _BaseColor;
		uniform float4 _BaseColor_ST;
		uniform float3 _offset;
		uniform float _MidLevel;
		uniform float _OffsetScale;
		uniform sampler2D _Normal;
		uniform float _NormalIntensity;
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
			float3 temp_output_28_0 = ( ase_worldPos + float3( _BaseColor_ST.zw ,  0.0 ) + _offset );
			float4 triplanar10 = TriplanarSampling10( _Height, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor_ST.xy, 1.0, 0 );
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
			float3 temp_output_28_0 = ( ase_worldPos + float3( _BaseColor_ST.zw ,  0.0 ) + _offset );
			float3 triplanar16 = TriplanarSampling16( _Normal, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor_ST.xy, _NormalIntensity, 0 );
			float3 tanTriplanarNormal16 = mul( ase_worldToTangent, triplanar16 );
			o.Normal = tanTriplanarNormal16;
			float4 triplanar3 = TriplanarSampling3( _BaseColor, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor_ST.xy, 1.0, 0 );
			o.Albedo = triplanar3.xyz;
			o.Smoothness = ( ( 1.0 - triplanar3.a ) * _RougnessScale );
			float4 triplanar15 = TriplanarSampling15( _AmbientOcclusion, temp_output_28_0, ase_worldNormal, 1.0, _BaseColor_ST.xy, 1.0, 0 );
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
236;73;1192;527;2076.471;256.2379;1.215273;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;6;-1379.738,-13.87401;Inherit;True;Property;_BaseColor;Base Color;0;0;Create;True;0;0;0;False;0;False;None;0333c8759b07a7d43a83a05f9287a735;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureTransformNode;26;-1024,0;Inherit;False;-1;False;1;0;SAMPLER2D;;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.Vector3Node;30;-1557.264,-13.7709;Inherit;False;Property;_offset;3D Offset;8;1;[HideInInspector];Create;False;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;27;-1392,-160;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1373.907,200.3708;Inherit;True;Property;_Height;Height;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;7667d5f868e7b354ebb75350c7e91003;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-1024,112;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;10;-689,192;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;0;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-688,-160;Inherit;False;Property;_MidLevel;MidLevel;6;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;3;-689,-1.61441;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-320,192;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-688,-240;Inherit;False;Property;_RougnessScale;Rougness Scale;1;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;20;-496,768;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;23;-288,0;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1376,386.0927;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;538f34e1f1f43ac4e9f4ea6cdbc5f3d2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;18;-688,-80;Inherit;False;Property;_OffsetScale;Offset Scale;7;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1376,768;Inherit;False;Property;_NormalIntensity;Normal Intensity;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;13;-1373.907,576;Inherit;True;Property;_Normal;Normal;2;2;[NoScaleOffset];[Normal];Create;True;1;Normal;0;0;False;0;False;None;ea826e7e8cff81f478f2e23bc397de20;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TriplanarNode;16;-688,576;Inherit;True;Spherical;World;True;Top Texture 3;_TopTexture3;white;0;None;Mid Texture 3;_MidTexture3;white;-1;None;Bot Texture 3;_BotTexture3;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-128,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;15;-688,384;Inherit;True;Spherical;World;False;Top Texture 2;_TopTexture2;white;0;None;Mid Texture 2;_MidTexture2;white;-1;None;Bot Texture 2;_BotTexture2;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-192,192;Inherit;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1,0;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Triplanar Substance;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;3;32;10;25;True;1;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;6;0
WireConnection;28;0;27;0
WireConnection;28;1;26;1
WireConnection;28;2;30;0
WireConnection;10;0;11;0
WireConnection;10;9;28;0
WireConnection;10;3;26;0
WireConnection;3;0;6;0
WireConnection;3;9;28;0
WireConnection;3;3;26;0
WireConnection;22;0;10;0
WireConnection;22;1;21;0
WireConnection;23;0;3;4
WireConnection;16;0;13;0
WireConnection;16;9;28;0
WireConnection;16;8;29;0
WireConnection;16;3;26;0
WireConnection;24;0;23;0
WireConnection;24;1;25;0
WireConnection;15;0;12;0
WireConnection;15;9;28;0
WireConnection;15;3;26;0
WireConnection;19;0;22;0
WireConnection;19;1;20;0
WireConnection;19;2;18;0
WireConnection;0;0;3;0
WireConnection;0;1;16;0
WireConnection;0;4;24;0
WireConnection;0;5;15;0
WireConnection;0;11;19;0
ASEEND*/
//CHKSM=390F7D246CF3D143A8472DE858A3CDCC04B7494A