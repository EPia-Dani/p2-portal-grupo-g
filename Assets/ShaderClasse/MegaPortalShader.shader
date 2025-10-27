Shader "Hidden/MegaPortalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex("Mask texture", 2D) = "white" {}
        _Cutout("Cutout", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
        Lighting Off
        Cull Back
        ZWrite On
        ZTest Less
        Fog{ Mode Off }

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
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float4 screenPos: TEXCOORD1; //del profe
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.screenPos = ComputeScreenPos(o.vertex); //del profe
            //o.screenPos = float2(1,1) - o.uv; //per provar profe
            return o;
        }
        
        sampler2D _MainTex;
        sampler2D _MaskTex;
        //float4 _MainTex_ST; //nou
        //float4 _MaskTex_ST; //nou
        float _Cutout;

        fixed4 frag (v2f i) : SV_Target
        {
            i.screenPos /= i.screenPos.w; //del profe
            // aplica tiling/offset definits al material
            //float2 uvMain = TRANSFORM_TEX(i.uv, _MainTex);// nou
            //float2 uvMask = TRANSFORM_TEX(i.uv, _MaskTex);// nou
            //fixed4 l_MaskColor = tex2D(_MaskTex, uvMask); //nou

            fixed4 l_MaskColor= tex2D(_MaskTex, i.uv); //del profe
            //if (l_MaskColor.a < _Cutout) del profe. pero nosaltres ens quedem amb la part de dins
            if (l_MaskColor.a > _Cutout)
                clip(-1);
            fixed4 col = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y)); //del profe
            //fixed4 col = tex2D(_MainTex, uvMain); nou
            return col;
        }
        
        ENDCG
        }
    }
}