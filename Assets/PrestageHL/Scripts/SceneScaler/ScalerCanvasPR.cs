using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerCanvasPR : MonoBehaviour
{

    public GameObject SceneScalerGo;
    private BoxCollider _boxCollider;
    private readonly Vector3 _offsetAddition = new Vector3(0, 0.1f, 0);

    #region Unity
    void Start ()
	{
	    _boxCollider = SceneScalerGo.GetComponent<BoxCollider>();
	}
	
	void Update ()
	{
	    OrientCanvasToCamera();
        UpdateButtonPosition();
	}
    #endregion // Unity


    #region MenuCallFunctions
    public void ScalerTurnOff()
    {
        // Unparent scene geometry
        for (int i = 0; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            geo.transform.parent = null;
        }
        Destroy(SceneScalerGo);
    }
    #endregion //MenuCallFunctions

    #region UpdateElements
    void UpdateButtonPosition()
    {
        Vector3 verticalOffset = Vector3.Project(_boxCollider.size / 2, SceneScalerGo.transform.up) + _offsetAddition;
        transform.position = SceneScalerGo.transform.TransformPoint(_boxCollider.center + verticalOffset);
    }

    /// <summary>
    /// Method that orients the Contex Menu canvas to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }
    #endregion // UpdateElements
}
