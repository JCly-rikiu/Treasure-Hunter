using UnityEngine;


[System.Serializable]
public struct HexTerrainCollection
{
    public Transform[] walkablePrefabs;
    public Transform[] unwalkablePrefabs;

    public Transform Pick(bool isWalkable)
    {
        float choice = Random.value;
        if (isWalkable)
        {
            return walkablePrefabs[(int)(choice * walkablePrefabs.Length)];
        }
        else
        {
            return unwalkablePrefabs[(int)(choice * unwalkablePrefabs.Length)];
        }
    }
}
