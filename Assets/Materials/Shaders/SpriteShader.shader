Shader "Unlit/SpriteShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTint ("Color tint", Color) = (1,1,1,1)
        _Intensity ("Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend srcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 vertex     : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // UNITY_INSTANCING_BUFFER_START(_Props)
            //     UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            // UNITY_INSTANCING_BUFFER_END(_Props)

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorTint;
            float _Intensity;

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);   
                
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 col = tex2D(_MainTex, i.uv);
                
                return col*_ColorTint*_Intensity*i.color;
            }
            ENDHLSL
        }
    }
}
