// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TexturedQuad" 
{
	Properties
	{
        // Shader properties
        //These are set in the unity editor
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
        }
        // Shader code
		Pass
        {
           Blend One OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _Color;

            float4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                
                float alpha = texColor.w;
                //float4 fo = float4(1, 1, 0, 1);
                float4 b = _Color*_Color.w;
                float4 c = b*texColor;
                c *= alpha;
                float4 color = c;
                return color;
            }
            ENDCG
		}
	} 
}
