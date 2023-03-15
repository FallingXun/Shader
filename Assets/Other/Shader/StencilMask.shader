Shader "Custom/StencilMask"
{
    SubShader
    {
        Properties
        {
            _Color("Main Color", Color) = (1,1,1,1)
            [IntRange]_Stencil("Stencil", Range(0, 255)) = 0
        }

        Stencil
        {
            Ref [_Stencil]
            Pass Replace
        }

        ColorMask 0

        Pass
        {
            
        }
    }
}