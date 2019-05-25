Shader "Ditto/CameraMapping"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Rotation("rotation",vector) = (00.,0.0,0.0,1.0)
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 rotation;
			float4 _Rotation;
			v2f vert (appdata v)
			{ 
				v2f o;

				//float radZ = radians(rotation.x);
				//float sinZ = sin(radZ);
				//float cosZ = cos(radZ);
				  
				//float3 Point = v.vertex.xyz;
				//float3 result=  float3(Point.x*cosZ-Point.y*sinZ,Point.x*sinZ+Point.y*cosZ,Point.z);  
				//v.vertex = float4(result,1);
				
				float radX = radians(_Rotation.x);
				float radY = radians(_Rotation.y);
				float radZ = radians(_Rotation.z);

				float sinX = sin(radX);
				float cosX = cos(radX);
				float sinY = sin(radY);
				float cosY = cos(radY);
				float sinZ = sin(radZ);
				float cosZ = cos(radZ);
				  
				float4x4 result = float4x4(cosY*cosZ,-cosY*sinZ,sinY,0.0,
				cosX*sinZ+sinX*sinY*cosZ,cosX*cosZ-sinX*sinY*sinZ,-sinX*cosY,0.0,
				sinX*sinZ-cosX*sinY*cosZ,sinX*cosZ+cosX*sinY*sinZ,cosX*cosY,0.0,
				0.0,0.0,0.0,1.0);
				v.vertex = mul(result,v.vertex);


				 
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	

				 

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
