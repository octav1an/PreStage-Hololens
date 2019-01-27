using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentVertex : MonoBehaviour {

    public GameObject[] VERTEX_COLL_GO
    {
        get
        {
            GameObject[] coll = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                coll[i] = transform.GetChild(i).gameObject;
            }
            return coll;
        }
    }
    public PRVertex[] GEO_VERTEX_COLL_CO
    {
        get
        {
            PRVertex[] coll = new PRVertex[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                coll[i] = transform.GetChild(i).GetComponent<PRVertex>();
            }
            return coll;
        }
    }

    public GameObject TempParentVertices;

    // Use this for initialization
    void Start () {
	    foreach (var vert in VERTEX_COLL_GO)
	    {
	        //print(vert.name);
	    }
	    //print(VERTEX_COLL_GO.Length);
	}
	
	// Update is called once per frame
	void Update () {
	    //print(VERTEX_COLL_GO.Length);
    }

    public void UnparentVertices()
    {
        // Create a tempParent
        TempParentVertices = new GameObject("tempParentVertices");
        TempParentVertices.SetActive(false);
        // Change parent
        foreach (GameObject vertex in VERTEX_COLL_GO)
        {
            vertex.transform.parent = TempParentVertices.transform;
        }
    }

    public void ParentVertices()
    {
        // Parent back
        GameObject[] childColl = new GameObject[TempParentVertices.transform.childCount];
        for (int i = 0; i < childColl.Length; i++)
        {
            childColl[i] = TempParentVertices.transform.GetChild(i).gameObject;
        }
        foreach (GameObject vertex in childColl)
        {
            vertex.transform.parent = this.transform;
        }
        // Destroy tempParent
        Destroy(TempParentVertices);
    }
}
