using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScaler : MonoBehaviour
{

    public GameObject ScalerCanvasPrefab;
    public GameObject ScalerCanvasGo;
    public Vector3 SavedScale = Vector3.zero;

	void Start () {
	    if (SavedScale == Vector3.zero)
	    {
	        SavedScale = gameObject.transform.localScale;
	    }
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.L))
        {
            transform.localScale *= 2f;
        }
	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        transform.localScale *= 0.8f;
	    }
	    if (Input.GetKeyDown(KeyCode.J))
	    {
	        SetExactScale();

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

    private void CalculateScaleDiff()
    {
        float scaleDiff = transform.localScale.x / SavedScale.x;
        // Record the scale diff in Manager.
        Manager.Instance.ScaleDiff = scaleDiff;
        Manager.Instance.newScaleR = 1 / scaleDiff * Manager.Instance.oldScaleR;
        Manager.Instance.oldScaleR = Manager.Instance.newScaleR;
    }

    private void SetExactScale()
    {
        // factor = oldScaleR/newScaleR
        float factor = Manager.Instance.oldScaleR / 100;
        transform.localScale *= factor;
    }

}
