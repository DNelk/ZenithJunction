// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Glow"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (1,1,1,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		_Size("Size", Range( 0 , 16)) = 4
		_FadePower("Fade Power", Range( 0 , 8)) = 4
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		LOD 200
		Cull Front
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 viewDir;
			float3 worldNormal;
		};

		uniform half _Size;
		uniform float4 _GlowColor;
		uniform half _FadePower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz += ( ase_vertex3Pos * ( _Size / 10 ) );
			float3 ase_vertexNormal = v.normal.xyz;
			v.normal = ( ase_vertexNormal * -1 );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _GlowColor.rgb;
			float3 normalizeResult25 = normalize( i.viewDir );
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float dotResult23 = dot( normalizeResult25 , ase_vertexNormal );
			float lerpResult31 = lerp( 0.0 , 1.0 , pow( saturate( dotResult23 ) , _FadePower ));
			o.Alpha = lerpResult31;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
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
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
171;97;1440;754;1101.175;1128.609;1.626678;True;False
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;24;-498.079,-1058.856;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;27;-494.2719,-819.0464;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;25;-319.1728,-1036.017;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;23;-220.2034,-849.4977;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;-64.13605,-845.6912;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;16;-561.4384,-216.073;Float;False;Constant;_10;10;4;0;Create;True;0;0;False;0;10;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-317.7758,-613.6374;Half;False;Property;_FadePower;Fade Power;2;0;Create;True;0;0;False;0;4;0;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-631.3212,-302.6282;Half;False;Property;_Size;Size;1;0;Create;True;0;0;False;0;4;0;0;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-256.3648,-303.7985;Float;False;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;18;139.9202,-358.1762;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;20;166.5657,-219.2383;Float;False;Constant;_1;-1;4;0;Create;True;0;0;False;0;-1;0;0;1;INT;0
Node;AmplifyShaderEditor.PowerNode;32;-2.538743,-692.4278;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;12;-610.8045,-441.7695;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-105.3038,-439.5412;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;1;-83.39456,-1059.431;Float;False;Property;_GlowColor;Glow Color;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;410.1829,-344.8728;Float;True;2;2;0;FLOAT3;0,0,0;False;1;INT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;31;176.4561,-818.9373;Float;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;26;829.4073,-736.6932;Float;False;0;True;Transparent;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;554.0823,-951.3683;Float;False;True;2;Float;ASEMaterialInspector;200;0;Lambert;Glow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;True;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;1,1,1,0;VertexOffset;True;False;Cylindrical;False;Relative;200;Diffuse;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;24;0
WireConnection;23;0;25;0
WireConnection;23;1;27;0
WireConnection;22;0;23;0
WireConnection;15;0;6;0
WireConnection;15;1;16;0
WireConnection;32;0;22;0
WireConnection;32;1;7;0
WireConnection;13;0;12;0
WireConnection;13;1;15;0
WireConnection;21;0;18;0
WireConnection;21;1;20;0
WireConnection;31;2;32;0
WireConnection;0;2;1;0
WireConnection;0;9;31;0
WireConnection;0;11;13;0
WireConnection;0;12;21;0
ASEEND*/
//CHKSM=BFB78A1CC6A6A13E372F443A62552A583417881D