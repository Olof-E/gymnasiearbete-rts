Shader "Unlit/StarShader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        [Hdr] _PrimaryColor ("Prim Col", Color) = (1,1,1,1)
        [Hdr] _SecondaryColor ("Sec Col", Color) = (1,1,1,1)
    }
    SubShader
    {        
        Tags { "RenderType"="Opaque" }
        ZTest LEqual
        LOD 100


        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            #pragma vertex vert
            #pragma fragment frag
 
            // New hash based on hash13() from "Hash without Sine" by Dave_Hoskins (https://www.shadertoy.com/view/4djSRW)
            float noise(in float4 p) {
                p = frac(p * 0.1031);
                p += dot(p, p.zwyx + 31.32);
                return frac((p.x + p.y) * p.z - p.x * p.w);
            }

            float snoise(in float4 p) {
                float4 cell = floor(p);
                float4 local = frac(p);
                local *= local * (3.0 - 2.0 * local);

                float ldbq = noise(cell);
                float rdbq = noise(cell + float4(1.0, 0.0, 0.0, 0.0));
                float ldfq = noise(cell + float4(0.0, 0.0, 1.0, 0.0));
                float rdfq = noise(cell + float4(1.0, 0.0, 1.0, 0.0));
                float lubq = noise(cell + float4(0.0, 1.0, 0.0, 0.0));
                float rubq = noise(cell + float4(1.0, 1.0, 0.0, 0.0));
                float lufq = noise(cell + float4(0.0, 1.0, 1.0, 0.0));
                float rufq = noise(cell + float4(1.0, 1.0, 1.0, 0.0));
                float ldbw = noise(cell + float4(0.0, 0.0, 0.0, 1.0));
                float rdbw = noise(cell + float4(1.0, 0.0, 0.0, 1.0));
                float ldfw = noise(cell + float4(0.0, 0.0, 1.0, 1.0));
                float rdfw = noise(cell + float4(1.0, 0.0, 1.0, 1.0));
                float lubw = noise(cell + float4(0.0, 1.0, 0.0, 1.0));
                float rubw = noise(cell + float4(1.0, 1.0, 0.0, 1.0));
                float lufw = noise(cell + float4(0.0, 1.0, 1.0, 1.0));
                float rufw = noise(cell + 1.0);

                return lerp(lerp(lerp(lerp(ldbq, rdbq, local.x),
                                lerp(lubq, rubq, local.x),
                                local.y),

                            lerp(lerp(ldfq, rdfq, local.x),
                                lerp(lufq, rufq, local.x),
                                local.y),

                            local.z),

                        lerp(lerp(lerp(ldbw, rdbw, local.x),
                                lerp(lubw, rubw, local.x),
                                local.y),

                            lerp(lerp(ldfw, rdfw, local.x),
                                lerp(lufw, rufw, local.x),
                                local.y),

                            local.z),

                        local.w);
            }

            float fnoise(float4 p, float scale, float octaves) {
                p *= scale;
                float nscale = 1.0;
                float tscale = 0.0;
                float value = 0.0;

                for (float octave=0.0; octave < octaves; octave++) {
                    value += snoise(p) * nscale;
                    tscale += nscale;
                    nscale *= 0.35;
                    p *= 3.5;
                }

                return value / tscale;
            }
 


            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {   
                float4 vertex : SV_POSITION;
                float3 position  : TEXCOORD1;
            };

            float4 _PrimaryColor;
            float4 _SecondaryColor;

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInputs = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInputs.positionCS;
                o.position = normalize(v.vertex.xyz);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float timeScale = 1000;
                //Calculate noise coords to warp the final noise
                float noiseCol1 = fnoise(float4(i.position, _Time.y/timeScale), 6, 3);
                float noiseCol2 = fnoise(float4(i.position+noiseCol1.xxx, _Time.y/timeScale), 64, 1);
                float noiseCol3 = fnoise(float4(i.position+float3(noiseCol1, noiseCol2, lerp(noiseCol1, noiseCol2, 0.5)), _Time.y/timeScale), 32, 1);
                //float warpedNoise = fnoise(float4((i.position + float3(noiseCol1,noiseCol2,noiseCol3)*15), _Time.y/timeScale), 3.75, 2.0);

                // Calculate final color
                return lerp(_SecondaryColor, _PrimaryColor, pow(noiseCol3*noiseCol1,1.85));//pow(lerp(noiseCol1,warpedNoise,0.525),3.25));
            }
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

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
        

    }
}
