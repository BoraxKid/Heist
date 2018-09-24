Shader "Hidden/StaticSwitchNode"
{
	Properties
	{
		_A ("True", 2D) = "white" {}
		_B ("False", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			int _Condition;

			float4 frag( v2f_img i ) : SV_Target
			{
				if( _Condition == 1)
					return tex2D( _A, i.uv );
				else
					return tex2D( _B, i.uv );
			}
			ENDCG
		}
	}
}
