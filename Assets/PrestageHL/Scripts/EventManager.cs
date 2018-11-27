using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable
{
    public delegate void OnAirTapDown();
    public static event OnAirTapDown AirTapDown;

    public delegate void OnAirTapUp();
    public static event OnAirTapUp AirTapUp;

    public delegate void OnAirTapClick();
    public static event OnAirTapClick AirTapClick;

    public delegate void OnEnterFocus();
    public static event OnEnterFocus FocusEnter;

    public delegate void OnExitFocus();
    public static event OnExitFocus FocusExit;


    private void Start()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    private void Update()
    {
    }


    public void OnInputDown(InputEventData eventData)
    {
        if (AirTapDown != null)
        {
            AirTapDown();
        }

        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (AirTapUp != null)
        {
            AirTapUp();
        }
        eventData.Use();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {

        if (AirTapClick != null)
        {
            AirTapClick();
        }

        eventData.Use();
    }

    public void OnFocusEnter()
    {
        if (FocusEnter != null)
        {
            FocusEnter();
        }
    }

    public void OnFocusExit()
    {
        if (FocusExit != null)
        {
            FocusExit();
        }
    }
}
