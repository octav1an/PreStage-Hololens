using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ContexMenu : MonoBehaviour
{
    public Vector3 SavedHitPosition;
    public bool IsActive;
    // Double click fields.
    private int _tapCount = 0;
    private const float Delay = 0.5f;
    private float _timer;

    #region Unity
   
    void Start () {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
	
	void Update ()
	{
	    OrientCanvasToCamera();
        if (_tapCount != 0 && (Time.time - _timer) > Delay)
	    {
	        _tapCount = 0;
	    }
    }

    void OnEnable()
    {
        EventManager.AirTapClick += OnTap;
    }

    void OnDisable()
    {
        EventManager.AirTapClick -= OnTap;
    }
    #endregion //Unity

    #region Events

    void OnTap()
    {
        _tapCount++;
        if (_tapCount == 1)
        {
            _timer = Time.time;
            SavedHitPosition = Manager.Instance.HIT_LOCATION;
            //print("OneClick");
            // Deactivate if hit outside contex menu.
            DeactivateContexMenu();

        }
        else if (_tapCount == 2 && (Time.time - _timer) < Delay)
        {
            _tapCount = 0;
            //print("DoubleClick");
            if(!IsActive)ActivateContexMenu();
        }
        else
        {
            _tapCount = 0;
        }
    }

    #endregion //Events

    private void ActivateContexMenu()
    {
        float offset = 0.2f;
        Vector3 dir = Camera.main.transform.forward.normalized;
        this.transform.position = SavedHitPosition - dir * offset;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(true);
        }
        IsActive = true;
    }

    private void DeactivateContexMenu()
    {
        if (Manager.Instance.GET_COLLIDER_TAG == "ContexMenu" && CMSubmenu.Instance.IsAnySubmenuActive)
        {
            CMSubmenu.Instance.DeactivateSubmenu();
            return;
        }
        if (Manager.Instance.GET_COLLIDER_TAG == "ContexMenu" || Manager.Instance.GET_COLLIDER_TAG == "CMSubmenu")
        {
            return;
        }
        // Deactivate the submenu first.
        if (CMSubmenu.Instance.IsAnySubmenuActive)
        {
            CMSubmenu.Instance.DeactivateSubmenu();
        }
        // Deactivate the buttons of the menu.
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
        IsActive = false;
    }

    /// <summary>
    /// Method that orients the Contex Menu canvas to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }

}
