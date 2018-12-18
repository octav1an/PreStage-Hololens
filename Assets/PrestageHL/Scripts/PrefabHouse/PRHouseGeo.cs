using System.Collections;
using System.Collections.Generic;
using PRGeoClasses;
using UnityEngine;

public class PRHouseGeo : PRGeo
{

    #region Unity
    protected override void Awake()
    {
        //base.Awake();
        GetComponent<MeshFilter>().mesh = GenerateMesh();
        CubeMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = CubeMesh;

        PrEdgeHolders = CreateUniqEdgePrefabs(GenerateEdgeHolders());
        // Invert normals
        ReverseNormals();
    }

    protected override void Start()
    {
        //base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    #endregion

    #region Generate
    protected override Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 9;
        // Define mesh vertices.
        Vector3 v0 = new Vector3(-0.5f, -0.633333f, -0.5f);
        Vector3 v1 = new Vector3(-0.5f, -0.633333f, 0.5f);
        Vector3 v2 = new Vector3(0.5f, -0.633333f, 0.5f);
        Vector3 v3 = new Vector3(0.5f, -0.633333f, -0.5f);
        Vector3 v4 = new Vector3(-0.5f, 0.366667f, -0.5f);
        Vector3 v5 = new Vector3(-0.5f, 0.366667f, 0.5f);
        Vector3 v6 = new Vector3(0.5f, 0.366667f, 0.5f);
        Vector3 v7 = new Vector3(0.5f, 0.366667f, -0.5f);
        Vector3 v8 = new Vector3(0.0f, 0.866667f, -0.5f);
        Vector3 v9 = new Vector3(0.0f, 0.866667f, 0.5f);
        // Make the faces array vertices.
        Vector3[] face0 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v1),
            transform.TransformVector(v0)
        };
        Vector3[] face1 = new Vector3[4]
        {
            transform.TransformVector(v9),
            transform.TransformVector(v5),
            transform.TransformVector(v4),
            transform.TransformVector(v8)
        };
        Vector3[] face2 = new Vector3[4]
        {
            transform.TransformVector(v9),
            transform.TransformVector(v8),
            transform.TransformVector(v7),
            transform.TransformVector(v6)
        };
        Vector3[] face3 = new Vector3[4]
        {
            transform.TransformVector(v1),
            transform.TransformVector(v0),
            transform.TransformVector(v4),
            transform.TransformVector(v5)
        };
        Vector3[] face4 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v0),
            transform.TransformVector(v4),
            transform.TransformVector(v7)
        };
        Vector3[] face5 = new Vector3[4]
        {
            transform.TransformVector(v2),
            transform.TransformVector(v1),
            transform.TransformVector(v5),
            transform.TransformVector(v6)
        };
        Vector3[] face6 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v6),
            transform.TransformVector(v7)
        };
        Vector3[] face7 = new Vector3[3]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v8),
            transform.TransformVector(v7)
        };
        Vector3[] face8 = new Vector3[3]
        {
            transform.TransformVector(v9),
            transform.TransformVector(v5),
            transform.TransformVector(v6)
        };
        // Create FaceIndices arrays.
        // TODO: Here is the problem
        int[] faceIndices0 = new int[4] { 3, 2, 1, 0 };
        int[] faceIndices1 = new int[4] { 4, 5, 6, 7 };
        int[] faceIndices2 = new int[4] { 8, 9, 10, 11 };
        int[] faceIndices3 = new int[4] { 12, 13, 14, 15 };
        int[] faceIndices4 = new int[4] { 19, 18, 17, 16 };
        int[] faceIndices5 = new int[4] { 20, 21, 22, 23 };
        int[] faceIndices6 = new int[4] { 24, 25, 26, 27 };
        int[] faceIndices7 = new int[3] { 30, 29, 28 };
        int[] faceIndices8 = new int[3] { 33, 32, 31 };
        // Join the array with vertices.
        var list = new List<Vector3>();
        list.AddRange(face0);
        list.AddRange(face1);
        list.AddRange(face2);
        list.AddRange(face3);
        list.AddRange(face4);
        list.AddRange(face5);
        list.AddRange(face6);
        list.AddRange(face7);
        list.AddRange(face8);
        mesh.vertices = list.ToArray();
        // Assign indexes for each quad.
        mesh.SetIndices(faceIndices0, MeshTopology.Quads, 0);
        mesh.SetIndices(faceIndices1, MeshTopology.Quads, 1);
        mesh.SetIndices(faceIndices2, MeshTopology.Quads, 2);
        mesh.SetIndices(faceIndices3, MeshTopology.Quads, 3);
        mesh.SetIndices(faceIndices4, MeshTopology.Quads, 4);
        mesh.SetIndices(faceIndices5, MeshTopology.Quads, 5);
        mesh.SetIndices(faceIndices6, MeshTopology.Quads, 6);
        mesh.SetIndices(faceIndices7, MeshTopology.Triangles, 7);
        mesh.SetIndices(faceIndices8, MeshTopology.Triangles, 8);
        // Recalculate all
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    /// <summary>
    /// Generate the EdgeHolders for every Edge in every face. The array has overlaping EdgeHolders.
    /// </summary>
    /// <returns>Array with overlaping EdgeHolders.</returns>
    public override PREdgeHolder[] GenerateEdgeHolders()
    {
        PREdgeHolder[] edgeColl = new PREdgeHolder[CubeMesh.vertexCount];
        for (int i = 0; i < CubeMesh.subMeshCount; i++)
        {
            // Keep track of the actual number of the edge that is being generated,
            // If it is the last one - connect the vertex to first one.
            if (CubeMesh.GetTopology(i) == MeshTopology.Quads)
            {
                int index = -1;
                for (uint j = CubeMesh.GetIndexStart(i); j < CubeMesh.GetIndexStart(i) + CubeMesh.GetIndexCount(i); j++)
                {
                    index++;
                    if (index < 3)
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j + 1];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int) j;
                        edge.V1Index = (int) j + 1;
                        edgeColl[j] = edge;
                    }
                    else
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j - 3];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int) j;
                        edge.V1Index = (int) j - 3;
                        edgeColl[j] = edge;
                    }
                }
            }
            else if (CubeMesh.GetTopology(i) == MeshTopology.Triangles)
            {
                int index = -1;
                for (uint j = CubeMesh.GetIndexStart(i); j < CubeMesh.GetIndexStart(i) + CubeMesh.GetIndexCount(i); j++)
                {
                    index++;
                    if (index < 2)
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j + 1];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j + 1;
                        edgeColl[j] = edge;
                    }
                    else
                    {
                        Vector3 v0 = CubeMesh.vertices[j];
                        Vector3 v1 = CubeMesh.vertices[j - 2];
                        PREdgeHolder edge = new PREdgeHolder(v0, v1, this.gameObject);
                        edge.V0Index = (int)j;
                        edge.V1Index = (int)j - 2;
                        edgeColl[j] = edge;
                    }
                }
            }
        }
        return edgeColl;
    }

    void ReverseNormals()
    {
        MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }

    #endregion //Generate
}
