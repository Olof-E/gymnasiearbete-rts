Shader "Unlit/PhasedEnergyBeamShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Hdr] _Color("Star Color", Color) = (1,1,1,1)
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

            struct appdata_t {
                float4 vertex   : POSITION;
                //float4 color    : COLOR;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                //fixed4 color    : COLOR;
            }; 

            float4 _Color;
            // struct MeshProperties {
            //     float4x4 mat;
            //     float4 color;
            // };
            
            //StructuredBuffer<MeshProperties> _Properties;
            
            v2f vert(appdata_t i, uint instanceID: SV_InstanceID) {
                v2f o;
                //float4 pos = mul(_Properties[instanceID].mat, i.vertex);
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(i.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                //o.color = float4(0.75, 0.35, 0.05, 1);//_Properties[instanceID].color;

                return o;
            }

            float4 frag(v2f i) : SV_Target {
                //clip();
                return _Color*4.75;
            }

            ENDHLSL
        }
    }
}
