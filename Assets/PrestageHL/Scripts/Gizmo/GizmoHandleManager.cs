using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GizmoHandleManager : MonoBehaviour
{

    private MeshRenderer renderer;
    private GameObject tempParent;
    private bool firstFrame = true;

    void Start () {
        renderer = GetComponent<MeshRenderer>();
    }
	
	void Update ()
	{
	    if (IsThisHit() && !Manager.Instance.GIZMO._isTransforming || 
	        Manager.Instance.GIZMO.TransformationHandleName == gameObject.name)
	    {
	        if (firstFrame)
	        {
	            firstFrame = false;
	            StartCoroutine(HighlightHandle());
	        }
	    }
	}

    IEnumerator HighlightHandle()
    {
        // Create the temporary parent.
        tempParent = new GameObject("tempObj");
        tempParent.transform.position = GetComponent<Collider>().bounds.center;
        tempParent.transform.rotation = Quaternion.identity;
        // Assign the parent for tempParent first, before scaling it to 1,1,1.
        tempParent.transform.parent = transform.parent;
        tempParent.transform.localScale = Vector3.one;
        // Parent this hit gameObject ot the temporary parent.
        gameObject.transform.parent = tempParent.transform;
        Color color = renderer.material.color;
        float scaleFactor = 1.3f;
        float scaleFactorNormal = 1f;
        while (IsThisHit() || Manager.Instance.GIZMO.TransformationHandleName == gameObject.name)
        {
            renderer.material.color = new Color(color.r, color.g, color.b, 1f);
            // Scale the handle
            tempParent.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            yield return null;
        }
        // Reset back to normal.
        renderer.material.color = new Color(color.r, color.g, color.b, 0.3f);
        tempParent.transform.localScale = new Vector3(scaleFactorNormal, scaleFactorNormal, scaleFactorNormal);

        // Unparent and destroy temporary parent.
        gameObject.transform.parent = tempParent.transform.parent;
        Destroy(tempParent);
        firstFrame = true;
    }

    private bool IsThisHit()
    {
        if(Manager.Instance.IS_HIT && Manager.Instance.GET_COLLIDER_NAME == gameObject.name)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
