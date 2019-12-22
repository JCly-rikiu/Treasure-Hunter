Shader "Custom/Terrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular", Color) = (0.2,0.2,0.2)
        _BackgroundColor ("Background Color", Color) = (0,0,0)
        [NoScaleOffset] _GridCoordinates ("Grid Coordinates", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.5

        #include "HexCellData.cginc"

        sampler2D _MainTex, _GridCoordinates;

        struct Input
        {
            float2 uv_MainTex;
            float4 visibility;
        };

        half _Glossiness;
        fixed3 _Specular;
        fixed4 _Color;
        half3 _BackgroundColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            float3 pos = mul(unity_ObjectToWorld, v.vertex);

            float4 gridUV = float4(pos.xz, 0, 0);
            gridUV.x *= 1 / (4 * 8.66025404);
            gridUV.y *= 1 / (2 * 15.0);
            float2 cellDataCoordinates = floor(gridUV.xy) + tex2Dlod(_GridCoordinates, gridUV).rg;
            cellDataCoordinates *= 2;

            float4 cellData = GetCellData(cellDataCoordinates);
            data.visibility.x = cellData.x;
            data.visibility.x = lerp(0.2, 1, data.visibility.x);
            data.visibility.y = cellData.y;
            data.visibility.y = lerp(0.1, 1, data.visibility.y);
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            float explored = IN.visibility.y;
            o.Albedo = c.rgb * (IN.visibility.x * explored);
            o.Specular = _Specular * explored;
            o.Smoothness = _Glossiness;
            o.Occlusion = explored;
            o.Emission = 0;//_BackgroundColor * (1 - explored);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
