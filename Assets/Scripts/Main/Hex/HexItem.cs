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

    public bool IsExpired
    {
        get
        {
            return turns < 0;
        }
    }
    int turns;

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

    public void Effect()
    {
        item.gameObject.SetActive(false);
        if (effect)
        {
            effect.gameObject.SetActive(true);
        }
    }

    public void RemoveFromMap()
    {
        location.Item = null;
        location = null;
    }
}
