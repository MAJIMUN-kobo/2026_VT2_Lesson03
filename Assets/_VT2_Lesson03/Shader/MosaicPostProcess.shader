Shader "Custom/Mosaic"
{
    Properties
    {
        _BlockSize ("Block Size (pixels)", Range(1, 128)) = 16
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        ZWrite Off
        ZTest Always
        Cull Off
        Blend Off

        Pass
        {
            Name "Mosaic"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // Vert(フルスクリーン三角形)、Varyings、_BlitTexture が定義されている
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _BlockSize;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // 入力テクスチャの解像度(ピクセル)
                float2 resolution = _BlitTexture_TexelSize.zw;

                // UV → ピクセル座標
                float2 pixelPos = input.texcoord * resolution;

                // ブロック単位に量子化し、ブロックの中心座標を求める
                float blockSize = max(_BlockSize, 1.0);
                float2 blockCenter = (floor(pixelPos / blockSize) + 0.5) * blockSize;

                // ピクセル座標 → UV に戻してサンプリング(ポイントサンプリング)
                float2 uv = blockCenter / resolution;

                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_PointClamp, uv, 0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
