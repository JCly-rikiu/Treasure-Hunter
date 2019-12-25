public enum HexItemType
{
    Treasure, Key, Coin, Bomb, Poison, EnergyPlus, Bonus, Stop, Ward, Shovel, MagicBox
}

public static class HexItemTypeExtensions
{
    public static bool isMagicBox(this HexItemType type)
    {
        switch (type)
        {
            case HexItemType.Bonus:
                return true;
        }
        return false;
    }
}
