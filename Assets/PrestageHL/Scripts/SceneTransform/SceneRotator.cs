using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRotator : MonoBehaviour {

    public GameObject RotatorCanvasPrefab;
    public GameObject RotatorCanvasGo;
    public float rotationMag;
    private Quaternion savedQua;

    void Start () {
		
	}
	
	void Update ()
	{
	    OneHandRotateScene();
	}

    void OnEnable()
    {
        EventManager.AirTapDown += OnInputDownLocal;
        EventManager.AirTapUp += OnInputUpLocal;

        RotatorCanvasGo = (GameObject)Instantiate(RotatorCanvasPrefab, Vector3.zero, Quaternion.identity);
        RotatorCanvasGo.GetComponent<RotatorCanvasPR>().SceneRotatorGo = this.gameObject;
    }

    void OnDestroy()
    {
        EventManager.AirTapDown -= OnInputDownLocal;
        EventManager.AirTapUp -= OnInputUpLocal;

        RotatorCanvasGo.GetComponent<RotatorCanvasPR>().RotatorTurnOff();
        Destroy(RotatorCanvasGo);
    }

    #region Events
    private void OnInputDownLocal()
    {
        savedQua = transform.rotation;
        print(savedQua.eulerAngles);
    }

    private void OnInputUpLocal()
    {

    }
    #endregion // Events

    private void OneHandRotateScene()
    {
        if (Manager.Instance.EVENT_MANAGER.EventDataManipulation != null)
        {
            //Project on the camera.right
            Vector3 projected = Vector3.Project(Manager.Instance.EVENT_MANAGER.EventDataManipulation.CumulativeDelta,
                Camera.main.transform.right);
            if (Mathf.Abs(Vector3.Angle(projected, Camera.main.transform.right) - 180) < 0.1f)
            {
                float angle = ExtensionMethods.Remap(projected.magnitude, 0, 1, 0, 180);
                transform.rotation = Quaternion.Euler(savedQua.eulerAngles + new Vector3(0, angle, 0));
            }
            else
            {
                float angle = ExtensionMethods.Remap(projected.magnitude, 0, 1, 0, -180);
                transform.rotation = Quaternion.Euler(savedQua.eulerAngles + new Vector3(0, angle, 0));
            }
        }
    }
}
