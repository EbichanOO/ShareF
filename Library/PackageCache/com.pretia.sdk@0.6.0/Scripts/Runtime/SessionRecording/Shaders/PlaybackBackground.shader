Shader "Hidden/PlaybackBackground" 
 {
     Properties
     {
         _MainTex("Main Texture", 2D) = "white" {}
         _BackgroundTex("Background Texture", 2D) = "white" {}
         _Angle("Angle", float) = 0
     }
     
     SubShader{
 
         Tags{ "PreviewType" = "Plane" }
 
         Pass {
             
             ZWrite Off
             Lighting Off
             ColorMask RGBA
             CGPROGRAM
             
             #pragma vertex vert
             #pragma fragment frag
             #include "UnityCG.cginc"
 
             sampler2D _MainTex;
             sampler2D _BackgroundTex;
             sampler2D _OccluderTex;
             float _Angle;
             

             struct v2f {
                 float4 pos : SV_POSITION;
                 float2 uv : TEXCOORD0;
             };
             
             float2 Rotate (float4 vertex, float degrees)
             {
                 float2 uv = vertex.xy;
                 float alpha = -3.1416 * degrees / 180;
                 uv -=0.5;
                 float s = -sin ( alpha );
                 float c = cos ( alpha );
                 float2x2 rotationMatrix = float2x2( c, -s, s, c);
                 uv = mul ( uv, rotationMatrix );
                 uv += 0.5;
                 return uv;
             }

             v2f vert(const appdata_full input)
             {
                 v2f output;
                 
                 output.pos = UnityObjectToClipPos(input.vertex);
                 output.uv = Rotate(input.texcoord, _Angle);
                 return output;
             }

             float4 frag(const v2f i) : COLOR
             {
                 float4 color = tex2D(_MainTex, i.uv);
                 float4 background_color = tex2D(_BackgroundTex, i.uv);
                 float4 occluder = tex2D(_OccluderTex, i.uv);
                 const float4 mask = round(color.a) - round(occluder.r);
                 
                 color = mask * color;
                 background_color = (1-mask) * background_color;
                 
                 float4 output = color + background_color;
                 output.a = 1;

                 return output;
             }

             
             ENDCG
         }
     }
}