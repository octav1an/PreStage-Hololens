using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTopo : MonoBehaviour {
    
   

	// Use this for initialization
	void Start ()
	{
	    MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
	    meshFilter.mesh.subMeshCount = 2;
	    float size = 0.1f;
	    Vector3[] vertColl_1 = new Vector3[4]
	    {
            new Vector3(0,0,0),
            new Vector3(0,size,0),
            new Vector3(size,size,0),
            new Vector3(size,0,0)
	    };
	    Vector3[] vertColl_2 = new Vector3[4]
	    {
	        new Vector3(0,0,size),
	        new Vector3(0,size,size),
	        new Vector3(0,size,0),
	        new Vector3(0,0,0)
	    };
        int[] triColl1 = new int[4] {0, 1, 2, 3};
	    int[] triColl2 = new int[4] { 4, 5, 6, 7 };
        meshFilter.mesh.vertices = vertColl_1.Concat(vertColl_2).ToArray();
        //meshFilter.mesh.SetVertices(vertColl_1.ToList());
	    //meshFilter.mesh.SetVertices(vertColl_2.ToList());
        meshFilter.mesh.SetIndices(triColl1, MeshTopology.Quads, 0);
        meshFilter.mesh.SetIndices(triColl2, MeshTopology.Quads, 1);
        //meshFilter.mesh.triangles = triColl;
	    meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateTangents();

	    //print("mesh01: " + meshFilter.mesh.GetIndices(0));
	    //print("mesh02: " + meshFilter.mesh.GetIndices(1));
	    for (int i = 0; i < meshFilter.mesh.GetTriangles(0).Length; i++)
	    {
            print("Index: " + i);
	        print("mesh01: " + meshFilter.mesh.GetTriangles(0)[i]);
	        print("mesh02: " + meshFilter.mesh.GetTriangles(1)[i]);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
