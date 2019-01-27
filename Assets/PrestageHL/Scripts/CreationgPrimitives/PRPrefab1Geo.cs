using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRPrefab1Geo : PRGeo
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
        mesh.subMeshCount = 6;
        // Define mesh vertices.
        Vector3 v0 = new Vector3(0.1f, 1.6f, -0.1f);
        Vector3 v1 = new Vector3(0.1f, 1.6f, 0.1f);
        Vector3 v2 = new Vector3(-0.1f, 1.6f, 0.1f);
        Vector3 v3 = new Vector3(-0.1f, 1.6f, -0.1f);
        Vector3 v4 = new Vector3(0.1f, -1.6f, -0.1f);
        Vector3 v5 = new Vector3(0.1f, -1.6f, 0.1f);
        Vector3 v6 = new Vector3(-0.1f, -1.6f, 0.1f);
        Vector3 v7 = new Vector3(-0.1f, -1.6f, -0.1f);


        // Make the faces array vertices.
        Vector3[] face0 = new Vector3[4]
        {
            transform.TransformVector(v7),
            transform.TransformVector(v4),
            transform.TransformVector(v0),
            transform.TransformVector(v3)
        };
        Vector3[] face1 = new Vector3[4]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v0),
            transform.TransformVector(v1),
            transform.TransformVector(v5)
        };
        Vector3[] face2 = new Vector3[4]
        {
            transform.TransformVector(v5),
            transform.TransformVector(v1),
            transform.TransformVector(v2),
            transform.TransformVector(v6)
        };
        Vector3[] face3 = new Vector3[4]
        {
            transform.TransformVector(v7),
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v6)
        };
        Vector3[] face4 = new Vector3[4]
        {
            transform.TransformVector(v7),
            transform.TransformVector(v4),
            transform.TransformVector(v5),
            transform.TransformVector(v6)
        };
        Vector3[] face5 = new Vector3[4]
        {
            transform.TransformVector(v0),
            transform.TransformVector(v3),
            transform.TransformVector(v2),
            transform.TransformVector(v1)
        };


        // Create FaceIndices arrays.
        // TODO: Here is the problem
        int[] faceIndices0 = new int[4] { 3, 2, 1, 0 };
        int[] faceIndices1 = new int[4] { 4, 5, 6, 7 };
        int[] faceIndices2 = new int[4] { 8, 9, 10, 11 };
        int[] faceIndices3 = new int[4] { 15, 14, 13, 12 };
        int[] faceIndices4 = new int[4] { 16, 17, 18, 19 };
        int[] faceIndices5 = new int[4] { 20, 21, 22, 23 };

        // Join the array with vertices.
        var list = new List<Vector3>();
        list.AddRange(face0);
        list.AddRange(face1);
        list.AddRange(face2);
        list.AddRange(face3);
        list.AddRange(face4);
        list.AddRange(face5);
        mesh.vertices = list.ToArray();
        // Assign indexes for each quad.
        mesh.SetIndices(faceIndices0, MeshTopology.Quads, 0);
        mesh.SetIndices(faceIndices1, MeshTopology.Quads, 1);
        mesh.SetIndices(faceIndices2, MeshTopology.Quads, 2);
        mesh.SetIndices(faceIndices3, MeshTopology.Quads, 3);
        mesh.SetIndices(faceIndices4, MeshTopology.Quads, 4);
        mesh.SetIndices(faceIndices5, MeshTopology.Quads, 5);
        // Recalculate all
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
    #endregion //Generate
}
