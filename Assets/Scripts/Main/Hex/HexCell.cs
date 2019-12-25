using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexGridChunk chunk;
    public HexCoordinates coordinates;
    public int Index { get; set; }

    [SerializeField]
    HexCell[] neighbors = new HexCell[6];

    public RectTransform uiRect;

    public HexMapCamera mapCamera;

    public HexUnit Unit { get; set; }

    public HexItem Item { get; set; }

    public HexCellShaderData ShaderData { get; set; }

    public Transform terrain;
    public HexTerrainType terrainType;

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }

            int originalViewElevation = ViewElevation;
            elevation = value;
            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }
            RefreshPosition();
        }
    }
    int elevation = int.MinValue;

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }

            int originalViewElevation = ViewElevation;
            waterLevel = value;
            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }
        }
    }
    int waterLevel;
    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    public int ViewElevation
    {
        get
        {
            return elevation >= waterLevel ? elevation : waterLevel;
        }
    }

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }
    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }
    bool hasIncomingRiver, hasOutgoingRiver;

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }
    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }
    HexDirection incomingRiver, outgoingRiver;
    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
        }
    }
    int distance;
    public HexCell PathFrom { get; set; }
    public int SearchHeuristic { get; set; }
    public int SearchPriority
    {
        get
        {
            return distance + SearchHeuristic;
        }
    }
    public HexCell NextWithSamePriority { get; set; }
    public int SearchPhase { get; set; }

    public bool IsVisible
    {
        get
        {
            return visibility > 0 && Explorable;
        }
    }
    int visibility;
    public void IncreaseVisibility()
    {
        visibility += 1;
        if (visibility == 1)
        {
            IsExplored = true;
            uiRect.GetChild(1).GetComponent<Image>().enabled = false;
            ShaderData.RefreshVisibility(this);

            if (Unit && !Unit.Owned)
            {
                Unit.unitRenderer.enabled = IsVisible;
            }

            if (Item)
            {
                Item.ShowEffect(IsVisible);
            }
        }
    }
    public void DecreaseVisibility()
    {
        visibility -= 1;
        if (visibility == 0)
        {
            ShaderData.RefreshVisibility(this);

            if (Unit && !Unit.Owned)
            {
                Unit.unitRenderer.enabled = IsVisible;
            }

            if (Item)
            {
                Item.ShowEffect(IsVisible);
            }
        }
    }

    public bool IsExplored
    {
        get
        {
            return explored && Explorable;
        }
        private set
        {
            explored = value;
        }
    }
    bool explored;

    public bool Explorable { get; set; }

    public bool IsWalkable { get; set; }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    public void SetCloudWater()
    {
        Vector3 uiPosition = uiRect.localPosition;

        Transform cloud = uiRect.GetChild(1);
        Vector3 cloudPosition = cloud.localPosition;
        cloudPosition.z = -(11 * HexMetrics.elevationStep) - uiPosition.z;
        cloud.localPosition = cloudPosition;
        cloud.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 6) * 60f);

        Transform water = uiRect.GetChild(2);
        Vector3 waterPosition = water.localPosition;
        waterPosition.z = -(waterLevel * HexMetrics.elevationStep) - uiPosition.z + 1f;
        water.localPosition = waterPosition;
        water.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 6) * 60f);
        water.GetComponent<Image>().enabled = IsUnderwater;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor))
        {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
    }

    bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (elevation >= neighbor.elevation || waterLevel == neighbor.elevation);
    }

    public void ResetVisibility()
    {
        if (visibility > 0)
        {
            visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void SetLabel(string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;

        float angle = mapCamera.GetRotationAngle() + 30f;
        int delta = Mathf.FloorToInt(angle / 60f);
        if (delta == 6)
        {
            delta = 0;
        }

        uiRect.localRotation = Quaternion.Euler(0f, 0f, -60f * delta);
    }

    public void InstantiateTerrain()
    {
        Transform t = Instantiate<Transform>(terrain);
        t.localRotation = Quaternion.Euler(0f, Random.Range(0, 6) * 60, 0f);
        t.SetParent(transform, false);
    }
}
