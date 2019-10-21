Shader "redHealth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Amount", Range (0.0, 1.0)) = 0.0
        _tAt ("tAt", Range (0.0, 1.0)) = 0.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            uniform float _Amount;
            uniform float _tAt;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.r = 1;
                col.g = 0;
                col.b = 0;
                col.a = 0;

                float min = lerp(0.0f, _Amount, _tAt);
                float max = 1.0f - min;

                if(i.uv.x < min) {
                    col.a = 1.0f - (i.uv.x / min);
                } else if(i.uv.x > max) {
                    col.a = (i.uv.x - max) / min;
                }

                float val = 0.0f;
                if(i.uv.y < min) {
                    val = 1.0f - (i.uv.y / min);
                } else if(i.uv.y > max) {
                    val = (i.uv.y - max) / min;
                }

                if(val > col.a) {
                    col.a = val;
                }

                col.a = lerp(0, 0.5f, col.a);
                
                return col;
            }
            ENDCG
        }
    }
}
