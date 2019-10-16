Shader "VegetationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _hitOffset ("hitOffset", Range (0.0, 1.0)) = 0.0

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Tags{
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            }
           Lighting Off
           Blend One OneMinusSrcAlpha

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

            float random (float2 st) {
                return frac(sin(dot(st,
                                     float2(12.9898f,78.233f)))*
                    43758.5453123f);
            }

            float noise (float2 st) {
                float2 i = floor(st);
                float2 f = frac(st);

                // Four corners in 2D of a tile
                float a = random(i);
                float b = random(i + float2(1.0f, 0.0f));
                float c = random(i + float2(0.0f, 1.0f));
                float d = random(i + float2(1.0f, 1.0f));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) +
                        (c - a)* u.y * (1.0 - u.x) +
                        (d - b) * u.x * u.y;
            }

            #define OCTAVES 6
            float fbm (float2 st) {
                // Initial values
                float value = 0.0;
                float amplitude = .5;
                float frequency = 0.;
                //
                // Loop of octaves
                for (int i = 0; i < OCTAVES; i++) {
                    value += amplitude * noise(st);
                    st *= 2.;
                    amplitude *= .5;
                }
                return value;
            }

            uniform float _hitOffset;

            v2f vert (appdata v)
            {
                v2f o;

                float4 temp = mul(unity_ObjectToWorld, v.vertex);
                float noise = ((1.5f*fbm(0.1f*temp.xy + 4*_Time.x))) - 1.0f;
                float x = lerp(0, noise + _hitOffset, v.uv.y);
                float4 offset = float4(x, v.vertex.y, v.vertex.z, v.vertex.w);
                float4 modelVertex = v.vertex + offset;
                o.vertex = UnityObjectToClipPos(modelVertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgba *= col.a;

                return col;
            }
            ENDCG
        }
    }
}
