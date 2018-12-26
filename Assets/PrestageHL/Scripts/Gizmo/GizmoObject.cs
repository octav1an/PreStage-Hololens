using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoObject : MonoBehaviour
{

    public float scaleMagnitude = 0.0125f;

    #region Unity
    void Start () {
		
	}
	
	void Update ()
	{
        // Scales the Gizmo in relation to camera distance, making it the same size.
		Manager.Instance.ScaleToDistance(gameObject, scaleMagnitude);
	}
    #endregion Unity
}
