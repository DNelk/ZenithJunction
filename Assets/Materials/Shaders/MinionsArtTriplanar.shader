Shader "Toon/MinionsArtTriplanar"
{
    Properties
    {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("Top Texture", 2D) = "white" {}
        _MainTexSide ("Side/Bottom Texture", 2D) = "white" {}
        _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
        _Normal("Normal/Noise", 2D) = "bump" {}
        _Scale("Top Scale", Range(-2,2)) = 1
        _SideScale("Side Scale", Range(-2,2)) = 1
        _NoiseScale("Noise Scale", Range(-2,2)) = 1
        _TopSpread("TopSpread", Range(-2,20)) = 1
        _EdgeWidth("EdgeWith", Range(0,0.5)) = 1
        _RimPower("Rim Power", Range(-2,20)) = 1
        _RimColor("Rim Color Top", Color) = (0.5,0.5,0.5,1)
        _RimColor2("Rim Color Side/Bottom", Color) = (0.5,0.5,0.5,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf ToonRamp
        
        sampler2D _Ramp;
        
        //Toon lighting (use angle btwn light and normal using texture ramp)
        #pragma lighting ToonRamp exclude_path:prepass
        inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
        {
            #ifndef USING_DIRECTIONAL_LIGHT
            lightDir = normalize(lightDir);
            #endif
            
            //Dot prod of output normal and light dir
            half d = dot(s.Normal, lightDir)*0.5 + 0.5;
            //Maps it to the ramp i think
            half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;
            
            //Multiply that by the albedo and light color
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
            c.a = 0;
            return c;
        }
        
        sampler2D _MainTex, _MainTexSide, _Normal;
        float4 _Color, _RimColor, _RimColor2;
        float _RimPower;
        float _TopSpread, _EdgeWidth;
        float _Scale, _SideScale, _NoiseScale;
        
        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float3 worldPos; //World position
            float3 worldNormal; //World Normal
            float3 viewDir; //Direction for rim light
        };
        
        void surf (Input IN, inout SurfaceOutput o)
        { 
            //clamp and increase world normal to blend btwn textures
            float3 blendNormal = saturate(pow(IN.worldNormal * 1.4,4));
            
            //Normal Noise Triplanar for x, y, z sides 
            float3 xn = tex2D(_Normal, IN.worldPos.zy * _NoiseScale);
            float3 yn = tex2D(_Normal, IN.worldPos.zx * _NoiseScale);
            float3 zn = tex2D(_Normal, IN.worldPos.xy * _NoiseScale);
            
            //Lerped together all sides for Noise tex
            float3 noisetexture = zn;
            noisetexture = lerp(noisetexture, xn, blendNormal.x);
            noisetexture = lerp(noisetexture, yn, blendNormal.y);
            
            //Triplanar Top
            float3 xm = tex2D(_MainTex, IN.worldPos.zy * _Scale);
            float3 zm = tex2D(_MainTex, IN.worldPos.xy * _Scale);
            float3 ym = tex2D(_MainTex, IN.worldPos.zx * _Scale);
            
            //Lerped together all sides for top tex
            float3 toptexture = zm;
            toptexture = lerp(toptexture, xm, blendNormal.x);
            toptexture = lerp(toptexture, ym, blendNormal.y);
            
            //Triplanar side/bottom
            float3 x = tex2D(_MainTexSide, IN.worldPos.zy * _SideScale);
            float3 z = tex2D(_MainTexSide, IN.worldPos.xy * _SideScale);
            float3 y = tex2D(_MainTexSide, IN.worldPos.zx * _SideScale);
            
            //Lerped together all sides for side/bottom tex
            float3 sidetexture = z;
            sidetexture = lerp(sidetexture, x, blendNormal.x);
            sidetexture = lerp(sidetexture, y, blendNormal.y);
            
            //rim light for top
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal * noisetexture));
                  
            //rim light for side/bottom
            half rim2 = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            
            //dot prod of world normal and surf normal + Noise
            float worldNormalDotNoise = dot(o.Normal + (noisetexture.y + (noisetexture * 0.5)), IN.worldNormal.y);
            
            //if dot is higher than top spread slider, mut by Triplanar mapped top tex
            //step replaces if statement
            //if (worldNormalDotNoise > _TopSpread{o.Albedo = toptexture}
            float3 topTextureResult = step(_TopSpread, worldNormalDotNoise) * toptexture;
            
            //if dot is lower than top spread slider multiplied by triplaner mapped side/bottom tex
            float3 sideTextureResult = step(worldNormalDotNoise, _TopSpread) * sidetexture;
            
            //if dot is in between the two, darken the texture 
            float3 topTextureEdgeResult = step(_TopSpread, worldNormalDotNoise) * step(worldNormalDotNoise, _TopSpread + _EdgeWidth) * -0.15;       
                //Final albedo
            o.Albedo = topTextureResult + sideTextureResult + topTextureEdgeResult;
            o.Albedo *= _Color;
            
            //rim
            o.Emission = step(_TopSpread, worldNormalDotNoise) * _RimColor.rgb * pow(rim, _RimPower) + step(worldNormalDotNoise, _TopSpread) * _RimColor2.rgb * pow(rim2, _RimPower);
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}
