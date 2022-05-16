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

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 vertex   : POSITION;
                //float4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 vertex   : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                //fixed4 color    : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            }; 

            UNITY_INSTANCING_BUFFER_START(_Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(_Props)
            // struct MeshProperties {
            //     float4x4 mat;
            //     float4 color;
            // };
            
            //StructuredBuffer<MeshProperties> _Properties;
            
            Varyings vert(Attributes v) {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                //float4 pos = mul(_Properties[instanceID].mat, i.vertex);
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                //o.color = float4(0.75, 0.35, 0.05, 1);//_Properties[instanceID].color;

                return o;
            }

            float4 frag(Varyings i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i);
                //clip();
                return UNITY_ACCESS_INSTANCED_PROP(_Props, _Color)*4.75;
            }

            ENDHLSL
        }
    }
}
