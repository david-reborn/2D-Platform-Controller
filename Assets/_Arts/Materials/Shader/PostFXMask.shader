// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Myd/PostFXMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortTex("Distortion Texture", 2D) = "white" {}
        _DistortIntensity("Distort Intensity", float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float4 screenPosition : TEXCOORD1;
                float2 screenuv : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float4 screenPosition = ComputeScreenPos(o.vertex);

                screenPosition = UnityPixelSnap(screenPosition);
                o.screenuv = (screenPosition.xy / screenPosition.w);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _DistortTex;
            sampler2D _GlobalRenderTexture;

            float _DistortIntensity;
            fixed4 frag(v2f i) : SV_Target
            {
                float4 uvDistort = tex2D(_DistortTex, i.uv);
                fixed4 col = tex2D(_GlobalRenderTexture, i.screenuv + fixed2((uvDistort.r) * 0.001 * _DistortIntensity, (uvDistort.g) * 0.001 * _DistortIntensity));
                fixed4 col2 = fixed4(_DistortIntensity, _DistortIntensity, _DistortIntensity, 1);
                return col;
            }
            ENDCG
        }
    }
}
