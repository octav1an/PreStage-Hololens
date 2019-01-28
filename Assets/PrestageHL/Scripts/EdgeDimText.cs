using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDimText : MonoBehaviour
{
    public PRGeo ParentGeo;
    public TextMesh TextMeshCO;
    public PREdge EdgeParent;
    private RaycastHit hit;

    #region Unity
    void Start ()
	{
	    TextMeshCO = GetComponent<TextMesh>();
	}
	
	void Update () {
	    OrientCanvasToCamera();
	    UpdatePosition();
	    UpdateDimention();

	    DisableOccluded();
    }
    #endregion // Unity

    #region UpdateElements

    private void UpdateDimention()
    {
        Vector3 v0 = ParentGeo.GeoMesh.vertices[EdgeParent.EdgeHolder.V0Index];
        Vector3 v1 = ParentGeo.GeoMesh.vertices[EdgeParent.EdgeHolder.V1Index];
        TextMeshCO.text = (ParentGeo.transform.TransformPoint(v0) - ParentGeo.transform.TransformPoint(v1)).magnitude.ToString("F2");
    }

    private void UpdatePosition()
    {
        //transform.position = transform.parent.transform.TransformPoint(EdgeParent.EdgeHolder.MidPos);
        Vector3 v0 = ParentGeo.GeoMesh.vertices[EdgeParent.EdgeHolder.V0Index];
        Vector3 v1 = ParentGeo.GeoMesh.vertices[EdgeParent.EdgeHolder.V1Index];
        Vector3 midPos = ParentGeo.transform.TransformPoint((v0 + v1) / 2);
        transform.position = midPos;
    }

    /// <summary>
    /// Method that Orients the menu panel to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }
    #endregion // UpdateElements

    #region Other

    /// <summary>
    /// Disables edge dimentions that are occluded, either behind the its geometry or other geometry.
    /// </summary>
    private void DisableOccluded()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        Ray ray = new Ray(Camera.main.transform.position, dir);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "EdgeDim")
            {
                GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    #endregion // Other

}
