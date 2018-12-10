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
    public PRVertex[] VERTEX_COLL_COMP
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
}
