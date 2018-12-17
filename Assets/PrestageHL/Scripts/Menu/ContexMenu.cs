using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using RuntimeGizmos;
#if UNITY_EDITOR
    using UnityEditorInternal;
#endif
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
    private PRGeo SELECTED_PRCUBE
    {
        get { return SELECTED_GO.GetComponent<PRGeo>(); }
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
            SavedHitPosition = Manager.Instance.GET_HIT_LOCATION;
            //print("OneClick");
            // Deactivate if hit outside contex menu.
            if(IsActive)DeactivateContexMenu();

        }
        else if (_tapCount == 2 && (Time.time - _timer) < Delay)
        {
            _tapCount = 0;
            //print("DoubleClick);
            if (!IsActive && IsSelectedTheSameAsHit())
            {
                ActivateContexMenu();
            }
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
            return;
        }
        if (Manager.Instance.GET_COLLIDER_TAG == "ContexMenu" || Manager.Instance.GET_COLLIDER_TAG == "CMSubmenu")
        {
            return;
        }
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

    /// <summary>
    /// Deactivate ContexMeny without checking any collider hit.
    /// </summary>
    /// <param name="state"></param>
    private void DeactivateContexMenu(bool noColliderCheck)
    {
        if (noColliderCheck)
        {
            if(!IsActive) return;
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

    #region MenuCallFunctions

    #region Selection Modes
    public void SetVertexMode()
    {
        // Set the Space to Global.
        Manager.Instance.GIZMO.space = TransformSpace.Local;
        // First deactivate all modes.
        StartCoroutine(SELECTED_PRCUBE.TurnOffAllModes());

        // Instanciate the field first;
        ActiveteVertex(true);
        // Turn off other things
        ActivateEdge(false);
        ActivateFace(false);
        GeometryModeActive = false;
        // Deactivate ContexMenu to get it out of the way.
        DeactivateContexMenu(true);
    }
    public void SetEdgeMode()
    {
        if (SELECTED_GO != null)
        {
            // Set the Space to Global.
            Manager.Instance.GIZMO.space = TransformSpace.Local;
            // First deactivate all modes.
            StartCoroutine(SELECTED_PRCUBE.TurnOffAllModes());

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
    public void SetFaceMode()
    {
        if (SELECTED_GO != null)
        {
            // Set the Space to Global.
            Manager.Instance.GIZMO.space = TransformSpace.Local;

            // First deactivate all modes.
            StartCoroutine(SELECTED_PRCUBE.TurnOffAllModes());

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
    public void SetGeometryMode()
    {
        if (SELECTED_GO != null)
        {
            // Set the Space to Global only if the transform mode is Move.
            if (Manager.Instance.GIZMO.type == TransformType.Move) Manager.Instance.GIZMO.space = TransformSpace.Global;

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
        PRGeoEdge[] edgeColl = parent.GetComponentsInChildren<PRGeoEdge>();
        foreach (var edge in edgeColl)
        {
            edge.EdgeHolder.UpdateInactiveEdgeInfo(SELECTED_PRCUBE.CubeMesh);
            edge.UpdateCollider();
        }
    }

    private void UpdateFace(GameObject paretn)
    {
        PRGeoFace[] faceColl = paretn.GetComponentsInChildren<PRGeoFace>();
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
        // Set the Space to Global.
        if(GeometryModeActive) Manager.Instance.GIZMO.space = TransformSpace.Global;

        Manager.Instance.GIZMO.type = TransformType.Move;
        // Disply the gizmo arrows.
        Manager.Instance.GIZMO.DisableGizmo = false;
        // Disble Grab script in selected primitive.
        SELECTED_GO.GetComponent<HandDraggable>().enabled = false;
        // Reactivate the Cube mode, in order to have the gizmo displyed.
        if(SELECTED_PRCUBE.CubeModeActive)StartCoroutine(SELECTED_PRCUBE.TurnOnCube());
        DeactivateContexMenu(true);
    }
    public void SetRotateTransformationType()
    {
        // Set the Space to Local.
        Manager.Instance.GIZMO.space = TransformSpace.Local;

        Manager.Instance.GIZMO.type = TransformType.Rotate;
        // Disply the gizmo arrows.
        Manager.Instance.GIZMO.DisableGizmo = false;
        // Disble Grab script in selected primitive.
        SELECTED_GO.GetComponent<HandDraggable>().enabled = false;
        // Reactivate the Cube mode, in order to have the gizmo displyed.
        if (SELECTED_PRCUBE.CubeModeActive) StartCoroutine(SELECTED_PRCUBE.TurnOnCube());
        DeactivateContexMenu(true);
    }
    public void SetScaleTransformationType()
    {
        // Set the Space to Local.
        Manager.Instance.GIZMO.space = TransformSpace.Local;

        Manager.Instance.GIZMO.type = TransformType.Scale;
        // Disply the gizmo arrows.
        Manager.Instance.GIZMO.DisableGizmo = false;
        // Disble Grab script in selected primitive.
        SELECTED_GO.GetComponent<HandDraggable>().enabled = false;
        // Reactivate the Cube mode, in order to have the gizmo displyed.
        if (SELECTED_PRCUBE.CubeModeActive) StartCoroutine(SELECTED_PRCUBE.TurnOnCube());
        DeactivateContexMenu(true);
    }
    public void SetGrabTransformationType()
    {
        Manager.Instance.GIZMO.DisableGizmo = true;
        SELECTED_GO.GetComponent<HandDraggable>().enabled = true;
        // Turn off other things
        ActiveteVertex(false);
        ActivateEdge(false);
        ActivateFace(false);
        // Reactivate the Cube mode, in order to have the gizmo displyed.
        // TODO: Grab only works with Mode
        StartCoroutine(SELECTED_PRCUBE.TurnOnCube());
        DeactivateContexMenu(true);
    }

    #endregion //MenuCallFunctions

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

    #region Other

    private bool IsSelectedTheSameAsHit()
    {
        // Check if there is a selected object.
        if (!Manager.Instance.SelectedGeo) return false;
        // Check if there is a hit object.
        if (!Manager.Instance.GET_COLLIDER_GO) return false;
        // Check if it has the same ID as the hit object.
        if (Manager.Instance.GET_COLLIDER_GO.GetInstanceID() ==
            Manager.Instance.SelectedGeo.gameObject.GetInstanceID())
        {
            //Debug.Log("1: " + Manager.Instance.GET_COLLIDER_GO.GetInstanceID());
            //Debug.Log("2: " + Manager.Instance.SelectedGeo.gameObject.GetInstanceID());
            return true;
        }
        // Activate the context menu when hitting faces or edges.
        if (Manager.Instance.GET_COLLIDER_TAG == "PRFace" ||
            Manager.Instance.GET_COLLIDER_TAG == "PREdge")
        {
            return true;
        }

        return false;
    }

    #endregion //Other
}
