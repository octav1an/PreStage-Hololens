using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeMesh : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
	    mf.mesh.subMeshCount = 6;

        CombineInstance[] meshColl = new CombineInstance[6];
	    for (int i = 0; i < transform.childCount; i++)
	    {
	        meshColl[i].mesh = transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
            meshColl[i].transform = transform.GetChild(i).GetComponent<MeshFilter>().transform.localToWorldMatrix;
	        transform.GetChild(i).gameObject.SetActive(false);
        }

        mf.mesh.CombineMeshes(meshColl, false);
        mf.mesh.RecalculateNormals();
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateTangents();
        print(mf.mesh.subMeshCount);
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
