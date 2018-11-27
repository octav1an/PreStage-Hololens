using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMSubmenu : MonoBehaviour
{
    public static CMSubmenu Instance;
    private string _activeButton;
    public bool IsAnySubmenuActive;

    #region Unity

    void Awake()
    {
        // Makes sure that I use always a game control even if my next scence already has one.
        // The instance of the object from the scene that is current will persist in the next scene.
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

	void Start ()
	{
	    IsAnySubmenuActive = false;
	}
	
	void Update () {
		
	}
    #endregion // Unity

    public void ActivateSubmenu(string buttonName)
    {
        Transform subMenuHolder = transform.Find(buttonName).Find("Submenu");
        if (subMenuHolder)
        {
            _activeButton = buttonName;
            IsAnySubmenuActive = true;
            for (int i = 0; i < subMenuHolder.childCount; i++)
            {
                GameObject child = subMenuHolder.GetChild(i).gameObject;
                child.SetActive(true);
            }
        }
        // Gray out the rest
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject buttonGo = transform.GetChild(i).gameObject;
            if (buttonGo.name != buttonName)
            {
                // Change button gray opacity to 50%
                Image imgBg = buttonGo.GetComponent<Image>();
                var tempColor = imgBg.color;
                tempColor.a = 0.5f;
                imgBg.color = tempColor;
                // Change button icon opacity to 50%
                Image imgIcon = buttonGo.transform.Find("Icon").GetComponent<Image>();
                var tempColorIcon = imgIcon.color;
                tempColorIcon.a = 0.5f;
                imgIcon.color = tempColorIcon;
                // Disable buttons functionality
                buttonGo.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void DeactivateSubmenu()
    {
        Transform subMenuHolder = transform.Find(_activeButton).Find("Submenu");
        if (subMenuHolder)
        {
            for (int i = 0; i < subMenuHolder.childCount; i++)
            {
                GameObject child = subMenuHolder.GetChild(i).gameObject;
                child.SetActive(false);
            }
        }
        
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject buttonGo = transform.GetChild(i).gameObject;
            if (buttonGo.name != _activeButton)
            {
                // Change button gray opacity to 100%
                Image imgBg = buttonGo.GetComponent<Image>();
                var tempColor = imgBg.color;
                tempColor.a = 1f;
                imgBg.color = tempColor;
                // Change button icon opacity to 100%
                Image imgIcon = buttonGo.transform.Find("Icon").GetComponent<Image>();
                var tempColorIcon = imgIcon.color;
                tempColorIcon.a = 1f;
                imgIcon.color = tempColorIcon;
                // Enable buttons functionality
                buttonGo.GetComponent<Button>().interactable = true;
            }
        }
        _activeButton = null;
        IsAnySubmenuActive = false;
    }
}
