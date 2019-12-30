public static class HexItemTypeCollection
{
    public static HexItemType[] GetMapRandomType()
    {
        return new HexItemType[] { HexItemType.Energy, HexItemType.Poison, HexItemType.Coin, HexItemType.Bonus, HexItemType.Bomb, HexItemType.FakeTreasureItem, HexItemType.Change };
    }

    public static float[] GetMapRandomValue()
    {
        return new float[] { 0.25f, 0.5f, 0.75f, 0.825f, 0.9f, 0.9375f, 1f };
    }
}
