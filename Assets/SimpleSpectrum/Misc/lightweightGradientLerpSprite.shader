// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SimpleSpectrum/lightweightGradientLerpSprite"
{
    Properties
    {
		_Color1("Color1", Color) = (1, 1, 1)
		_Color2("Color2", Color) = (1, 1, 1)
		_Val("Gradient Value", Float) = 0.5
		_MainTex("Texture", 2D) = "white" {}
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
           
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                half2 uv  : TEXCOORD0;
            };
           
			float4 _Color1;
			float4 _Color2;

			float _Val;
			sampler2D _MainTex;

            v2f vert(appdata_t i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            }
 
            fixed4 frag(v2f i) : COLOR
            {
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * lerp(_Color1, _Color2, _Val);
            }
        ENDCG
        }
    }
    Fallback "Sprites/Default"
}