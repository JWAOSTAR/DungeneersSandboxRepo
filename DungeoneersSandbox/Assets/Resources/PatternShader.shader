Shader "DS/PatternShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BGColor("Background", Color) = (1.0, 1.0, 1.0, 1.0)
        _XTiling("X Tiling", Float) = 1.0
        _YTiling("Y Tiling", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _XTiling;
            float _YTiling;
            //float4 _BGColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float2 st = float2(i.uv.x * _XTiling, i.uv.y * _YTiling);
                st = (st - floor(st));
                fixed4 col = tex2D(_MainTex, st.xy);
                //col.w = (col.w == 0.0) ? _BGColor : col.w;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
