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

    public TransformGizmo gizmo;

    #region Unity

    void Start ()
	{
	    gizmo = GameObject.FindWithTag("MainCamera").gameObject.GetComponent<TransformGizmo>();
	}
	
	void Update ()
	{
	    MoveEdge();
	}

    void OnEnable()
    {
        EventManager.AirTapDown += OnAirtapDown;
        EventManager.AirTapUp += OnAirtapUp;
    }

    void OnDisable()
    {
        EventManager.AirTapDown -= OnAirtapDown;
        EventManager.AirTapUp -= OnAirtapUp;
    }

    #endregion //Unity

    #region Events

    private void OnAirtapDown()
    {
        UpdateActiveStatus();
        if (active)
        {
            savePos = transform.localPosition;
            // Save the edge holder.
            edgeHolder.savedEH = new PREdgeHolder(edgeHolder);
            _meshVertices = CUBE_MESH.vertices;
        }
        else
        {
            edgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
        }
    }

    private void OnAirtapUp()
    {
        if (active)
        {
            savePos = Vector3.zero;
            edgeHolder.savedEH = null;
            _meshVertices = null;
            //PARENT_CUBE.CleanUpEdgePrefabs();
            //PARENT_CUBE.CreateUniqEdgePrefabs(PARENT_CUBE.GenerateEdgeHolders());
        }
        else
        {
            edgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
        }
        UpdateActiveStatus();
        UpdateCollider();
    }

    #endregion //Events

    #region Move&Snap

    private void MoveEdge()
    {
        if (active && _meshVertices != null)
        {
            // Move the edge holder verts
            edgeHolder.UpdateEdge(transform.localPosition - savePos);
            // Move the overlaping verts as this edge.
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
    }

    #endregion //Move&Snap

    #region UpdateElements

    /// <summary>
    /// Update the collider of the edge at AirtapUp to match the modified edge.
    /// </summary>
    private void UpdateCollider()
    {
        // Place it at the center.
        transform.localPosition = edgeHolder.MidPos;
        transform.localRotation = edgeHolder.MidRot;
        // Scale the mesh and collider
        Vector3 scaleVec = edgeHolder.V0 - edgeHolder.V1;
        float mag = scaleVec.magnitude;
        Vector3 actualScale = transform.localScale;
        transform.localScale = new Vector3(actualScale.x, actualScale.y, mag);
    }

    /// <summary>
    /// Get the selected object form gizmo and check if it is this.
    /// </summary>
    private void UpdateActiveStatus()
    {
        if (gizmo.targetRootsOrdered.Count > 0)
        {
            if (gizmo.targetRootsOrdered[0].name == this.name)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
        else
        {
            active = false;
        }
    }

    #endregion //UpdateElements

}
