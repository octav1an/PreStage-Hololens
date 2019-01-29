using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalerCanvasPR : MonoBehaviour
{
    public GameObject SceneScalerGo;
    private BoxCollider _boxCollider;
    private readonly Vector3 _offsetAddition = new Vector3(0, 0.1f, 0);
    private float ScaleMagnitude = 0.01f;

    #region Unity
    void Start ()
	{
	    _boxCollider = SceneScalerGo.GetComponent<BoxCollider>();
	}
	
	void Update ()
	{
	    OrientCanvasToCamera();
        UpdateButtonPosition();
	    // Scane Main menu in relation to distance from camera.
	    Manager.Instance.ScaleToDistance(gameObject, ScaleMagnitude);
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

    /// <summary>
    /// Set exact scale from predefined ones.
    /// </summary>
    /// <param name="desiredScale"> Index of the scale from the drop-down menu. </param>
    public void SetFixedScale(int desiredScale)
    {
        switch (desiredScale)
        {
            case 0:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(5);
                break;
            case 1:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(10);
                break;
            case 2:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(20);
                break;
            case 3:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(50);
                break;
            case 4:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(100);
                break;
            case 5:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(200);
                break;
            case 6:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(500);
                break;
            case 7:
                SceneScalerGo.GetComponent<SceneScaler>().SetExactScaleFormula(1000);
                break;
            case 8:
                transform.Find("T_Actuale_ScaleNr").GetComponent<Text>().text = "Set Exact Scale";
                break;
        }
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
