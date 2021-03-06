Shader "Unlit/PlanetAtmosphereShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Atmopshere Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        ZWRITE On
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend OneMinusDstColor One
        LOD 100
        CULL BACK

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
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float4 vertex     : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv         : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float4 tangentWS   : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            float3 _PlanetPosWS;

            Varyings vert (Attributes v)
            {
                Varyings o;
                
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                o.positionWS = vertexInputs.positionWS;
                VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(v.normal, v.tangent);
                o.normalWS = vertexNormalInputs.normalWS;

                real sign = v.tangent.w * GetOddNegativeScale();
                o.tangentWS = float4(vertexNormalInputs.tangentWS.xyz, sign);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float4 col = _Color;//tex2D(_MainTex, i.uv);                

                float fresnel = pow(1-max(0,dot(i.normalWS, GetWorldSpaceNormalizeViewDir(i.positionWS))),0.8)*max(0,dot(i.normalWS, -normalize(_PlanetPosWS)));

                return col*fresnel;
            }
            ENDHLSL
        }
    }
}
