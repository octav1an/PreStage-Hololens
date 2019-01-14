using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using PRGeoClasses;
using RuntimeGizmos;
using UnityEngine;

public class PREdge : MonoBehaviour, IFocusable
{

    private PRGeo PARENT_CUBE
    {
        get { return transform.parent.parent.GetComponent<PRGeo>(); }
    }
    private Mesh CUBE_MESH
    {
        get { return PARENT_CUBE.CubeMesh; }
    }
    /// <summary>
    /// GIZMO used for getting the selected object.
    /// </summary>
    public PREdgeHolder EdgeHolder;
    private Vector3 _savePos;
    private Vector3[] _meshVertices;
    private Material _savedThisMat;

    public bool Active;
    public bool FocusActive = false;

    #region Unity

    protected virtual void Awake()
    {
        //PARENT_CUBE = transform.parent.parent.GetComponent<PRGeo>();
        //CUBE_MESH = PARENT_CUBE.CubeMesh;
    }

    protected virtual void Start()
    {
        _savePos = transform.localPosition;
        _savedThisMat = GetComponent<MeshRenderer>().material;
    }

    protected virtual void Update()
    {
        MoveEdge();
        UpdateHighlightStatus();
    }

    void OnEnable()
    {
        EventManager.AirTapDown += OnInputDownLocal;
        EventManager.AirTapUp += OnInputUpLocal;
    }

    void OnDisable()
    {
        EventManager.AirTapDown -= OnInputDownLocal;
        EventManager.AirTapUp -= OnInputUpLocal;

        // Turn on all MeshRenderer components if the edges are closed.
        EdgeMeshDisplay(true);
        // Unhighlight the edges, so they are not highlighted next time they are turned on.
        Active = false;
        UnhighlightEdge();
    }

    #endregion //Unity

    #region Events

    public void OnFocusEnter()
    {
        if (!Active)
        {
            FocusActive = true;
        }
    }

    public void OnFocusExit()
    {
        if (!Active)
        {
            FocusActive = false;
        }
    }

    private void OnInputDownLocal()
    {
        if (UpdateActiveStatus())
        {
            _savePos = transform.localPosition;
            // Save the edge holder.
            EdgeHolder.savedEH = new PREdgeHolder(EdgeHolder);
            _meshVertices = CUBE_MESH.vertices;
        }
        else
        {
            EdgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
            // Deactivate other edge only when 
            if(Manager.Instance.GET_COLLIDER_TAG == "PREdge" ||
               Manager.Instance.IsGizmoHit()) EdgeMeshDisplay(false);
        }
    }

    private void OnInputUpLocal()
    {
        if (Active)
        {
            _savePos = Vector3.zero;
            EdgeHolder.savedEH = null;
            _meshVertices = null;
            FocusActive = false;
        }
        else
        {
            if(Manager.Instance.GET_COLLIDER_TAG != "GizmoScale") EdgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
        }
        UpdateActiveStatus();
        UpdateCollider();
        // Display all the edges.
        EdgeMeshDisplay(true);
    }

    private void HighlightEdge()
    {
        Material highlight = new Material(Manager.Instance.HighlightColliderMat);
        GetComponent<MeshRenderer>().material = highlight;
    }

    private void UnhighlightEdge()
    {
        GetComponent<MeshRenderer>().material = _savedThisMat;
    }

    //protected void DeactivateInactiveEdgesDuringTransformation()
    //{
    //    if (!Active)
    //    {
    //        //DeactivateEdgeMesh
    //    }
    //}

    protected void EdgeMeshDisplay(bool state)
    {
        GetComponent<MeshRenderer>().enabled = state;
        GetComponent<Collider>().enabled = state;
    }
    #endregion //Events

    #region Move&Snap

    private void MoveEdge()
    {
        if (Active && _meshVertices != null)
        {
            // Move the edge holder verts
            EdgeHolder.UpdateEdge(transform.localPosition - _savePos);
            // Move the overlaping verts as this edge.
            for (int i = 0; i < EdgeHolder.SameV0Index.Count; i++)
            {
                _meshVertices[EdgeHolder.SameV0Index[i]] = EdgeHolder.V0;
            }
            for (int i = 0; i < EdgeHolder.SameV1Index.Count; i++)
            {
                _meshVertices[EdgeHolder.SameV1Index[i]] = EdgeHolder.V1;
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
    public void UpdateCollider()
    {
        // Place it at the center.
        transform.localPosition = EdgeHolder.MidPos;
        transform.localRotation = EdgeHolder.MidRot;

        // Scale the mesh and collider
        Vector3 scaleVec = EdgeHolder.V0 - EdgeHolder.V1;
        float mag = scaleVec.magnitude;
        Vector3 actualScale = transform.localScale;
        transform.localScale = new Vector3(actualScale.x, mag / 2, actualScale.z);
    }

    /// <summary>
    /// Get the selected object form gizmo and check if it is this.
    /// </summary>
    private bool UpdateActiveStatus()
    {
        if (Manager.Instance.GIZMO.targetRootsOrdered.Count > 0)
        {
            if (Manager.Instance.GIZMO.targetRootsOrdered[0].name == this.name)
            {
                Active = true;
                return true;
            }
            else
            {
                Active = false;
                return false;
            }
        }
        else
        {
            Active = false;
            return false;
        }
    }

    private void UpdateHighlightStatus()
    {
        // Change Edge material to activeMaterial.
        if (Active)
        {
            GetComponent<MeshRenderer>().material = Manager.Instance.ActiveColliderMat;
        }

        // Unhighlight all edges when they are inactive and Gizmo.nearAxis is not None.
        if (!Active && Manager.Instance.GET_COLLIDER_LAYER == "Gizmo")
        {
            UnhighlightEdge();
        }
        if (FocusActive)
        {
            if (Manager.Instance.GET_COLLIDER_LAYER != "Gizmo")
            {
                HighlightEdge();
            }
        }
        else if (!Active)
        {
            UnhighlightEdge();
        }
    }
    #endregion //UpdateElements
}
