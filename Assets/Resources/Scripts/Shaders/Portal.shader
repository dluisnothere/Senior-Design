Shader "Unlit/Portal"
{
    // Using an unlit texture because the texture doesn't need to be effected by lighting.

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenCoord: TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenCoord = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // fix UV
                float2 screenSpaceUV = i.screenCoord.xy / i.screenCoord.w;

                // sample the texture
                fixed4 finalCol = tex2D(_MainTex, screenSpaceUV);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return finalCol;
            }
            ENDCG
        }
    }
}
