Shader "Custom/Dissolve_EdgeGlow"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Dissolve ("Dissolve Amount", Range(0, 1)) = 0
        _EdgeWidth ("Edge Width", Range(0.01, 0.2)) = 0.05
        _EdgeGlowColor ("Edge Glow Color", Color) = (0.2, 0.8, 1, 1) // 蓝色光
        _EdgeGlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alphatest:_Cutoff

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        float _Dissolve;
        float _EdgeWidth;
        float4 _EdgeGlowColor;
        float _EdgeGlowIntensity;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
            float noise = tex2D(_NoiseTex, IN.uv_MainTex).r;

            float edge = smoothstep(_Dissolve, _Dissolve + _EdgeWidth, noise);
            float alphaMask = step(noise, _Dissolve);

            float finalAlpha = alphaMask * edge;

            float glow = (1.0 - edge) * alphaMask;
            float3 emission = _EdgeGlowColor.rgb * glow * _EdgeGlowIntensity;

            o.Albedo = mainCol.rgb;
            o.Emission = emission;
            o.Alpha = finalAlpha;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
