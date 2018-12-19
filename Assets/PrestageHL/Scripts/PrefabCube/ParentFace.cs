using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentFace : MonoBehaviour {

    public GameObject[] FACE_COLL_GO
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
    public PRFace[] FACE_COLL_COMP
    {
        get
        {
            PRFace[] coll = new PRFace[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                coll[i] = transform.GetChild(i).GetComponent<PRFace>();
            }
            return coll;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
