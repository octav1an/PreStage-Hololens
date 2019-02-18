using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRPrefab4Geo : PRGeo
{

    #region Unity
    protected override void Awake()
    {
        base.Awake();
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
        mesh.subMeshCount = 16;
        // Define mesh vertices.
        Vector3 v0 = new Vector3(-0.431993f, -0.5f, 0.431993f);
        Vector3 v1 = new Vector3(0.431993f, -0.5f, 0.431993f);
        Vector3 v2 = new Vector3(0.431993f, -0.5f, -0.431993f);
        Vector3 v3 = new Vector3(-0.431993f, -0.5f, -0.431993f);
        Vector3 v4 = new Vector3(-1.0f, -0.5f, -1.0f);
        Vector3 v5 = new Vector3(-1.0f, -0.5f, 1.0f);
        Vector3 v6 = new Vector3(1.0f, -0.5f, 1.0f);
        Vector3 v7 = new Vector3(1.0f, -0.5f, -1.0f);
        Vector3 v8 = new Vector3(-0.431993f, 0.5f, -0.431993f);
        Vector3 v9 = new Vector3(-0.431993f, 0.5f, 0.431993f);
        Vector3 v10 = new Vector3(0.431993f, 0.5f, 0.431993f);
        Vector3 v11 = new Vector3(0.431993f, 0.5f, -0.431993f);
        Vector3 v12 = new Vector3(1.0f, 0.5f, -1.0f);
        Vector3 v13 = new Vector3(-1.0f, 0.5f, -1.0f);
        Vector3 v14 = new Vector3(-1.0f, 0.5f, 1.0f);
        Vector3 v15 = new Vector3(1.0f, 0.5f, 1.0f);

        // Make the faces array vertices.
        Vector3[] face0 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v5),
            transform.TransformVector(v6),
            transform.TransformVector(v1)
        };
        Vector3[] face1 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v5),
            transform.TransformVector(v4),
            transform.TransformVector(v3)
        };
        Vector3[] face2 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v4),
            transform.TransformVector(v7),
            transform.TransformVector(v2)
        };
        Vector3[] face3 = new Vector3[4]
        {
            transform.TransformVector(v6),
            transform.TransformVector(v1),
            transform.TransformVector(v2),
            transform.TransformVector(v7)
        };
        Vector3[] face4 = new Vector3[4]
        {
            transform.TransformVector(v5),
            transform.TransformVector(v6),
            transform.TransformVector(v15),
            transform.TransformVector(v14)
        };
        Vector3[] face5 = new Vector3[4]
        {
            transform.TransformVector(v5),
            transform.TransformVector(v4),
            transform.TransformVector(v13),
            transform.TransformVector(v14)
        };
        Vector3[] face6 = new Vector3[4]
        {
            transform.TransformVector(v6),
            transform.TransformVector(v7),
            transform.TransformVector(v12),
            transform.TransformVector(v15)
        };
        Vector3[] face7 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v1),
            transform.TransformVector(v10),
            transform.TransformVector(v9)
        };
        Vector3[] face8 = new Vector3[4]
        {
            transform.TransformVector(v14),
            transform.TransformVector(v13),
            transform.TransformVector(v8),
            transform.TransformVector(v9)
        };
        Vector3[] face9 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v3),
            transform.TransformVector(v8),
            transform.TransformVector(v9)
        };
        Vector3[] face10 = new Vector3[4]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v11),
            transform.TransformVector(v8)
        };
        Vector3[] face11 = new Vector3[4]
        {
            transform.TransformVector(v1),
            transform.TransformVector(v2),
            transform.TransformVector(v11),
            transform.TransformVector(v10)
        };
        Vector3[] face12 = new Vector3[4]
        {
            transform.TransformVector(v15),
            transform.TransformVector(v12),
            transform.TransformVector(v11),
            transform.TransformVector(v10)
        };
        Vector3[] face13 = new Vector3[4]
        {
            transform.TransformVector(v15),
            transform.TransformVector(v14),
            transform.TransformVector(v9),
            transform.TransformVector(v10)
        };
        Vector3[] face14 = new Vector3[4]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v7),
            transform.TransformVector(v12),
            transform.TransformVector(v13)
        };
        Vector3[] face15 = new Vector3[4]
        {
            transform.TransformVector(v13),
            transform.TransformVector(v12),
            transform.TransformVector(v11),
            transform.TransformVector(v8)
        };


        // Create FaceIndices arrays.
        // TODO: Here is the problem
        int[] faceIndices0 = new int[4] { 3, 2, 1, 0 };
        int[] faceIndices1 = new int[4] { 4, 5, 6, 7 };
        int[] faceIndices2 = new int[4] { 8, 9, 10, 11 };
        int[] faceIndices3 = new int[4] { 12, 13, 14, 15 };
        int[] faceIndices4 = new int[4] { 16, 17, 18, 19 };
        int[] faceIndices5 = new int[4] { 23, 22, 21, 20 };
        int[] faceIndices6 = new int[4] { 24, 25, 26, 27 };
        int[] faceIndices7 = new int[4] { 31, 30, 29, 28 };
        int[] faceIndices8 = new int[4] { 35, 34, 33, 32 };
        int[] faceIndices9 = new int[4] { 36, 37, 38, 39 };
        int[] faceIndices10 = new int[4] { 40, 41, 42, 43 };
        int[] faceIndices11 = new int[4] { 47, 46, 45, 44 };
        int[] faceIndices12 = new int[4] { 48, 49, 50, 51 };
        int[] faceIndices13 = new int[4] { 55, 54, 53, 52 };
        int[] faceIndices14 = new int[4] { 59, 58, 57, 56 };
        int[] faceIndices15 = new int[4] { 63, 62, 61, 60 };

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
        list.AddRange(face9);
        list.AddRange(face10);
        list.AddRange(face11);
        list.AddRange(face12);
        list.AddRange(face13);
        list.AddRange(face14);
        list.AddRange(face15);
        mesh.vertices = list.ToArray();
        // Assign indexes for each quad.
        mesh.SetIndices(faceIndices0, MeshTopology.Quads, 0);
        mesh.SetIndices(faceIndices1, MeshTopology.Quads, 1);
        mesh.SetIndices(faceIndices2, MeshTopology.Quads, 2);
        mesh.SetIndices(faceIndices3, MeshTopology.Quads, 3);
        mesh.SetIndices(faceIndices4, MeshTopology.Quads, 4);
        mesh.SetIndices(faceIndices5, MeshTopology.Quads, 5);
        mesh.SetIndices(faceIndices6, MeshTopology.Quads, 6);
        mesh.SetIndices(faceIndices7, MeshTopology.Quads, 7);
        mesh.SetIndices(faceIndices8, MeshTopology.Quads, 8);
        mesh.SetIndices(faceIndices9, MeshTopology.Quads, 9);
        mesh.SetIndices(faceIndices10, MeshTopology.Quads, 10);
        mesh.SetIndices(faceIndices11, MeshTopology.Quads, 11);
        mesh.SetIndices(faceIndices12, MeshTopology.Quads, 12);
        mesh.SetIndices(faceIndices13, MeshTopology.Quads, 13);
        mesh.SetIndices(faceIndices14, MeshTopology.Quads, 14);
        mesh.SetIndices(faceIndices15, MeshTopology.Quads, 15);
        // Recalculate all
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
    #endregion //Generate
}
