using UnityEngine;

public class HexItem : MonoBehaviour
{
    public HexItemType itemType;
    public static HexItem itemPrefab;
    public Transform[] itemPrefabs;
    public Transform[] itemEPrefabs;
    public Transform[] itemTPrefabs;
    Transform item, showEffect, effect;

    public bool Owned { get; set; }

    public HexGrid Grid { get; set; }

    public int VisionRange
    {
        get
        {
            return 2;
        }
    }

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;
            value.Item = this;
            transform.localPosition = value.Position;
        }
    }
    HexCell location;

    public void InstantiateItem()
    {
        HexItemType type = itemType;
        // HexItemType type = itemType.isMagicBox() ? HexItemType.MagicBox : itemType;
        Transform itemP = itemPrefabs[(int)type];
        if (itemP)
        {
            item = Instantiate<Transform>(itemP);
            item.localRotation = Quaternion.Euler(0f, Random.Range(0, 6) * 60, 0f);
            item.SetParent(transform, false);
        }

        Transform itemEP = itemEPrefabs[(int)type];
        if (itemEP)
        {
            showEffect = Instantiate<Transform>(itemEP);
            showEffect.localRotation = Quaternion.Euler(0f, Random.Range(0, 6) * 60, 0f);
            showEffect.SetParent(transform, false);
        }

        Transform effectP = itemTPrefabs[(int)itemType];
        if (effectP)
        {
            effect = Instantiate<Transform>(effectP);
            effect.localRotation = Quaternion.Euler(0f, Random.Range(0, 6) * 60, 0f);
            effect.SetParent(transform, false);
        }
    }

    public void ShowEffect(bool toggle)
    {
        if (showEffect)
        {
            showEffect.gameObject.SetActive(toggle);
        }
    }

    public void Effect(HexUnit unit)
    {
        if (item)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject, 1);
        }

        if (effect)
        {
            effect.gameObject.SetActive(true);
            Destroy(effect.gameObject, 1);
        }

        switch (itemType)
        {
            case HexItemType.Treasure:
                unit.Score += 500;
                unit.hasTreasure = true;
                break;
            case HexItemType.Key:
                unit.hasKey = true;
                break;
            case HexItemType.Coin:
                unit.Score += 50;
                break;
            case HexItemType.Bomb:
                unit.SetZeroSpeed();
                break;
            case HexItemType.Poison:
                unit.speedMinus.Add(20);
                unit.speedMinus.Add(20);
                unit.speedMinus.Add(20);
                break;
            case HexItemType.EnergyPlus:
                unit.speedPlus.Add(20);
                unit.speedPlus.Add(20);
                unit.speedPlus.Add(20);
                break;
            case HexItemType.Bonus:
                unit.Score += 100;
                break;
            case HexItemType.Stop:
                break;
            case HexItemType.Ward:
                break;
            case HexItemType.Shovel:
                break;
            case HexItemType.MagicBox:
                break;
        }
    }

    public void RemoveFromMap()
    {
        location.Item = null;
        location = null;
    }

    public bool isWalkable(HexUnit unit)
    {
        if (itemType == HexItemType.Treasure && !unit.hasKey)
        {
            return false;
        }

        if (itemType == HexItemType.Stop)
        {
            return false;
        }

        return true;
    }
}
