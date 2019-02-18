using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMHelp : MonoBehaviour {

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
            MainMenu.Instance.transform.Find("B_File").transform.Find("SubButtons").gameObject.SetActive(false);
            MainMenu.Instance.transform.Find("B_File").GetComponent<MMFile>().IsActive = false;
            MainMenu.Instance.SettingsPanel.GetComponent<MMPSettings>().DeactivatePanelElements();
        }
        else
        {
            MainMenu.Instance.CloseAllSubmenus();
        }
    }
}
