Shader "Unlit/StructureShader"
{
    Properties
    {
        //[PreRendererData] _PlanetPosWS ("planet pos", vector) = (0,0,0,0)
        [MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)
	
        [Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping", Float) = 0
        _Cutoff ("Alpha Cutoff", Float) = 0.5
        _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit"}
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ _CLUSTERED_RENDERING

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

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
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float4 tangentWS   : TEXCOORD3;
            };

            float4 _StructurePosWS;
            //sampler2D _BumpMap;

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

                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float4 col = float4(0.4,0.4,0.4,1);//tex2D(_MainTex, i.uv);

                float3 normal = UnpackNormal(tex2D(sampler_BumpMap, i.uv)); 
                
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

                    #ifdef _ADDITIONAL_LIGHTS
                        // Shade additional cone and point lights. Functions in URP/ShaderLibrary/Lighting.hlsl
                        uint numAdditionalLights = GetAdditionalLightsCount();
                        for (uint lightI = 0; lightI < numAdditionalLights; lightI++) {
                            Light light = GetAdditionalLight(lightI, i.positionWS, 1);
                            col += float4(light.color * light.distanceAttenuation * light.shadowAttenuation, 0);
                        }
                    #endif
                return col;
            }
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            ZTest LEqual
        
            HLSLPROGRAM

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5
        
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
}
