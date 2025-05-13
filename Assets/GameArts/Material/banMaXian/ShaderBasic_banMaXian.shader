Shader "Custom/UI/banMaXian"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_Color0("Color 0", Color) = (1,1,1,0)
		_Color1("Color 1", Color) = (1,0.25,0.25,0)
		_FlowSpeed_X("FlowSpeed_X", Float) = -0.1
		_FlowSpeed_Y("FlowSpeed_Y", Float) = 0
		_flowTextureSample2("flow Texture Sample 2", 2D) = "white" {}
		_maskTextureSample0("mask Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR


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
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform sampler2D _maskTextureSample0;
			uniform float4 _maskTextureSample0_ST;
			uniform sampler2D _flowTextureSample2;
			uniform float _FlowSpeed_X;
			uniform float _FlowSpeed_Y;
			uniform float4 _Color0;
			uniform float4 _Color1;
			uniform float4 _MainTex_ST;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_maskTextureSample0 = IN.texcoord.xy * _maskTextureSample0_ST.xy + _maskTextureSample0_ST.zw;
				float4 tex2DNode63 = tex2D( _maskTextureSample0, uv_maskTextureSample0 );
				float4 appendResult70 = (float4(_FlowSpeed_X , _FlowSpeed_Y , 0.0 , 0.0));
				float2 texCoord41 = IN.texcoord.xy * float2( 0.5,0.5 ) + float2( 0,0 );
				float2 panner40 = ( 1.0 * _Time.y * appendResult70.xy + texCoord41);
				float4 tex2DNode42 = tex2D( _flowTextureSample2, panner40 );
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 temp_output_20_0 = ( ( tex2D( _MainTex, uv_MainTex ) + _EnableExternalAlpha ) * IN.color );
				float4 appendResult25 = (float4(( ( ( tex2DNode63.r * tex2DNode42 * _Color0 ) + ( ( 1.0 - tex2DNode42 ) * _Color1 * tex2DNode63.r ) ) + float4( (temp_output_20_0).rgb , 0.0 ) ).rgb , (temp_output_20_0).a));
				
				fixed4 c = appendResult25;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}