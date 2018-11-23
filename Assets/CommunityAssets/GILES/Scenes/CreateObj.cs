using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GILES;

public class CreateObj : MonoBehaviour
{

    public GameObject asset;

	// Use this for initialization
	void Start ()
	{
	    Instantiate();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Instantiate()
    {
        Camera cam = Camera.main;
        GameObject go;

        //Vector3 org = pb_Selection.activeGameObject == null ? Vector3.zero : pb_Selection.activeGameObject.transform.position;
        //Vector3 nrm = pb_Selection.activeGameObject == null ? Vector3.up : pb_Selection.activeGameObject.transform.localRotation * Vector3.up;

        //Plane plane = new Plane(nrm, org);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        float hit = 0f;

        //if (plane.Raycast(ray, out hit))
            go = (GameObject)pb_Scene.Instantiate(asset, new Vector3(), Quaternion.identity);
        //else
           // go = (GameObject)pb_Scene.Instantiate(asset, new Vector3(), Quaternion.identity);

        Undo.RegisterStates(new List<IUndo>() { new UndoSelection(), new UndoInstantiate(go) }, "Create new object");

        //pb_Selection.SetSelection(go);
 
        bool curSelection = pb_Selection.activeGameObject != null;

        if (!curSelection)
            pb_SceneCamera.Focus(go);
    }
}
