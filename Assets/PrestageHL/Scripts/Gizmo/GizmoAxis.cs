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

    void Draw(bool drawOn)
    {
        if (drawOn)
        {
            float lineLength = 0.1f;
            Vector3 center = transform.position;
            Debug.DrawLine(center, center + (transform.right * lineLength), Color.cyan);
            Debug.DrawLine(center, center + (transform.up * lineLength), Color.magenta);
            Debug.DrawLine(center, center + (-transform.forward * lineLength), Color.yellow);
        }
    }
}
