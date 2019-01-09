using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMExpand : MonoBehaviour
{

    public bool isExpanded;
    public GameObject buttonMeshGO;

	void Start ()
	{
	    isExpanded = false;
	    ExpandToggle();

	}
	
	void Update () {
		
	}

    public void ExpandToggle()
    {
        for (int i = 1; i < transform.parent.childCount; i++)
        {
            GameObject obj = transform.parent.GetChild(i).gameObject;
            obj.SetActive(isExpanded);
        }

        isExpanded = !isExpanded;
    }
}
