Shader "Custom/fireMountain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SecondTex ("Secondary Texture", 2D) = "black" {}
        _ExpansionCenter ("Expansion Center", Vector) = (0,0,0,0)
        _ExpansionRadius ("Expansion Radius", Float) = 5.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SecondTex;
        float4 _MainTex_ST;
        float4 _SecondTex_ST;

        float4 _ExpansionCenter;
        float _ExpansionRadius;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SecondTex;
            float3 worldPos; // 世界坐标
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 获取两张贴图颜色
            fixed4 col1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed4 col2 = tex2D(_SecondTex, IN.uv_SecondTex) * _Color;

            // 计算与扩散中心距离
            float dist = distance(IN.worldPos, _ExpansionCenter.xyz);
            float blend = saturate(1.0 - dist / _ExpansionRadius);

            // 线性混合贴图颜色
            fixed4 finalCol = lerp(col1, col2, blend);

            o.Albedo = finalCol.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = finalCol.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
