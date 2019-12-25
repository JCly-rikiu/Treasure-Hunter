public static class HexItemTypeCollection
{
    public static HexItemType[] GetMapRandom()
    {
        return new HexItemType[] { HexItemType.Coin, HexItemType.Bomb, HexItemType.Poison, HexItemType.EnergyPlus, HexItemType.Bonus };
    }

    public static HexItemType[] GetInventoryRandom()
    {
        return new HexItemType[] { HexItemType.Bomb, HexItemType.Poison, HexItemType.Stop, HexItemType.Ward, HexItemType.Shovel, HexItemType.EnergyPlus };
    }
}
