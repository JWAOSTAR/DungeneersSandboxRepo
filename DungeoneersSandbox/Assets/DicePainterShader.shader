Shader "DS/DicePainterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        ZTest Off
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXTCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            //float4 _MainTex_ST;

            float4 _Mouse;

            float4x4 _WorldMatrix;

            //Brash Values
            float4 _BrushColor;
            float _BrushOpacity;
            float _BrushHardness;
            float _BrushSize;

            v2f vert (appdata v)
            {
                v2f o;
                float2 uvAlt = v.uv.xy;
                uvAlt.y = 1.0 - uvAlt.y;
                uvAlt = uvAlt * 2.0 - 1.0;
                o.vertex = float4(uvAlt.xy, 0.0, 1.0);
                o.worldPos = mul(_WorldMatrix, v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float b_size = _BrushSize;
                float b_hardness = _BrushHardness;
                /*float4 b_color = _BrushColor;
                b_color.w = 1.0;*/
                float multiplier = distance(_Mouse.xyz, i.worldPos);
                multiplier = 1.0 - smoothstep(b_size * b_hardness, b_size, multiplier);
                col = lerp(col, /*b_color*/_BrushColor, multiplier * _Mouse.w * _BrushOpacity);
                col = saturate(col);
                return col;
            }
            ENDCG
        }
    }
}
