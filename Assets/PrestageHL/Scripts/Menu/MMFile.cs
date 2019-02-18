using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MMFile : MonoBehaviour {

    public bool IsActive = false;

    void Start () {
		
	}
	
	void Update () {
		
	}

    public void SwitchSubButtons()
    {
        if (!IsActive)
        {
            GameObject obj = transform.Find("SubButtons").gameObject;
            obj.SetActive(!obj.activeInHierarchy);
            IsActive = true;

            // Deactivate the other functions in case they are already activated.
            MainMenu.Instance.SettingsPanel.GetComponent<MMPSettings>().DeactivatePanelElements();
            MainMenu.Instance.transform.Find("B_Help").transform.Find("SubButtons").gameObject.SetActive(false);
            MainMenu.Instance.transform.Find("B_Help").GetComponent<MMHelp>().IsActive = false;
        }
        else
        {
            MainMenu.Instance.CloseAllSubmenus();
        }
    }
}
