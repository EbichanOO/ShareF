Shader "Hidden/PlaybackOccluder"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
            sampler2D_float _CameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 tex_coord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 tex_coord : TEXCOORD0;
                float linear_depth : TEXCOORD1;
                float4 screen_pos : TEXCOORD2;
			};

			v2f vert(const appdata input)
			{
				v2f output;
				float4 pos = UnityObjectToClipPos(input.vertex);
				output.pos = pos;
                output.tex_coord = input.tex_coord;
                output.screen_pos = ComputeScreenPos(output.pos);
                output.linear_depth = -(pos.z * _ProjectionParams.w);
                return output;
			}

			float4 frag(v2f i) : COLOR
			{
                float4 output = float4(0, 0, 0, 1);
                const float2 uv = i.screen_pos.xy / i.screen_pos.w;
				
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth (depth);

                const float diff = saturate(i.linear_depth - depth);
                if(diff < 0.001)
                    output = float4(1, 0, 0, 1);
				
                return output;
			}

			ENDCG
		}
    }
}