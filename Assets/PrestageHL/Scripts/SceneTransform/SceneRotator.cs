using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRotator : MonoBehaviour {

    public GameObject RotatorCanvasPrefab;
    public GameObject RotatorCanvasGo;

    void Start () {
		
	}
	
	void Update () {
		
	}

    void OnEnable()
    {
        RotatorCanvasGo = (GameObject)Instantiate(RotatorCanvasPrefab, Vector3.zero, Quaternion.identity);
        RotatorCanvasGo.GetComponent<RotatorCanvasPR>().SceneRotatorGo = this.gameObject;
    }

    void OnDestroy()
    {
        RotatorCanvasGo.GetComponent<RotatorCanvasPR>().RotatorTurnOff();
        Destroy(RotatorCanvasGo);
    }
}
