//shader que serveix per renderitzar el portal? Inverteix els colors de la textura
Shader "Hidden/MegaPortalShader"
{
    Properties //el que surt com a opció al material
    {
        //"aquest shader accepta una textura 2D com a entrada i li diem mainText"
        _MainTex ("Texture", 2D) = "white" {}
        //exemples profe
        _CullingText ("Culling Texture", 2D) = "white" {}
        _ClippingOffset ("Clipping Offset", Range(0.0, 0.5)) = 0.25
    }
    SubShader //el shader funciona en diferents passades
    {
        Tags{"Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque"} //definim les propietats del shader
        
        // No culling or depth
        Cull Off //no fer culling (renderitzar les dues cares), perquè és un portal
        ZWrite Off //no escriure a depth buffer
        ZTest Always //sempre passar el test de profunditat

        Pass
        {   //tot el que esta entre CGPROGRAM i ENDCG és codi Cg, el que està dins el shader
            CGPROGRAM 
            #pragma vertex vert //definim la funció vertex i fragment
            #pragma fragment frag

            #include "UnityCG.cginc" //incloem funcions de Unity

            struct appdata //definim l'estructura de dades del vertex shader (entrada)
            {
                float4 vertex : POSITION; //x,y,z,w (la w ha dit q no cal, q es mes barato 4 q tres eixos)
                float2 uv : TEXCOORD0;
            };

            struct v2f //definim l'estructura de dades del fragment shader (sortida del vertex shader)
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) //funció vertex, que rep l'entrada appdata i retorna v2f
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //transforma coordenades d'objecte a coordenades de pantalla
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex; //definim la textura d'entrada

            fixed4 frag (v2f i) : SV_Target //funció fragment, que rep l'entrada v2f i retorna un color (SV_Target)
            {
                fixed4 col = tex2D(_MainTex, i.uv); //li dic textura, coordenades uv i ens retorna el color
                // just invert the colors
                col.rgb = 1 - col.rgb; 
                return col;
            }
            ENDCG
        }
    }
}
