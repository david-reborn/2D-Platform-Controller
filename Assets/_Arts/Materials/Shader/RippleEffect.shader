Shader "Unlit/RippleEffect"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _CenterX("Center X", float) = 300
        _CenterY("Center Y", float) = 250
        _Amount("Amount", float) = 25
        _WaveSpeed("Wave Speed", range(.50, 50)) = 20
        _WaveAmount("Wave Amount", range(0, 20)) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _CenterX;
            float _CenterY;
            float _Amount;
            float _WaveSpeed;
            float _WaveAmount;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 center = fixed2(_CenterX / _ScreenParams.x, _CenterY / _ScreenParams.y);
                fixed time = _Time.y * _WaveSpeed;
                fixed amt = _Amount / 1000;

                fixed2 uv = center.xy - i.uv;
                uv.x *= _ScreenParams.x / _ScreenParams.y;

                fixed dist = sqrt(dot(uv,uv));
                fixed ang = dist * _WaveAmount - time;
                uv = i.uv + normalize(uv) * sin(ang) * amt;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
