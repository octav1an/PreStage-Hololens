using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MMExpandCollider : MonoBehaviour
{

    public GameObject ButtonObject;
    public GameObject ButtonText;
    private Vector3 _savedMeshScale;
    private Vector3 _savedTextScale;

#region Unity
    void Start ()
    {
        _savedMeshScale = ButtonObject.transform.localScale;
        _savedTextScale = ButtonText.transform.localScale;

    }
	
	void Update () {
	    if (Manager.Instance.IS_HIT && Manager.Instance.GET_COLLIDER_ID == GetComponent<Collider>().GetInstanceID())
	    {
	        float scaleFactor = 1.3f;
	        Vector3 newMeshScale = new Vector3(_savedMeshScale.x * scaleFactor, _savedMeshScale.y * scaleFactor, _savedMeshScale.z);
	        Vector3 newTextScale = new Vector3(_savedTextScale.x * scaleFactor, _savedTextScale.y * scaleFactor, _savedTextScale.z);
            ButtonObject.transform.localScale = newMeshScale;
	        ButtonText.transform.localScale = newTextScale;
	    }
	    else
	    {
	        ButtonObject.transform.localScale = _savedMeshScale;
	        ButtonText.transform.localScale = _savedTextScale;
        }
	}
#endregion // Unity
}
