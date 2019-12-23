sampler2D _HexCellData;
float4 _HexCellData_TexelSize;

float4 GetCellData (float2 cellDataCoordinates)
{
    float2 uv = cellDataCoordinates + 0.5;
    uv.x *= _HexCellData_TexelSize.x;
    uv.y *= _HexCellData_TexelSize.y;
    return tex2Dlod(_HexCellData, float4(uv, 0, 0));
}
