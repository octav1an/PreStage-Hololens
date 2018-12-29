using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScaler : MonoBehaviour
{

    public GameObject ScalerCanvasPrefab;
    public GameObject ScalerCanvasGo;


	void Start () {
		
	}
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.L))
	    {
	        Destroy(gameObject);
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
        Destroy(ScalerCanvasGo);
    }


}
