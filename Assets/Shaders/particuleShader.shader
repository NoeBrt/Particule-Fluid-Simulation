Shader "Instanced/Particle2D" {
	Properties {
		
	}
	SubShader {

		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"


            struct Particule{
                float2 position;
                float2 velocity;
				float radius;
            };
			StructuredBuffer<Particule> particuleBuffer;
			float4 colA;
			Texture2D<float4> ColourMap;
			SamplerState linear_clamp_sampler;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 colour : TEXCOORD1;
			};

			v2f vert (appdata_full v, uint instanceID : SV_InstanceID)
			{
				float speed = length(particuleBuffer[instanceID].velocity);
				float speedT = saturate(speed);
				float colT = speedT;
				
				float3 centreWorld = float3(particuleBuffer[instanceID].position, 0);
				float3 worldVertPos = centreWorld + mul(unity_ObjectToWorld, v.vertex * particuleBuffer[instanceID].radius);
				float3 objectVertPos = mul(unity_WorldToObject, float4(worldVertPos.xyz, 1));

				v2f o;
				o.uv = v.texcoord;
				o.pos = UnityObjectToClipPos(objectVertPos);
				o.colour = ColourMap.SampleLevel(linear_clamp_sampler, float2(colT, 0.5), 0);
				return o;
			}


			float4 frag (v2f i) : SV_Target
			{
				float2 centreOffset = (i.uv.xy - 0.5) * 2;
				float sqrDst = dot(centreOffset, centreOffset);
				float delta = fwidth(sqrt(sqrDst));
				float alpha = 1 - smoothstep(1 - delta, 1 + delta, sqrDst);

				float3 colour = i.colour;
				return float4(colour, alpha);
			}

			ENDCG
		}
	}
}