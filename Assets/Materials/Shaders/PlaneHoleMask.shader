Shader "Custom/PlaneHoleMask"
{
    SubShader
    {
        Tags {
            "Queue" = "Transparent+1"
        }

        Pass {
            Blend Zero One
        }
    }

    FallBack "Diffuse"
}
