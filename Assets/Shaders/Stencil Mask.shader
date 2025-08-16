Shader "Custom/StencilMask"
{
    Properties
    {
        _StencilRef ("Stencil Reference", Range(0, 255)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry-100"
        }
        
        // Make the mask invisible but write to stencil
        ColorMask 0
        ZWrite Off
        Cull Off  // Important for 3D volumes
        
        Stencil
        {
            Ref [_StencilRef]
            Comp Always
            Pass Replace
        }
        
        Pass
        {
            Name "FOV3DMask"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
