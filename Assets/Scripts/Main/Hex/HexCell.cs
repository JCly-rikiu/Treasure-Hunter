using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexGridChunk chunk;

    public HexCoordinates coordinates;

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    [SerializeField]
    HexCell[] neighbors = new HexCell[6];

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
