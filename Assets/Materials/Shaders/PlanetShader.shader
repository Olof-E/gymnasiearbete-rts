Shader "Unlit/PlanetShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _RoughnessMap ("Roughness Map", 2D) = "grey" {}
        _SpecularMap ("Specular Map", 2D) = "grey" {}
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

            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            sampler2D _RoughnessMap;
            float4 _RoughnessMap_ST;

            sampler2D _SpecularMap;
            float4 _SpecularMap_ST;

            float4 _PlanetPosWS;

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

                float4 col = tex2D(_MainTex, i.uv);
                float3 normal = UnpackNormal(tex2D(_NormalMap, i.uv)); 
                
                float3x3 tangent2Object =
                {
                    i.tangentWS.xyz*i.tangentWS.w,
                    cross(i.tangentWS*i.tangentWS.w, i.normalWS),
                    i.normalWS
                };
                   
                normal = -TransformTangentToWorld(normal, CreateTangentToWorld(i.normalWS, i.tangentWS,1));
                float3 worldNormal = normalize(normal);

                float specular = pow(
                    max(0, dot(
                        reflect(normalize(_PlanetPosWS), worldNormal), 
                        GetWorldSpaceNormalizeViewDir(i.positionWS))), 
                    7.5)*tex2D(_SpecularMap, i.uv)*1.2;

                


                col *= pow(max(0, dot(normalize(_PlanetPosWS), worldNormal)), 0.75);

                col +=  specular * (tex2D(_RoughnessMap, i.uv));

                col *= 1.5;

                return col;
            }
            ENDHLSL
        }
    }
}
