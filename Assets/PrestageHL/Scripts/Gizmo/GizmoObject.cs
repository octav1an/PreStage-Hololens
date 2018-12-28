using System.Collections;
using System.Collections.Generic;
using RuntimeGizmos;
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

    public void ActivateScaleGizmo()
    {
        // First deactivate any gizmo type.
        DeactivateAllGizmo();
        transform.GetChild(1).gameObject.SetActive(true);
        Manager.Instance.GIZMO.type = TransformType.Scale;
    }

    public void ActivateDefaultGizmo()
    {
        // First deactivate any gizmo type.
        DeactivateAllGizmo();
        transform.GetChild(0).gameObject.SetActive(true);
        Manager.Instance.GIZMO.type = TransformType.Move;
    }

    private void DeactivateAllGizmo()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
