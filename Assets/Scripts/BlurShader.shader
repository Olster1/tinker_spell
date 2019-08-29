Shader "BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Horizontal ("IsHorizontal", Int) = 0
        _TexDimensionX ("TexDimensionX", Float) = 0
        _TexDimensionY ("TexDimensionY", Float) = 0
        
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
            //uniform float _Radius;
            uniform int _Horizontal;
            uniform float _TexDimensionX;
            uniform float _TexDimensionY;
            static float weight[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

            fixed4 frag (v2f i) : SV_Target
            {
                float2 tex_offset = 1.0 / float2(_TexDimensionX, _TexDimensionY);

                fixed2 TexCoords = i.uv;
                float3 result = tex2D(_MainTex, TexCoords).rgb * weight[0]; // current fragment's contribution
                if(_Horizontal == 0)
                {
                    for(int i = 1; i < 5; ++i)
                    {
                        result += tex2D(_MainTex, TexCoords + float2(tex_offset.x * i, 0.0)).rgb * weight[i];
                        result += tex2D(_MainTex, TexCoords - float2(tex_offset.x * i, 0.0)).rgb * weight[i];
                    }
                }
                else
                {
                    for(int i = 1; i < 5; ++i)
                    {
                        result += tex2D(_MainTex, TexCoords + float2(0.0, tex_offset.y * i)).rgb * weight[i];
                        result += tex2D(_MainTex, TexCoords - float2(0.0, tex_offset.y * i)).rgb * weight[i];
                    }
                }
                fixed4 col = float4(result, 1.0);
                return col;
            }
            ENDCG
        }
    }
}
