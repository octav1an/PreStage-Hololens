using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using RuntimeGizmos;
using UnityEngine;

public class ContexMenu : MonoBehaviour
{
    public static ContexMenu Instance;
    public Vector3 SavedHitPosition;
    /// <summary>
    /// Used in DoubleClick logic to avoid double activation.
    /// </summary>
    public bool IsActive;
    /// <summary>
    /// State of selection mode for the whole geomery
    /// </summary>
    public bool GeometryModeActive;

    private GameObject SELECTED_GO
    {
        get
        {
            if (Manager.Instance.SelectedGeo)
            {
                return Manager.Instance.SelectedGeo.gameObject;
            }

            return null;
        }
    }
    private PRCube SELECTED_PRCUBE
    {
        get { return SELECTED_GO.GetComponent<PRCube>(); }
    }

    // Double click fields.
    private int _tapCount = 0;
    private const float Delay = 0.5f;
    private float _timer;

    #region Unity

    private void Awake()
    {
        // Makes sure that I use always a game control even if my next scence already has one.
        // The instance of the object from the scene that is current will persist in the next scene.
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void Start () {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
	
	void Update ()
	{
	    OrientCanvasToCamera();
        if (_tapCount != 0 && (Time.time - _timer) > Delay)
	    {
	        _tapCount = 0;
	    }
    }

    void OnEnable()
    {
        EventManager.AirTapClick += OnTap;
    }

    void OnDisable()
    {
        EventManager.AirTapClick -= OnTap;
    }
    #endregion //Unity

    #region Events

    void OnTap()
    {
        _tapCount++;
        if (_tapCount == 1)
        {
            _timer = Time.time;
            SavedHitPosition = Manager.Instance.HIT_LOCATION;
            //print("OneClick");
            // Deactivate if hit outside contex menu.
            if(IsActive)DeactivateContexMenu();

        }
        else if (_tapCount == 2 && (Time.time - _timer) < Delay)
        {
            _tapCount = 0;
            //print("DoubleClick");
            if(!IsActive)ActivateContexMenu();
        }
        else
        {
            _tapCount = 0;
        }
    }

    #endregion //Events

    #region MenuActivation

    private void ActivateContexMenu()
    {
        float offset = 0.2f;
        Vector3 dir = Camera.main.transform.forward.normalized;
        this.transform.position = SavedHitPosition - dir * offset;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(true);
        }
        IsActive = true;
    }

    private void DeactivateContexMenu()
    {
        if (Manager.Instance.GET_COLLIDER_TAG == "ContexMenu" && CMSubmenu.Instance.IsAnySubmenuActive &&
            CMSubmenu.Instance.ActiveButton != Manager.Instance.GET_COLLIDER_GO.transform.parent.name)
        {
            CMSubmenu.Instance.DeactivateSubmenu();
            Debug.Log("HereWeGo!_part2");
            return;
        }
        if (Manager.Instance.GET_COLLIDER_TAG == "ContexMenu" || Manager.Instance.GET_COLLIDER_TAG == "CMSubmenu")
        {
            return;
        }
        // Deactivate the submenu first.
        Debug.Log("HereWeGo!");
        if (CMSubmenu.Instance.IsAnySubmenuActive)
        {
            CMSubmenu.Instance.DeactivateSubmenu();
        }
        // Deactivate the buttons of the menu.
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
        IsActive = false;
    }

    /// <summary>
    /// Deactivate ContexMeny without checking any collider hit.
    /// </summary>
    /// <param name="state"></param>
    private void DeactivateContexMenu(bool noColliderCheck)
    {
        if (noColliderCheck)
        {
            // Deactivate the submenu first.
            if (CMSubmenu.Instance.IsAnySubmenuActive)
            {
                CMSubmenu.Instance.DeactivateSubmenu();
            }
            // Deactivate the buttons of the menu.
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(false);
            }
            IsActive = false;
        }
    }

    #endregion //MenuActivation

    #region Menu Call Functions

    #region Selection Modes
    public void ActivateVertexMode()
    {
        // Instanciate the field first;
        ActiveteVertex(true);
        // Turn off other things
        ActivateEdge(false);
        ActivateFace(false);
        GeometryModeActive = false;
        // Deactivate ContexMenu to get it out of the way.
        DeactivateContexMenu(true);
    }
    public void ActivateEdgeMode()
    {
        if (SELECTED_GO != null)
        {
            ActivateEdge(true);
            // Turn off other things
            ActiveteVertex(false);
            ActivateFace(false);
            GeometryModeActive = false;
            // Update Edges when turned on so it fits the latest version of mesh.
            UpdateEdges(SELECTED_PRCUBE.PR_EDGE_GO);
            // Deactivate ContexMenu to get it out of the way.
            DeactivateContexMenu(true);
        }
    }
    public void ActivateFaceMode()
    {
        if (SELECTED_GO != null)
        {
            ActivateFace(true);
            // Turn off other things
            ActiveteVertex(false);
            ActivateEdge(false);
            GeometryModeActive = false;
            // Update face locations when turning it on, so it matches the actual mesh.
            UpdateFace(SELECTED_PRCUBE.PR_FACE_GO);
            // Deactivate ContexMenu to get it out of the way.
            DeactivateContexMenu(true);
        }
    }
    public void ActivateGeometryMode()
    {
        if (SELECTED_GO != null)
        {
            GeometryModeActive = true;
            ActiveteVertex(false);
            ActivateEdge(false);
            ActivateFace(false);
            // Deactivate ContexMenu to get it out of the way.
            DeactivateContexMenu(true);
            // Add the selected geometry to gizmo.
            Manager.Instance.GIZMO.ClearAndAddTarget(SELECTED_GO.transform);
        }
    }

    // Activate/Deactivate elements.
    private void ActiveteVertex(bool state)
    {
        SELECTED_PRCUBE.VertexModeActive = state;
        SELECTED_PRCUBE.PR_VERTEX_GO.SetActive(state);
    }
    private void ActivateEdge(bool state)
    {
        SELECTED_PRCUBE.EdgeModeActive = state;
        SELECTED_PRCUBE.PR_EDGE_GO.SetActive(state);
    }
    private void ActivateFace(bool state)
    {
        SELECTED_PRCUBE.FaceModeActive = state;
        SELECTED_PRCUBE.PR_FACE_GO.SetActive(state);
    }
    // Update Elements when switching between modes.
    private void UpdateEdges(GameObject parent)
    {
        PREdge[] edgeColl = parent.GetComponentsInChildren<PREdge>();
        foreach (var edge in edgeColl)
        {
            edge.EdgeHolder.UpdateInactiveEdgeInfo(SELECTED_PRCUBE.CubeMesh);
            edge.UpdateCollider();
        }
    }

    private void UpdateFace(GameObject paretn)
    {
        PRFace[] faceColl = paretn.GetComponentsInChildren<PRFace>();
        foreach (var face in faceColl)
        {
            face.UpdateCollider();
        }
    }
    #endregion //Selection Modes

    public void DeleteGeometry()
    {
        if (SELECTED_GO)
        {
            DeactivateContexMenu(true);
            Destroy(SELECTED_GO);
            Manager.Instance.SelectedGeo = null;
        }
    }

    public void SetMoveTransformationType()
    {
        Manager.Instance.GIZMO.type = TransformType.Move;
        DeactivateContexMenu(true);
    }

    public void SetRotateTransformationType()
    {
        Manager.Instance.GIZMO.type = TransformType.Rotate;
        DeactivateContexMenu(true);
    }
    public void SetScaleTransformationType()
    {
        Manager.Instance.GIZMO.type = TransformType.Scale;
        DeactivateContexMenu(true);
    }

    #endregion //Menu Call Functions

    #region UpdateElements

    /// <summary>
    /// Method that orients the Contex Menu canvas to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }

    #endregion //UpdateElements
}
