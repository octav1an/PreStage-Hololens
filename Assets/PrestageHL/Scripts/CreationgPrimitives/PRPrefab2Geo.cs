using System.Collections;
using System.Collections.Generic;
using PRGeoClasses;
using UnityEngine;

public class PRPrefab2Geo : PRGeo
{

    #region Unity
    protected override void Awake()
    {
        base.Awake();

        // Invert normals
        //ReverseNormals();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    #endregion // Unity

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
        int[] faceIndices0 = new int[4] { 0, 1, 2, 3 };
        int[] faceIndices1 = new int[4] { 7, 6, 5, 4 };
        int[] faceIndices2 = new int[4] { 11, 10, 9, 8 };
        int[] faceIndices3 = new int[4] { 15, 14, 13, 12 };
        int[] faceIndices4 = new int[4] { 16, 17, 18, 19 };
        int[] faceIndices5 = new int[4] { 23, 22, 21, 20 };
        int[] faceIndices6 = new int[4] { 27, 26, 25, 24 };
        int[] faceIndices7 = new int[3] { 28, 29, 30 };
        int[] faceIndices8 = new int[3] { 31, 32, 33 };
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
