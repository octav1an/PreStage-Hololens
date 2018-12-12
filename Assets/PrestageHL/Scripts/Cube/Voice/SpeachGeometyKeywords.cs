using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SpeachGeometyKeywords : MonoBehaviour
{


    #region Unity
    private void Awake()
    {
    }

    private void OnDestroy()
    {

    }
    #endregion // Unity


    public void OnSpeechKeywordRecognizedLocal()
    {
        //ChangeTransformationMode(Manager.Instance.EVENT_MANAGER.EventDataSpeech.RecognizedText);
        //ChangeSelectionMode(Manager.Instance.EVENT_MANAGER.EventDataSpeech.RecognizedText);
    }

    public void ChangeTransformationMode(string mode)
    {
        switch (mode.ToLower())
        {
            case "move":
                // Set move mode
                ContexMenu.Instance.SetMoveTransformationType();
                break;
            case "scale":
                // Set Scale mode
                ContexMenu.Instance.SetScaleTransformationType();
                break;
            case "rotate":
                // Set Rotate mode
                ContexMenu.Instance.SetRotateTransformationType();
                break;
            case "grab":
                // Set Gran mode
                ContexMenu.Instance.SetGrabTransformationType();
                break;
        }
    }
    
    public void ChangeSelectionMode(string mode)
    {
        switch (mode.ToLower())
        {
            case "geometry mode":
                ContexMenu.Instance.SetGeometryMode();
                break;
            case "vertex mode":
                ContexMenu.Instance.SetVertexMode();
                break;
            case "edge mode":
                ContexMenu.Instance.SetEdgeMode();
                break;
            case "face mode":
                ContexMenu.Instance.SetFaceMode();
                break;
        }
    }

}
