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
	    if (Input.GetKeyDown(KeyCode.G))
	    {
	        float scale = 0.05f;

	        transform.localScale += new Vector3(scale, scale, scale);
	    }
    }

    void OnEnable()
    {
        ScalerCanvasGo = (GameObject)Instantiate(ScalerCanvasPrefab, Vector3.zero, Quaternion.identity);
        ScalerCanvasGo.GetComponent<ScalerCanvasPR>().SceneScalerGo = this.gameObject;
    }

    void OnDestroy()
    {
        Destroy(ScalerCanvasGo);
    }


}
