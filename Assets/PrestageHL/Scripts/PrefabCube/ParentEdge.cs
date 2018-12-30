using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentEdge : MonoBehaviour {

    public GameObject[] EDGE_COLL_GO
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
    public PREdge[] EDGE_COLL_COMP
    {
        get
        {
            PREdge[] coll = new PREdge[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                coll[i] = transform.GetChild(i).GetComponent<PREdge>();
            }
            return coll;
        }
    }

    public GameObject TempParentEdges;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DetroyEdges()
    {
        foreach (GameObject edge in EDGE_COLL_GO)
        {
            DestroyImmediate(edge);
        }
    }

    public void UnparentEdges()
    {
        // Create a tempParent
        TempParentEdges = new GameObject("tempParentEdges");
        TempParentEdges.SetActive(false);
        // Change parent
        foreach (GameObject edge in EDGE_COLL_GO)
        {
            edge.transform.parent = TempParentEdges.transform;
        }
    }

    public void ParentEdges()
    {
        // Parent back
        GameObject[] childColl = new GameObject[TempParentEdges.transform.childCount];
        for (int i = 0; i < childColl.Length; i++)
        {
            childColl[i] = TempParentEdges.transform.GetChild(i).gameObject;
        }
        foreach (GameObject edge in childColl)
        {
            edge.transform.parent = this.transform;
        }
        // Destroy tempParent
        Destroy(TempParentEdges);
    }
}
