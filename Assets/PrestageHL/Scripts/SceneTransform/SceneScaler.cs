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

    #region Unity
    void Start () {
	    if (SavedScale == Vector3.zero)
	    {
	        SavedScale = gameObject.transform.localScale;
	    }

        DynamicScaleGO = ScalerCanvasGo.transform.Find("T_Actuale_ScaleNr").gameObject;
    }
	
	void Update ()
	{
	    CalculateScaleDynamicaly();
        if (Input.GetKeyDown(KeyCode.L))
        {
            transform.localScale *= 2f;
        }
	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        transform.localScale *= 0.8f;
	    }
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
        // Record the scale diff in Manager.
        Manager.Instance.ScaleDiff = scaleDiff;
        Manager.Instance.newScaleR = 1 / scaleDiff * Manager.Instance.oldScaleR;
        Manager.Instance.oldScaleR = Manager.Instance.newScaleR;
    }

    /// <summary>
    /// Calculate and display the scale dynamicaly while scaling with two hands.
    /// </summary>
    private void CalculateScaleDynamicaly()
    {
        float scaleDiff = transform.localScale.x / SavedScale.x;
        Manager.Instance.ScaleDiff = scaleDiff;
        float scaleDymanic = 1 / scaleDiff * Manager.Instance.oldScaleR;
        DynamicScaleGO.GetComponent<Text>().text = scaleDymanic.ToString("F0");
    }

    public void SetExactScaleFormula(int desiredScale)
    {
        // factor = oldScaleR/newScaleR
        float factor = Manager.Instance.oldScaleR / desiredScale;
        transform.localScale *= factor;
    }
}
