using System.Collections;
using System.Collections.Generic;
using RuntimeGizmos;
using UnityEngine;

public class GizmoObject : MonoBehaviour
{
    private GameObject tempCursor;
    public float scaleMagnitude = 0.0125f;

    #region Unity

    void Awake()
    {
        tempCursor = transform.Find("tempCursor").gameObject;
        tempCursor.SetActive(false);
    }

    void Start ()
    {
        
    }
	
	void Update ()
	{
        // Scales the Gizmo in relation to camera distance, making it the same size.
		Manager.Instance.ScaleToDistance(gameObject, scaleMagnitude);
	    ActivateTempCursor();
	}
    #endregion Unity

    private void ActivateTempCursor()
    {
        if (Manager.Instance.IS_HIT)
        {
            if (Manager.Instance.GET_COLLIDER_TAG == "GizmoMove" ||
                Manager.Instance.GET_COLLIDER_TAG == "GizmoPlaneMove" ||
                Manager.Instance.GET_COLLIDER_TAG == "GizmoRotate" ||
                Manager.Instance.GET_COLLIDER_TAG == "GizmoScale" ||
                Manager.Instance.GET_COLLIDER_TAG == "GizmoScaleCenter")
            {
                tempCursor.transform.position = Manager.Instance.GET_HIT_LOCATION;
                tempCursor.SetActive(true);
            }
            else
            {
                tempCursor.transform.position = Vector3.zero;
                tempCursor.SetActive(false);
            }
        }
        else
        {
            tempCursor.SetActive(false);
        }
    }

    #region UpdateGizmoStatus
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
    #endregion // UpdateGizmoStatus
}
