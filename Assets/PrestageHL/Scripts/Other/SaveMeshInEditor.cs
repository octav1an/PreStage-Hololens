#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Usage: Attach to gameobject, assign target gameobject (from where the mesh is taken), Run, Press savekey

public class SaveMeshInEditor : MonoBehaviour
{

    public KeyCode saveKey = KeyCode.F12;
    public string saveName = "SavedMesh";
    public Transform selectedGameObject;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = GenerateMesh();
    }

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveAsset();
        }
    }

    void SaveAsset()
    {
        var mf = selectedGameObject.GetComponent<MeshFilter>();
        if (mf)
        {
            var savePath = "Assets/" + saveName + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            AssetDatabase.CreateAsset(mf.mesh, savePath);
        }
    }

    public Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 9;
        // Define mesh vertices.
        Vector3 v0 = new Vector3(-0.5f, -0.5f, -0.633333f);
        Vector3 v1 = new Vector3(-0.5f, 0.5f, -0.633333f);
        Vector3 v2 = new Vector3(0.5f, 0.5f, -0.633333f);
        Vector3 v3 = new Vector3(0.5f, -0.5f, -0.633333f);
        Vector3 v4 = new Vector3(-0.5f, -0.5f, 0.366667f);
        Vector3 v5 = new Vector3(-0.5f, 0.5f, 0.366667f);
        Vector3 v6 = new Vector3(0.5f, 0.5f, 0.366667f);
        Vector3 v7 = new Vector3(0.5f, -0.5f, 0.366667f);
        Vector3 v8 = new Vector3(0.0f, -0.5f, 0.866667f);
        Vector3 v9 = new Vector3(0.0f, 0.5f, 0.866667f);
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
}
#endif
