Shader "Hidden/PlaybackProjector" {
   
   Properties
   {
      _ShadowTex ("Projected Image", 2D) = "white" {}
   }
   SubShader {
      Pass {
         
         ZWrite Off
         Offset -1, -1

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag
         #pragma fragmentoption ARB_fog_exp2
         #pragma fragmentoption ARB_precision_hint_fastest
         #include "UnityCG.cginc"
 
         uniform sampler2D _ShadowTex; 
         uniform float4x4 unity_Projector;
         float4 _ShadowTex_ST;

         struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         
         struct v2f {
            float4 pos : SV_POSITION;
            float4 posProjector : TEXCOORD0;
            float2 uv : TEXCOORD1;
         };

 
         v2f vert(const appdata i) 
         {
            v2f output;
 
            output.posProjector = mul(unity_Projector, i.vertex);
            output.pos = UnityObjectToClipPos(i.vertex);
            output.uv = TRANSFORM_TEX (output.posProjector.xy, _ShadowTex);
            
            return output;
         }
 
 
         float4 frag(v2f i) : COLOR
         {
            const float depth = i.posProjector.w;
            const float u = i.uv.x;
            const float v = i.uv.y;
            
            if (depth > 0 && u <= depth && v <= depth && u >= 0 && v >= 0)
            {
               return tex2D(_ShadowTex , i.posProjector.xy / i.posProjector.w); 
            }
            return float4(0.0, 0.0, 0.0, 0.0);
         }
 
         ENDCG
      }
   }  
   Fallback "Projector/Light"
}