Shader "Unlit/ShieldShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Atmopshere Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend srcAlpha OneMinusSrcAlpha
        CULL Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex     : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            float3 _HitPosWS;

            int _HitsCount = 0;
            float _HitsRadius[10];
            float3 _HitsPosition[10];
            float _HitsIntensity[10];
            
            float DrawRing(float intensity, float radius, float dist)
            {
                float border = 0.08;
                float currentRadius = lerp(0, radius, 1 - intensity);//expand radius over time 
                return intensity * (1 - smoothstep(currentRadius, currentRadius + border, dist) - (1 - smoothstep(currentRadius - border, currentRadius, dist)));
            }

            void CalculateHitsFactor_float(float3 objectPosition, out float factor)
            {
                factor = 0;
                for (int i = 0; i < _HitsCount; i++)
                {
                    float distanceToHit = distance(objectPosition, _HitsPosition[i]);
                    factor += DrawRing(_HitsIntensity[i], 0.6, distanceToHit);
                }
                factor = saturate(factor);
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                o.positionWS = vertexInputs.positionWS;
                o.positionOS = v.vertex.xyz;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float3 col = tex2D(_MainTex, i.uv);
                float mask; //1 - saturate((distance(i.positionWS, _HitPosWS) - 0.25) / (1 - 0.1));
                


                CalculateHitsFactor_float(i.positionOS, mask);
                return float4(col, mask)*_Color;
            }
            ENDHLSL
        }
    }
}
