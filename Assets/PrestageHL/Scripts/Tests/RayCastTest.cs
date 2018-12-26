using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    private RaycastHit hit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update()
    {
        float mag = (Camera.main.transform.position - this.transform.position).magnitude;
        //transform.localScale = new Vector3(mag/80, mag / 80, mag / 80);

        // Check for a Wall.
        LayerMask mask = LayerMask.GetMask("Gizmo");

        // Check if a Wall is hit.
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20.0f, mask))
        {
            //Debug.Log(hit.collider.transform.parent.name);
        }
    
    }
}
