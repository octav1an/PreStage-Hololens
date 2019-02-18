using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherGeo : PRGeo
{

    void Awake()
    {
        GeoMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshCollider>().sharedMesh = GeoMesh;
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
