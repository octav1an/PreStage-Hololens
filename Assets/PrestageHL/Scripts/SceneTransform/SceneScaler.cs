using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneScaler : MonoBehaviour
{

    public GameObject ScalerCanvasPrefab;
    public GameObject ScalerCanvasGo;
    public GameObject DynamicScaleGO;
    public Vector3 SavedScale = Vector3.zero;
    public float oldScale;

    #region Unity
    void Start () {
	    if (SavedScale == Vector3.zero)
	    {
	        SavedScale = gameObject.transform.localScale;
	    }
        oldScale = Manager.Instance.ScaleRatio;
        DynamicScaleGO = ScalerCanvasGo.transform.Find("T_Actuale_ScaleNr").gameObject;
    }
	
	void Update ()
	{
	    CalculateScaleDynamicaly();
     //   if (Input.GetKeyDown(KeyCode.L))
     //   {
     //       transform.localScale *= 2f;
     //   }
	    //if (Input.GetKeyDown(KeyCode.K))
	    //{
	    //    transform.localScale *= 0.8f;
	    //}
    }

    void OnEnable()
    {
        ScalerCanvasGo = (GameObject)Instantiate(ScalerCanvasPrefab, Vector3.zero, Quaternion.identity);
        ScalerCanvasGo.GetComponent<ScalerCanvasPR>().SceneScalerGo = this.gameObject;
    }

    void OnDestroy()
    {
        ScalerCanvasGo.GetComponent<ScalerCanvasPR>().ScalerTurnOff();
        CalculateScaleDiff();
        Destroy(ScalerCanvasGo);
    }
    #endregion // Unity

    /// <summary>
    /// Calculate and update the scale ratio in Manager.
    /// </summary>
    private void CalculateScaleDiff()
    {
        float scaleDiff = transform.localScale.x / SavedScale.x;
        // Update the scale in Manager.
        float newScale = 1 / scaleDiff * Manager.Instance.ScaleRatio;
        Manager.Instance.ScaleRatio = newScale;
    }

    /// <summary>
    /// Calculate and display the scale dynamicaly while scaling with two hands.
    /// </summary>
    private void CalculateScaleDynamicaly()
    {
        float scaleDiff = transform.localScale.x / SavedScale.x;
        float scaleDymanic = 1 / scaleDiff * oldScale;
        // Update the text in ScalerCanvas.
        DynamicScaleGO.GetComponent<Text>().text = scaleDymanic.ToString("F0");
    }

    public void SetExactScaleFormula(int desiredScale)
    {
        // factor = oldScaleR/newScaleR
        float factor = Manager.Instance.ScaleRatio / desiredScale;
        transform.localScale *= factor;
        Manager.Instance.ScaleRatio = desiredScale;
    }
}
