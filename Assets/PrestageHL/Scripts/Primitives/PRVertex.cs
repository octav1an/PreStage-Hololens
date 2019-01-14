using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using PRGeoClasses;

public class PRVertex : MonoBehaviour, IFocusable
{
    private PRGeo PARENT_CUBE
    {
        get { return transform.parent.parent.GetComponent<PRGeo>(); }
    }
    private Mesh CUBE_MESH
    {
        get { return PARENT_CUBE.CubeMesh; }
    }
    private Vector3[] _meshVertices;
    public PRVertexHolder VertexHolder;
    private Vector3 _savePos;
    private Material _savedThisMat;
    public GameObject DisplayVertexGO;

    public bool Active;
    public bool FocusActive = false;


    #region Unity
    protected virtual void Start ()
    {
        _savePos = transform.localPosition;
        _savedThisMat = DisplayVertexGO.GetComponent<MeshRenderer>().material;
    }

    protected virtual void Update ()
    {
        MoveVertex();
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
        //EdgeMeshDisplay(true);
        // Unhighlight the edges, so they are not highlighted next time they are turned on.
        Active = false;
        UnhighlightVertex();
    }
    #endregion // Unity


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
            VertexHolder.SavedV = new PRVertexHolder(VertexHolder);
            _meshVertices = CUBE_MESH.vertices;
        }
        else
        {
            // Deactivate other edge only when 
            if (Manager.Instance.GET_COLLIDER_TAG == "PRVertex" ||
                Manager.Instance.IsGizmoHit()) VertexMeshDisplay(false);
        }
    }

    private void OnInputUpLocal()
    {
        if (Active)
        {
            _savePos = Vector3.zero;
            VertexHolder.SavedV = null;
            _meshVertices = null;
            FocusActive = false;
        }

        UpdateActiveStatus();
        UpdateVertexPosition();
        // Display all the edges.
        VertexMeshDisplay(true);
    }

    private void HighlightVertex()
    {
        Material highlight = new Material(Manager.Instance.HighlightColliderMat);
        DisplayVertexGO.GetComponent<MeshRenderer>().material = highlight;
    }

    private void UnhighlightVertex()
    {
        DisplayVertexGO.GetComponent<MeshRenderer>().material = _savedThisMat;
    }

    protected void VertexMeshDisplay(bool state)
    {
        DisplayVertexGO.GetComponent<MeshRenderer>().enabled = state;
        GetComponent<Collider>().enabled = state;
    }
    #endregion // Events


    #region Move&Snap

    private void MoveVertex()
    {
        if (Active && _meshVertices != null)
        {
            // Move the vertex holder
            VertexHolder.MoveVertex(transform.localPosition - _savePos);
            // Move the overlaping verts as this edge.
            for (int i = 0; i < VertexHolder.SameVIndexColl.Count; i++)
            {
                _meshVertices[VertexHolder.SameVIndexColl[i]] = VertexHolder.V;
            }
            CUBE_MESH.vertices = _meshVertices;
            CUBE_MESH.RecalculateBounds();
        }
    }

    #endregion // Move&Snap


    #region UpdateElements

    /// <summary>
    /// Updates the infor of the vertex holder as well as vertex prefab location.
    /// </summary>
    public void UpdateVertexPosition()
    {
        VertexHolder.V = CUBE_MESH.vertices[VertexHolder.VIndex];
        transform.localPosition = VertexHolder.V;
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
        // Change Vertex material to activeMaterial.
        if (Active)
        {
            DisplayVertexGO.GetComponent<MeshRenderer>().material = Manager.Instance.ActiveColliderMat;
        }

        // Unhighlight all vertices when they are inactive and Gizmo.nearAxis is not None.
        if (!Active && Manager.Instance.GET_COLLIDER_LAYER == "Gizmo")
        {
            UnhighlightVertex();
        }
        if (FocusActive)
        {
            if (Manager.Instance.GET_COLLIDER_LAYER != "Gizmo")
            {
                HighlightVertex();
            }
        }
        else if (!Active)
        {
            UnhighlightVertex();
        }
    }

    #endregion // UpdateElements
}
