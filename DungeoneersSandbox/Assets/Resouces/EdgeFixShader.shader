Shader "DS/EdgeFixShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_UV_Unit;
            sampler2D _EdgeMap;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float map = tex2D(_EdgeMap, i.uv);

                float3 avg = col;

                if (map.x < 0.2) 
                {
                    int num = 0;

                    avg = float3(0.0, 0.0, 0.0);
                    for (float v = -1.5; v <= 1.5; v++)
                    {
                        for (float u = -1.5; u <= 1.5; u++)
                        {
                            float3 m_tex = tex2D(_MainTex, i.uv + _MainTex_UV_Unit.xy * float2(u, v));
                            float e_tex = tex2D(_EdgeMap, i.uv + _MainTex_UV_Unit.xy * float2(u, v));

                            num += step(0.1, e_tex);
                            avg += m_tex * step(0.1, e_tex);
                        }
                    }

                    avg /= num;
                }

                col.xyz = avg;
                
                return col;
            }
            ENDCG
        }
    }
}
