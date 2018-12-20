using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoAxis : MonoBehaviour
{
    public Vector3 AxisX
    {
        get { return transform.right; }
    }
    public Vector3 AxisZ
    {
        get { return transform.forward; }
    }
    public Vector3 AxisY
    {
        get { return transform.up; }
    }

    // Use this for initialization
    void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
