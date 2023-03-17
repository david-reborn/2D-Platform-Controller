Shader "Myd/DistortShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortTex("Distortion Texture", 2D) = "white" {} //109
        _DistortAmount("Distortion Amount", Range(0,2)) = 0.5 //110
        _DistortTexXSpeed("Scroll speed X", Range(-50,50)) = 5 //111
        _DistortTexYSpeed("Scroll speed Y", Range(-50,50)) = 5 //112
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DistortTex;
            half4 _DistortTex_ST;
            half _DistortTexXSpeed, _DistortTexYSpeed, _DistortAmount;
            half _Alpha, _RandomSeed;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half2 uvDistTex : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvDistTex = TRANSFORM_TEX(v.uv, _DistortTex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            { 
                //i.uvDistTex.x += ((_Time + _RandomSeed) * _DistortTexXSpeed) % 1;
                //i.uvDistTex.y += ((_Time + _RandomSeed) * _DistortTexYSpeed) % 1;
                half distortAmnt = (tex2D(_DistortTex, i.uvDistTex).r - 0.5) * 0.2 * _DistortAmount;
                i.uv.x += distortAmnt;
                i.uv.y += distortAmnt;
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
