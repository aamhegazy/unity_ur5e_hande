Shader "Custom/HexagonalGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.2, 0.5, 1.0, 0.3)
        _GridScale ("Grid Scale", Float) = 10.0
        _LineWidth ("Line Width", Range(0.01, 0.5)) = 0.05
        _CenterY ("Center Y Position", Float) = 0.0
        _FadeWidth ("Fade Width", Range(0.01, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GridScale;
            float _LineWidth;
            float _CenterY;
            float _FadeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float hexDist(float2 p)
            {
                p = abs(p);
                float c = dot(p, normalize(float2(1, 1.73)));
                c = max(c, p.x);
                return c;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Only show upper hemisphere
                float hemisphereAlpha = smoothstep(_CenterY - _FadeWidth, _CenterY, i.worldPos.y);
                
                float2 grid = i.uv * _GridScale;
                
                // Hexagonal tiling
                float2 r = float2(1, 1.73);
                float2 h = r * 0.5;
                float2 a = fmod(grid, r) - h;
                float2 b = fmod(grid - h, r) - h;
                
                float2 gv = length(a) < length(b) ? a : b;
                
                float d = hexDist(gv);
                float edge = smoothstep(_LineWidth, _LineWidth * 0.5, abs(d - 0.5));
                
                float4 col = _Color;
                col.a *= edge * hemisphereAlpha;
                
                return col;
            }
            ENDCG
        }
    }
}