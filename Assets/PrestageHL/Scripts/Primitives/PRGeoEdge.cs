using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using PRGeoClasses;
using UnityEngine;

public class PRGeoEdge : MonoBehaviour, IFocusable
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

    protected virtual void Start()
    {
        _savePos = transform.localPosition;
    }

    protected virtual void Update()
    {
        MoveEdge();
        if (Active)
        {
            GetComponent<MeshRenderer>().material = Manager.Instance.ActiveColliderMat;
        }
        else if (!Active && _savedThisMat && !FocusActive)
        {
            GetComponent<MeshRenderer>().material = _savedThisMat;
        }
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
    }

    #endregion //Unity

    #region Events

    public void OnFocusEnter()
    {
        if (!Active)
        {
            FocusActive = true;
            HighlightEdge();
        }
    }

    public void OnFocusExit()
    {
        if (!Active)
        {
            FocusActive = false;
            UnhighlightEdge();
        }
    }

    private void OnInputDownLocal()
    {
        UpdateActiveStatus();
        if (Active)
        {
            _savePos = transform.localPosition;
            //_savedThisMat = GetComponent<MeshRenderer>().material;
            // Save the edge holder.
            EdgeHolder.savedEH = new PREdgeHolder(EdgeHolder);
            _meshVertices = CUBE_MESH.vertices;
        }
        else
        {
            EdgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
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
            EdgeHolder.UpdateInactiveEdgeInfo(CUBE_MESH);
        }
        UpdateActiveStatus();
        UpdateCollider();
    }

    private void HighlightEdge()
    {
        // Store this object material.
        _savedThisMat = GetComponent<MeshRenderer>().material;

        Material highlight = new Material(Manager.Instance.HighlightColliderMat);
        GetComponent<MeshRenderer>().material = highlight;
    }

    private void UnhighlightEdge()
    {
        GetComponent<MeshRenderer>().material = _savedThisMat;
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
    private void UpdateActiveStatus()
    {
        if (Manager.Instance.GIZMO.targetRootsOrdered.Count > 0)
        {
            if (Manager.Instance.GIZMO.targetRootsOrdered[0].name == this.name)
            {
                Active = true;
            }
            else
            {
                Active = false;
            }
        }
        else
        {
            Active = false;
        }
    }

    #endregion //UpdateElements
}
