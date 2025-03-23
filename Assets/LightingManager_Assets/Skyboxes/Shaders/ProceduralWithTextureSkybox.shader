Shader "Custom/ProceduralWithTextureSkybox"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Cubemap ("Cubemap", Cube) = "" { }
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            samplerCUBE _Cubemap;
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Blend the cubemap and texture here, for example, based on some factor
                half4 texColor = tex2D(_MainTex, i.pos.xy); // Texture sampling
                half4 cubemapColor = texCUBE(_Cubemap, i.pos.xyz); // Cubemap sampling

                return lerp(cubemapColor, texColor, 0.5); // Blend both
            }
            ENDCG
        }
    }
    FallBack "Skybox/Procedural"
}
