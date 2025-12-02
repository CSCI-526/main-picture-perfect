Shader "Custom/LavaFlow"
{
    Properties
    {
        _MainTex ("Lava Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,0.4,0,1)
        _FlowSpeed ("Flow Speed", Float) = 0.2
        _Tiling ("Tiling", Vector) = (1,1,0,0)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EmissionColor;
            float _FlowSpeed;
            float4 _Tiling;

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

            float _TimeY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 流动方向为 Y 轴向下
                float2 flowOffset = float2(0, -_Time.y * _FlowSpeed);
                o.uv = v.uv * _Tiling.xy + flowOffset;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _EmissionColor;
            }
            ENDCG
        }
    }
}
