using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexGridChunk chunk;
    public HexCoordinates coordinates;
    public int Index { get; set; }

    [SerializeField]
    HexCell[] neighbors = new HexCell[6];

    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                // ShaderData.RefreshTerrain(this);
            }
        }
    }
    int terrainTypeIndex;
    public GameObject[] terrains;

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
                // ShaderData.ViewElevationChanged();
            }
            RefreshPosition();

            // Refresh();
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
                // ShaderData.ViewElevationChanged();
            }

            // Refresh();
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

    public int PlantLevel
    {
        get
        {
            return plantLevel;
        }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
            }
        }
    }
    int plantLevel;

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
    }

    public void InstantiateTerrain()
    {
        GameObject terrain;
        if (IsUnderwater)
        {
            terrain = Instantiate<GameObject>(terrains[1]);
        }
        else
        {
            terrain = Instantiate<GameObject>(terrains[2]);
        }
        terrain.transform.SetParent(transform, false);

        Debug.Log("e");
    }
}
