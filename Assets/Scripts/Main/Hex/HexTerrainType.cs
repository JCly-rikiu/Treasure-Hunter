public enum HexTerrainType
{
    Ground, Grass, Mud, Snow
}

public static class HexHexTerrainTypeExtensions
{
    public static int GetMoveCost(this HexTerrainType terrainType)
    {
        switch (terrainType)
        {
            case HexTerrainType.Ground:
                return 1;
            case HexTerrainType.Grass:
                return 0;
            case HexTerrainType.Mud:
                return 3;
            case HexTerrainType.Snow:
                return 2;
        }

        return 0;
    }
}
