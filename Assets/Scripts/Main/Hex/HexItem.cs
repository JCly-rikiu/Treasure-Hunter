using UnityEngine;

public class HexItem : MonoBehaviour
{
    public HexItemType itemType;
    public static HexItem itemPrefab;
    public Transform[] itemPrefabs;
    public Transform[] itemEPrefabs;
    public Transform[] itemTPrefabs;
    Transform item, showEffect, effect;

    public HexGrid Grid { get; set; }

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
        Transform itemP = itemPrefabs[(int)itemType];
        if (itemP)
        {
            item = Instantiate<Transform>(itemP);
            item.localRotation = Quaternion.Euler(0f, Random.Range(0, 6) * 60, 0f);
            item.SetParent(transform, false);
        }

        Transform itemEP = itemEPrefabs[(int)itemType];
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
        switch (itemType)
        {
            case HexItemType.Treasure:
                unit.Score += 300;
                unit.GetTreasure();
                break;
            case HexItemType.Key:
                unit.SetKey(true);
                break;
            case HexItemType.Coin:
                unit.Score += 50;
                break;
            case HexItemType.Bonus:
                unit.Score += 100;
                break;
            case HexItemType.Bomb:
                unit.SetZeroSpeed();
                break;
            case HexItemType.Poison:
                unit.GetItem(itemType);
                break;
            case HexItemType.Energy:
                unit.SpeedEffect(20, 3);
                break;
            case HexItemType.FakeTreasureItem:
                unit.GetItem(itemType);
                break;
            case HexItemType.Change:
                unit.GetItem(itemType);
                break;
            case HexItemType.FakeTreasure:
                unit.SetKey(false);
                break;
        }
    }

    public void RemoveFromMap()
    {
        if (item)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject, 2);
        }

        if (effect)
        {
            effect.gameObject.SetActive(true);
            Destroy(effect.gameObject, 2);
        }

        location.Item = null;
        location = null;
        Destroy(gameObject, 2);
    }

    public bool isWalkable(HexUnit unit)
    {
        if (itemType == HexItemType.Treasure && !unit.HasKey)
        {
            return false;
        }
        if (itemType == HexItemType.FakeTreasure && !unit.HasKey)
        {
            return false;
        }
        return true;
    }
}
