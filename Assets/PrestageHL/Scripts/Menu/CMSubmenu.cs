using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMSubmenu : MonoBehaviour
{
    public static CMSubmenu Instance;
    public string ActiveButton;
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

    public void Print()
    {
        Debug.Log("DoubleCkickShit");
    }

    public void ActivateSubmenu(string buttonName)
    {
        Transform subMenuHolder = transform.Find(buttonName).Find("Submenu");
        if (subMenuHolder)
        {
            ActiveButton = buttonName;
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
                ChangeButtonActiveState(buttonGo, false);
            }
            else
            {
                // Disable text for the pressed button.
                buttonGo.transform.Find("Text").gameObject.SetActive(false);
            }
        }
    }

    public void DeactivateSubmenu()
    {
        Transform subMenuHolder = transform.Find(ActiveButton).Find("Submenu");
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
            if (buttonGo.name != ActiveButton)
            {
                ChangeButtonActiveState(buttonGo, true);
            }
            else
            {
                // Enable text for the pressed button.
                buttonGo.transform.Find("Text").gameObject.SetActive(true);
            }
        }
        ActiveButton = null;
        IsAnySubmenuActive = false;
    }

    /// <summary>
    /// Grays out or makes the button active, incluting text On/Off and opacity change for button's background and icon.
    /// </summary>
    private void ChangeButtonActiveState(GameObject buttonGo, bool active)
    {
        float opacity = 1f;
        // Change the values for opacity to gray out.
        if (!active)
        {
            opacity = 0.5f;
        }
        // Change button gray opacity.
        Image imgBg = buttonGo.GetComponent<Image>();
        var tempColor = imgBg.color;
        tempColor.a = opacity;
        imgBg.color = tempColor;

        // Change button icon opacity.
        Image imgIcon = buttonGo.transform.Find("Icon").GetComponent<Image>();
        var tempColorIcon = imgIcon.color;
        tempColorIcon.a = opacity;
        imgIcon.color = tempColorIcon;

        // Change text status.
        GameObject textGo = buttonGo.transform.Find("Text").gameObject;
        textGo.SetActive(active);

        // Change buttons functionality.
        buttonGo.GetComponent<Button>().interactable = active;
    }
}
