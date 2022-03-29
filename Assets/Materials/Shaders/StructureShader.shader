Shader "Unlit/StructureShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Bump1Map ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _Bump1Map;
            //float4 _BumpMap_ST;
            float4 _StructurePosWS;

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
                float4 col = float4(0.4,0.4,0.4,1);//tex2D(_MainTex, i.uv);

                float3 normal = UnpackNormal(tex2D(_Bump1Map, i.uv)); 
                
                float3x3 tangent2Object =
                {
                    i.tangentWS.xyz*i.tangentWS.w,
                    cross(i.tangentWS*i.tangentWS.w, i.normalWS),
                    i.normalWS
                };
                tangent2Object=transpose(tangent2Object);
                normal = mul(tangent2Object, normal);
                float3 worldNormal = normalize(normal);
                //float3 combinedNormals = float3(i.normalWS.x+normalMapWS.x, i.normalWS.y+normalMapWS.y, i.normalWS.z);
                
                col *= max(0.075,dot(worldNormal, -normalize(i.positionWS)))*1.5;
                return col;
            }
            ENDHLSL
        }
    }
}
