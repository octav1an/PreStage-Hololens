using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRPrefab5Geo : PRGeo
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
        mesh.subMeshCount = 5;
        // Define mesh vertices.
        Vector3 v0 = new Vector3(0.568555f, 0.426416f, -8.9498e-8f);
        Vector3 v1 = new Vector3(0.568555f, -0.426416f, 1.1383e-7f);
        Vector3 v2 = new Vector3(-0.284278f, -0.426416f, 0.5f);
        Vector3 v3 = new Vector3(-0.284278f, -0.426416f, -0.5f);
        Vector3 v4 = new Vector3(-0.284278f, 0.426416f, -0.5f);
        Vector3 v5 = new Vector3(-0.284278f, 0.426416f, 0.5f);


        // Make the faces array vertices.
        Vector3[] face0 = new Vector3[3]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v5),
            transform.TransformVector(v0)
        };
        Vector3[] face1 = new Vector3[3]
        {
            transform.TransformVector(v3),
            transform.TransformVector(v1),
            transform.TransformVector(v2)
        };
        Vector3[] face2 = new Vector3[4]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v5),
            transform.TransformVector(v2),
            transform.TransformVector(v3)
        };
        Vector3[] face3 = new Vector3[4]
        {
            transform.TransformVector(v4),
            transform.TransformVector(v0),
            transform.TransformVector(v1),
            transform.TransformVector(v3)
        };
        Vector3[] face4 = new Vector3[4]
        {
            transform.TransformVector(v5),
            transform.TransformVector(v0),
            transform.TransformVector(v1),
            transform.TransformVector(v2)
        };
        // Create FaceIndices arrays.
        // TODO: Here is the problem
        int[] faceIndices0 = new int[3] { 0, 1, 2 };
        int[] faceIndices1 = new int[3] { 3, 4, 5 };
        int[] faceIndices2 = new int[4] { 9, 8, 7, 6 };
        int[] faceIndices3 = new int[4] { 10, 11, 12, 13 };
        int[] faceIndices4 = new int[4] { 17, 16, 15, 14 };

        // Join the array with vertices.
        var list = new List<Vector3>();
        list.AddRange(face0);
        list.AddRange(face1);
        list.AddRange(face2);
        list.AddRange(face3);
        list.AddRange(face4);
        mesh.vertices = list.ToArray();
        // Assign indexes for each quad.
        mesh.SetIndices(faceIndices0, MeshTopology.Triangles, 0);
        mesh.SetIndices(faceIndices1, MeshTopology.Triangles, 1);
        mesh.SetIndices(faceIndices2, MeshTopology.Quads, 2);
        mesh.SetIndices(faceIndices3, MeshTopology.Quads, 3);
        mesh.SetIndices(faceIndices4, MeshTopology.Quads, 4);
        // Recalculate all
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
    #endregion //Generate
}
