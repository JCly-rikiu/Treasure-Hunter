using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * outerToInner;
    public const float outerToInner = 0.866025404f;
    public const float innerToOuter = 1f / outerToInner;

    public const float elevationStep = 3f;

    public const int chunkSizeX = 5, chunkSizeZ = 5;

    public static Texture2D noiseSource;

    public const float noiseScale = 0.003f;
    public const float cellPerturbStrength = 4f;
    public const float elevationPerturbStrength = 1.5f;

    public const int hashGridSize = 256;
    public const float hashGridScale = 0.25f;
    static HexHash[] hashGrid;

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }

    public static void InitializeHashGrid(int seed)
    {
        hashGrid = new HexHash[hashGridSize * hashGridSize];
        Random.State currentState = Random.state;
        Random.InitState(seed);
        for (int i = 0; i < hashGrid.Length; i++)
        {
            hashGrid[i] = HexHash.Create();
        }
        Random.state = currentState;
    }
}
