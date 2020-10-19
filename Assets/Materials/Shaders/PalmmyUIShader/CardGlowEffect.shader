// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CardAuraEffectShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_Aura_Back("Aura_Back", 2D) = "white" {}
		_Aura("Aura", 2D) = "white" {}
		_AuraThin("AuraThin", 2D) = "white" {}
		_AuraThinBack("AuraThinBack", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform sampler2D _Aura;
			uniform float4 _Aura_ST;
			uniform sampler2D _Aura_Back;
			uniform float4 _Aura_Back_ST;
			uniform float4 Aura_Color;
			uniform sampler2D _AuraThin;
			uniform float4 _AuraThin_ST;
			uniform sampler2D _AuraThinBack;
			uniform float4 _AuraThinBack_ST;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_Aura = IN.texcoord.xy * _Aura_ST.xy + _Aura_ST.zw;
				float2 uv011 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime18 = _Time.y * 2.5;
				float cos16 = cos( ( mulTime18 * -1.0 ) );
				float sin16 = sin( ( mulTime18 * -1.0 ) );
				float2 rotator16 = mul( uv011 - float2( 0.5,0.5 ) , float2x2( cos16 , -sin16 , sin16 , cos16 )) + float2( 0.5,0.5 );
				float temp_output_15_0 = sin( ( rotator16.y * UNITY_PI ) );
				float4 color32 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float2 uv_Aura_Back = IN.texcoord.xy * _Aura_Back_ST.xy + _Aura_Back_ST.zw;
				float4 tex2DNode28 = tex2D( _Aura_Back, uv_Aura_Back );
				float2 uv_AuraThin = IN.texcoord.xy * _AuraThin_ST.xy + _AuraThin_ST.zw;
				float2 uv_AuraThinBack = IN.texcoord.xy * _AuraThinBack_ST.xy + _AuraThinBack_ST.zw;
				
				half4 color = ( ( ( ( tex2D( _Aura, uv_Aura ).a * temp_output_15_0 * 1.0 * temp_output_15_0 * color32 * temp_output_15_0 ) + ( tex2DNode28.a * temp_output_15_0 * 1.0 * temp_output_15_0 * Aura_Color * tex2DNode28.a ) ) * 1.25 ) + tex2D( _AuraThin, uv_AuraThin ).a + ( tex2D( _AuraThinBack, uv_AuraThinBack ).a * Aura_Color ) );
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
0;16.8;1536;827;1250.025;-403.0872;1.456681;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;18;-2541.054,669.2797;Inherit;False;1;0;FLOAT;2.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2532.554,757.8603;Inherit;False;Constant;_ClockWise;ClockWise;6;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2380.685,424.8784;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2253.554,697.8603;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;16;-2063.04,567.1217;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;17;-1860.676,574.1158;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.PiNode;13;-2364.824,842.9982;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1569.713,585.545;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;-1263.492,226.6381;Inherit;True;Property;_Aura;Aura;1;0;Create;True;0;0;False;0;-1;None;92b5e7c4952696e408b22211480a8f8c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-1255.934,852.7682;Inherit;True;Property;_Aura_Back;Aura_Back;0;0;Create;True;0;0;False;0;-1;None;92b5e7c4952696e408b22211480a8f8c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;32;-1218.433,53.32566;Inherit;False;Constant;_Color0;Color 0;3;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;15;-1346.745,591.1009;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1185.854,425.5455;Inherit;False;Constant;_Opacity;Opacity;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-1218.434,1048.811;Inherit;False;Global;Aura_Color;Aura_Color;2;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-831.7092,265.8752;Inherit;True;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-790.5869,825.495;Inherit;True;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;COLOR;0,0,0,0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-482.74,447.5469;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;37;-281.84,477.9816;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-447.6563,732.4547;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;1.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-489.9296,1282.221;Inherit;True;Property;_AuraThinBack;AuraThinBack;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;39;-439.5204,842.6301;Inherit;True;Property;_AuraThin;AuraThin;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-99.52962,1076.362;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-165.6371,450.6729;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;227.9549,581.2426;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;25;559.366,457.9734;Float;False;True;-1;2;ASEMaterialInspector;0;4;CardAuraEffectShader;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;26;0;18;0
WireConnection;26;1;27;0
WireConnection;16;0;11;0
WireConnection;16;2;26;0
WireConnection;17;0;16;0
WireConnection;12;0;17;1
WireConnection;12;1;13;0
WireConnection;15;0;12;0
WireConnection;14;0;21;4
WireConnection;14;1;15;0
WireConnection;14;2;19;0
WireConnection;14;3;15;0
WireConnection;14;4;32;0
WireConnection;14;5;15;0
WireConnection;30;0;28;4
WireConnection;30;1;15;0
WireConnection;30;2;19;0
WireConnection;30;3;15;0
WireConnection;30;4;29;0
WireConnection;30;5;28;4
WireConnection;34;0;14;0
WireConnection;34;1;30;0
WireConnection;37;0;34;0
WireConnection;41;0;40;4
WireConnection;41;1;29;0
WireConnection;36;0;37;0
WireConnection;36;1;35;0
WireConnection;38;0;36;0
WireConnection;38;1;39;4
WireConnection;38;2;41;0
WireConnection;25;0;38;0
ASEEND*/
//CHKSM=E281BC10C863747926E500BFC33FD33951DA3C94