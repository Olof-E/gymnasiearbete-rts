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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            float3 _HitPosWS;

            int _HitsCount = 0;
            float _HitsRadius[10];
            float3 _HitsObjectPosition[10];
            float _HitsIntensity[10];
            
            float DrawRing(float intensity, float radius, float dist)
            {
                float border = 0.25;
                float currentRadius = lerp(0, radius, 1 - intensity);//expand radius over time 
                return intensity * (1 - smoothstep(currentRadius, currentRadius + border, dist) - (1 - smoothstep(currentRadius - border, currentRadius, dist)));
            }

            void CalculateHitsFactor_float(float3 objectPosition, out float factor)
            {
                factor = 0;
                for (int i = 0; i < _HitsCount; i++)
                {
                    float distanceToHit = distance(objectPosition, _HitsObjectPosition[i]);
                    factor += DrawRing(_HitsIntensity[i], _HitsRadius[i], distanceToHit);
                }
                factor = saturate(factor);
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                o.positionWS = vertexInputs.positionWS;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float3 col = tex2D(_MainTex, i.uv);
                float mask; //1 - saturate((distance(i.positionWS, _HitPosWS) - 0.25) / (1 - 0.1));
                
                _HitsCount = 2;
                _HitsRadius[0] = 2;
                _HitsObjectPosition[0] = float3(0,0, 2.14);
                _HitsIntensity[0] = (1-(sin(_Time.y*2)*0.5+0.5));
                _HitsRadius[1] = 2;
                _HitsObjectPosition[1] = float3(0,0, -2.14);
                _HitsIntensity[1] = (1-(sin(_Time.y*1)*0.5+0.5));

                CalculateHitsFactor_float(i.positionWS, mask);
                return float4(col, mask)*_Color;
            }
            ENDHLSL
        }
    }
}
