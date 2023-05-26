// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SpaceTimeShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_ease ("Ease", float) = 3
		_FadingStart("FadingStart", FLOAT) = -2.0
		
	}
	SubShader
	{
        LOD 200
    	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    	ZWrite true
    	Blend SrcAlpha OneMinusSrcAlpha
    	Cull Off
		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members yPos)
//#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float yPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 worldPos : WORLD_POS;
				float3 normal : NORMAL;
			};
			
			#include "/SpaceTimeMath.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float _FadingStart;
			float _FadingPerUnit;
			
			float _centUsageNormal;

			float mapf(float s, float a1, float a2, float b1, float b2)
			{
				return b1 + (s-a1)*(b2-b1)/(a2-a1);
			}
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.y = GetYPosAtPoint(v.vertex.xz, _centUsageNormal);;
				

				float2 texelSize = float2(0.1, 0.1);

				float x1 = GetYPosAtPoint(v.vertex.xz + float2(-texelSize.x, 0), _centUsageNormal);
				float x2 = GetYPosAtPoint(v.vertex.xz + float2( texelSize.x, 0), _centUsageNormal);
				
				float z1 = GetYPosAtPoint(v.vertex.xz + float2(0,-texelSize.y), _centUsageNormal);
				float z2 = GetYPosAtPoint(v.vertex.xz + float2(0, texelSize.y), _centUsageNormal);
				
				
				
				float sx = GetYPosAtPoint(v.vertex.xz + float2( 0.1, 0), _centUsageNormal);
				float sz = GetYPosAtPoint(v.vertex.xz + float2( 0, 0.1), _centUsageNormal);

				o.normal = normalize(float3(x1 - x2, 2, z1 - z2));
				
				o.yPos = v.vertex.y;

				o.worldPos = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);


				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			float4 WhiteHightGraph (v2f i)
			{
				float f3 = fmod(-i.yPos, 10) / 10;
				
				float c3 = (f3 < 0.1) ? 1 : 0;
				
				float4 col = float4(1,1,1, c3);
				return col;
			}
			float4 NormalHeightTerrain (v2f i)
			{
				float2 ddxUV = ddx(i.uv);
				float2 ddyUV = ddy(i.uv);
				float mipLevel = max(0.5 * log2(max(dot(ddxUV, ddxUV), dot(ddyUV, ddyUV))), 0);

				float accuracyOfAlpha = 200;
				float alphaCutoff = 1 - (1 / accuracyOfAlpha);
				
				float4 linesColor = WhiteHightGraph(i);

				float4 fillColor = float4(1,1,1,1);
					
				if (i.normal.y > alphaCutoff)
				{
					float alphaRange = 1;
					fillColor.a += (i.normal.y - alphaCutoff) * accuracyOfAlpha * alphaRange;
				}
				fillColor.a = 1;
				fillColor.g = mapf(i.normal.y, 0.9, 1, 1, 0);
				fillColor.b = mapf(i.normal.y, 0.99, 1, 1, 0);
				
				fillColor.r = clamp(mapf(i.normal.y, 0.9999, 1, 0, 1) / 2,0,1);
				fillColor.r += clamp(mapf(i.normal.y, 0.999995, 1, 0, 1) / 2, 0, 1);

				float4 col = linesColor.a > 0 ? linesColor : fillColor;

				if (col.a < 0.1)
				{

				}
				return col;
			}
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a = max (col.a, 0.5);
				col.a = 1;
				if (i.yPos < _FadingStart) {
					// col.a = min (col.a, col.a - (_FadingStart - i.yPos) * _FadingPerUnit);
				}
				// float f = fmod(-i.yPos, 1);
				// col.rgb = float3(f,f,f);
				
				float f1 = fmod(-i.yPos, 0.01) * 100;
				float f2 = fmod(-i.yPos, 0.1) * 10;
				float f3 = fmod(-i.yPos, 10) / 10;
				
				float c3 = (f3 < 0.1) ? 1 : 0;
				
				
				
				float accuracyOfAlpha = 1000;
				float alphaCutoff = 1 - (1 / accuracyOfAlpha);
				
				
				// if (i.normal.y > alphaCutoff)
				// {
				// 	float alphaRange = 1 - col.a;
				// 	col.a += (i.normal.y - alphaCutoff) * accuracyOfAlpha * alphaRange;
				// }
				
				col = NormalHeightTerrain(i);//WhiteHightGraph(i.yPos);//float4(1,1,1,c * col.a);
				// col = float4(0,0,0,1);
				return col;
			}
			ENDCG
		}
	}
}
