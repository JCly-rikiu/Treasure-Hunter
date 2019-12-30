public static class HexItemTypeCollection
{
    public static HexItemType[] GetMapRandomType()
    {
        return new HexItemType[] { HexItemType.Energy, HexItemType.Poison, HexItemType.Coin, HexItemType.Bonus, HexItemType.Bomb, HexItemType.FakeTreasureItem, HexItemType.Change };
    }

    public static float[] GetMapRandomValue()
    {
        return new float[] { 0.2f, 0.4f, 0.65f, 0.725f, 0.8f, 0.8875f, 1f };
    }
}
