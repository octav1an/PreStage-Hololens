using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

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
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (AirTapUp != null)
        {
            AirTapUp();
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (AirTapClick != null)
        {
            AirTapClick();
        }
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
