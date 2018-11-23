using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class HandMove : MonoBehaviour, IManipulationHandler
{
    public GameObject obj;
    public Vector3 savedPos;
    public Quaternion qua;

    // Use this for initialization
    void Start () {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        print("OnManipStarted");

        savedPos = obj.transform.position;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        //print("OnManipUpdated");

        print(eventData.InputSource.TryGetGripRotation(eventData.SourceId, out qua));
        print(qua);
        obj.transform.position = savedPos + eventData.CumulativeDelta * 3;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        print("OnManipCompleted");
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        print("OnManipCanceled");
    }
}
