using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour, IInputHandler, IInputClickHandler, IManipulationHandler, ISpeechHandler, INavigationHandler
{
    public delegate void OnAirTapDown();
    public static event OnAirTapDown AirTapDown;

    public delegate void OnAirTapUp();
    public static event OnAirTapUp AirTapUp;

    public delegate void OnAirTapClick();
    public static event OnAirTapClick AirTapClick;

    public delegate void OnManipulationStartedPR();
    public static event OnManipulationStartedPR ManipulationStarted;

    public delegate void OnManipulationUpdatedPR();
    public static event OnManipulationUpdatedPR ManipulationUpdated;

    public delegate void OnManipulationCompletedPR();
    public static event OnManipulationCompletedPR ManipulationCompleted;

    public delegate void OnManipulationCanceledPR();
    public static event OnManipulationCanceledPR ManipulationCanceled;

    public delegate void OnNavigationCanceledPR();
    public static event OnNavigationCanceledPR NavigationCanceled;

    public delegate void OnSpeechKeywordRecognizedPR();
    public static event OnSpeechKeywordRecognizedPR SpeechKeywordRecognized;

    // Variables
    public ManipulationEventData EventDataManipulation;
    public SpeechEventData EventDataSpeech;

    #region Unity
    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
    private void Update()
    {
    }
    #endregion //Unity

    public void OnInputDown(InputEventData eventData)
    {
        if (AirTapDown != null)
        {
            AirTapDown();
        }

        //eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (AirTapUp != null)
        {
            AirTapUp();
        }
        //eventData.Use();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        if (AirTapClick != null)
        {
            AirTapClick();
        }

        //eventData.Use();
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        if (ManipulationStarted != null)
        {
            ManipulationStarted();
        }

        EventDataManipulation = eventData;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (ManipulationUpdated != null)
        {
            ManipulationUpdated();
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        if (ManipulationCompleted != null)
        {
            ManipulationCompleted();
        }

        // Remove EventDataManipulation data.
        EventDataManipulation = null;
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        if (ManipulationCanceled != null)
        {
            ManipulationCanceled();
        }

        // Remove EventDataManipulation data.
        EventDataManipulation = null;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (SpeechKeywordRecognized != null)
        {
            EventDataSpeech = eventData;
            SpeechKeywordRecognized();
        }
    }

    public void OnNavigationStarted(NavigationEventData eventData)
    {

    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
        
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
        
    }

    // Use this event to check if the hand has left the tracking field.
    public void OnNavigationCanceled(NavigationEventData eventData)
    {
        if (NavigationCanceled != null)
        {
            NavigationCanceled();
        }
    }
}
