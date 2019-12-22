using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
public class HexMesh : MonoBehaviour
{
    public bool useCollider;

    Mesh hexMesh;
    MeshCollider meshCollider;

    [NonSerialized] List<Vector3> vertices, cellIndices;
    [NonSerialized] List<int> triangles;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";

        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }

    public void Clear()
    {
        hexMesh.Clear();

        vertices = ListPool<Vector3>.Get();
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        hexMesh.RecalculateNormals();

        if (useCollider)
        {
            meshCollider.sharedMesh = hexMesh;
        }
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
}
