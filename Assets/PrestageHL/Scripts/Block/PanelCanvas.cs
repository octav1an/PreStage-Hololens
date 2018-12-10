using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCanvas : MonoBehaviour {

    public PRCube CUBE_COMP
    {
        get
        {
            return transform.GetComponentInParent<PRCube>();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    OrientCanvasToCamera();
	    AlignCenterFace(CUBE_COMP.CENTER_GEOMETRICAL, 3f);

	}

    /// <summary>
    /// Method that Orients the panel canvas in every block to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }

    /// <summary>
    /// Align the panel canvas be the on the top of the block.
    /// </summary>
    /// <param name="vec">Face Y Positive local center.</param>
    /// <param name="offsetY">Offset on Y axis.</param>
    private void AlignCenterFace(Vector3 vec, float offsetY)
    {
        Vector3 finalPos = new Vector3(vec.x, vec.y + offsetY, vec.z);
        this.transform.localPosition = finalPos;
    }
}
