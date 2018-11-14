using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class DragTest : MonoBehaviour, INavigationHandler, IManipulationHandler, ISelectHandler
{

    Vector3 saveLoc = new Vector3();
    public GameObject obj;

	// Use this for initialization
	void Start () {
		InputManager.Instance.AddGlobalListener(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnNavigationStarted(NavigationEventData eventData)
    {
        //print("OnNavStarted");
        saveLoc = transform.position;
    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
        //print("OnNavUpdated");
        //print(eventData.NormalizedOffset);
        //obj.transform.position = saveLoc + eventData.CumulativeDelta;
        this.transform.position = saveLoc + eventData.NormalizedOffset;
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
        //print("OnNavCompleted");
    }

    public void OnNavigationCanceled(NavigationEventData eventData)
    {
        //print("OnNavCanceled");
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //print("OnManipulationStarted");
        //saveLoc = transform.position;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        //print("OnManipulationUpdated");
        //obj.transform.position = saveLoc + eventData.CumulativeDelta;
        //print(eventData.CumulativeDelta);
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        //print("OnManipulationCompleted");
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        //print("OnManipulationCanceled");
    }

    public void OnSelectPressedAmountChanged(SelectPressedEventData eventData)
    {
        print("event");
    }
}
