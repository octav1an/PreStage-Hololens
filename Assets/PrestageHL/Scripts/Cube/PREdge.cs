using System.Collections;
using System.Collections.Generic;
using PRCubeClasses;
using RuntimeGizmos;
using UnityEngine;
using UnityEngine.UI;
using RuntimeGizmos;


public class PREdge : MonoBehaviour
{
    private PRCube PARENT_CUBE
    {
        get { return transform.parent.parent.GetComponent<PRCube>(); }
    }
    private Mesh CUBE_MESH
    {
        get { return PARENT_CUBE.CubeMesh; }
    }
    public PREdgeHolder edgeHolder;
    private Vector3 savePos;
    private Vector3[] _meshVertices;

    public bool active;
    public bool firstTime = true;
    public bool secondTime = false;

    public TransformGizmo gizmo; 

	// Use this for initialization
	void Start ()
	{
	    gizmo = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<TransformGizmo>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        // Get the selected object form gizmo and check if it is this.
	    if (gizmo.targetRootsOrdered.Count > 0)
	    {
	        if (gizmo.targetRootsOrdered[0].name == this.name)
	        {
	            active = true;
	        }
	    }
	    if (active && _meshVertices != null)
        { 
            edgeHolder.UpdateEdge(transform.localPosition - savePos);
            for (int i = 0; i < edgeHolder.SameV0Index.Count; i++)
	        {
	            _meshVertices[edgeHolder.SameV0Index[i]] = edgeHolder.V0;
	        }
	        for (int i = 0; i < edgeHolder.SameV1Index.Count; i++)
	        {
	            _meshVertices[edgeHolder.SameV1Index[i]] = edgeHolder.V1;
	        }
            CUBE_MESH.vertices = _meshVertices;
            CUBE_MESH.RecalculateBounds();
        }
        //print(gizmo.targetRootsOrdered[0]);
	}

    void OnEnable()
    {
        EventManager.AirTapDown += SavePos;
        EventManager.AirTapUp += CleanUpOnAirtapUp;
    }

    void OnDisable()
    {
        EventManager.AirTapDown -= SavePos;
        EventManager.AirTapUp -= CleanUpOnAirtapUp;
    }

    void SavePos()
    {
        if (active)
        {
            savePos = transform.localPosition;
            // Save the edge holder.
            edgeHolder.savedEH = new PREdgeHolder(edgeHolder);
            _meshVertices = CUBE_MESH.vertices;
        }
    }

    void CleanUpOnAirtapUp()
    {
        if (active && secondTime)
        {
            active = false;
            savePos = Vector3.zero;
            edgeHolder.savedEH = null;
            _meshVertices = null;
            //PARENT_CUBE.CleanUpEdgePrefabs();
            //PARENT_CUBE.CreateUniqEdgePrefabs(PARENT_CUBE.GenerateEdgeHolders());
            return;
        }
        else if(active)
        {
            secondTime = true;
        }
    }

}
