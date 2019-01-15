using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMPSettings : MonoBehaviour
{

    public bool IsActive = false;

    private readonly string _appearanceD = "B_Appearance";
    private readonly string _appearanceA = "B_AppearanceActive";
    private readonly string _gizmoD = "B_Gizmo";
    private readonly string _gizmoA = "B_GizmoActive";
    private readonly string _snapD = "B_Snap";
    private readonly string _snapA = "B_SnapActive";
    private readonly string _otherD = "B_Other";
    private readonly string _otherA = "B_OtherActive";

    #region Unity
    void Start () {
        // Make sure that the setting panel elements are off.
	    DeactivatePanelElements();
	}
	
	void Update () {
		
	}
    #endregion // Unity


    #region MenuCallFunctions
    public void ActivateAppearance()
    {
        // Deactivate the other stuff
        DeactivateAllButtons();
        // Activate the Large Appearance button
        transform.Find(_appearanceA).gameObject.SetActive(true);
    }

    public void ActivateGizmo()
    {
        // Deactivate the other stuff
        DeactivateAllButtons();
        // Activate the Large button
        transform.Find(_gizmoA).gameObject.SetActive(true);
    }

    public void ActivateSnap()
    {
        // Deactivate the other stuff
        DeactivateAllButtons();
        // Activate the Large button
        transform.Find(_snapA).gameObject.SetActive(true);
    }

    public void ActivateOther()
    {
        // Deactivate the other stuff
        DeactivateAllButtons();
        // Activate the Large button
        transform.Find(_otherA).gameObject.SetActive(true);
    }
    #endregion // MenuCallFunctions


    #region Other
    /// <summary>
    /// Deactivate all 'Active' buttons, used to easily avoid multiple activation of the setting's tabs.
    /// </summary>
    private void DeactivateAllButtons()
    {
        transform.Find(_appearanceA).gameObject.SetActive(false);
        transform.Find(_gizmoA).gameObject.SetActive(false);
        transform.Find(_snapA).gameObject.SetActive(false);
        transform.Find(_otherA).gameObject.SetActive(false);
    }

    public void TogglePanelElements()
    {
        DeactivateAllButtons();
        IsActive = !IsActive;
        transform.Find("BG").gameObject.SetActive(IsActive);
        transform.Find(_appearanceD).gameObject.SetActive(IsActive);
        transform.Find(_appearanceA).gameObject.SetActive(IsActive);
        transform.Find(_gizmoD).gameObject.SetActive(IsActive);
        transform.Find(_snapD).gameObject.SetActive(IsActive);
        transform.Find(_otherD).gameObject.SetActive(IsActive);
    }

    private void DeactivatePanelElements()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
    }
    #endregion // Other
}
