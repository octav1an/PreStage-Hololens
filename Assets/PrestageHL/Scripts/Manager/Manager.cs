using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using RuntimeGizmos;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    public static Manager Instance;
    //public TransformGizmo GIZMO
    //{
    //    get
    //    {
    //        if (Camera.main)
    //        {
    //            return Camera.main.gameObject.GetComponent<TransformGizmo>();
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //}
    public TransformGizmo GIZMO;
    public EventManager EVENT_MANAGER
    {
        get { return GetComponent<EventManager>(); }
    }
    public GameObject SpatialMappingGo;
    /// <summary>
    /// Get the name for the collider hit by the ray.
    /// </summary>
    public string GET_COLLIDER_NAME
    {
        get
        {
            _ray = GazeManager.Instance.Rays[0];
            if (Physics.Raycast(_ray, out _hit))
            {
                return _hit.collider.name;
            }
            else
            {
                return null;
            }
        }
    }
    public string GET_COLLIDER_TAG
    {
        get
        {
            _ray = GazeManager.Instance.Rays[0];
            if (Physics.Raycast(_ray, out _hit))
            {
                return _hit.collider.tag;
            }
            else
            {
                return null;
            }
        }
    }
    public GameObject GET_COLLIDER_GO
    {
        get
        {
            _ray = GazeManager.Instance.Rays[0];
            if (Physics.Raycast(_ray, out _hit))
            {
                return _hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// Location where the hit point is, world space.
    /// </summary>
    public Vector3 GET_HIT_LOCATION
    {
        get
        {
            _ray = GazeManager.Instance.Rays[0];
            if (Physics.Raycast(_ray, out _hit))
            {
                return _hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    // TODO Change the SelectedGeo to be a regular gameObject. - better for the future.
    public GameObject SelectedGo;
    public PRGeo SelectedGeo;
    
    /// <summary>
    /// Highlight Material when the block is Active.
    /// </summary>
    public Material SelectedMaterial;
    public Material UnselectedMaterial;
    public Material HighlightColliderMat;
    public Material ActiveColliderMat;
    /// <summary>
    /// List with all the objects that are drawn on the canvas.
    /// </summary>
    public List<GameObject> CollGeoObjects = new List<GameObject>();
    /// <summary>
    /// Store location of mouse when right click is pressed.
    /// </summary>
    public static Vector3 SavedMouseLoc = new Vector3();
    /// <summary>
    /// Get the vector that represents difference between last mouse location and current mouse location
    /// </summary>
    public static Vector3 CHANGE_IN_MOUSE_LOC
    {
        get
        {
            if(SavedMouseLoc.x == 0 && SavedMouseLoc.y == 0)
            {
                return new Vector3();
            }
            else
            {
                return Input.mousePosition - SavedMouseLoc;
            }
        }
    }
    public static GameObject GROUND
    {
        get
        {
            return GameObject.FindGameObjectWithTag("Ground");
        }
    }
    /// <summary>
    /// Block prefab used to instanciate new blocks.
    /// </summary>
    public GameObject BlockPrefab;
    //--------------------------------------------
    private static Ray _ray;
    public static RaycastHit _hit;
    //--------------------------------------------
    public static bool InputDown;
    public bool FirstTime = true;



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
        CountAndStoreBlocks();
        SelectedGeo = null;
    }

    void Start () {

    }
	
	void Update () {
        //--------------------------------------------
        // Run all the time.
        //--------------------------------------------
	    _ray = GazeManager.Instance.Rays[0];
	    if (Physics.Raycast(_ray, out _hit))
	    {
	        //PRCube block = UpdateSelection(_hit);
	        //Debug.Log(_hit.collider.tag);
        }

	    if (Input.GetKeyDown(KeyCode.G))
	    {
            print("NumberOfObjects: " + CollGeoObjects.Count);
	    }
        //
        //Debug.Log(SpatialMappingManager.Instance.GetMeshes());
	    //Debug.Log("MeshCounts: " + SpatialMappingManager.Instance.GetMeshes().Count);
	}

    void OnEnable()
    {

        EventManager.AirTapDown += OnInputDownLocal;
        EventManager.AirTapUp += OnInputUpLocal;
        EventManager.AirTapDown += GIZMO.OnInputDownLocal;
        EventManager.AirTapUp += GIZMO.OnInputUpLocal;
        EventManager.AirTapClick += GIZMO.OnClickLocal;
        EventManager.SpeechKeywordRecognized += OnSpeechKeywordRecognizedLocal;

    }

    void OnDisable()
    {
        EventManager.AirTapDown -= OnInputDownLocal;
        EventManager.AirTapUp -= OnInputUpLocal;
        EventManager.AirTapDown -= GIZMO.OnInputDownLocal;
        EventManager.AirTapUp -= GIZMO.OnInputUpLocal;
        EventManager.AirTapClick -= GIZMO.OnClickLocal;
        EventManager.SpeechKeywordRecognized -= OnSpeechKeywordRecognizedLocal;

    }
    #endregion //Unity

    //---------------------------------------------MOUSE UP-------------------------------------------------------
    /// <summary>
    /// Method that is activated once when the mouse right click is released and the block is Active.
    /// </summary>
    private void OnMouseUpGlobal()
    {
        if (Input.GetMouseButtonUp(0))
        {

        }
    }

    //---------------------------------------------MOUSE DOWN------------------------------------------------------
    /// <summary>
    /// Method that is activated once when the mouse right click is pressed and the block is Active.
    /// </summary>
    private void OnMouseDownGlobal()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //-------------------------------------------------------
            // Update the colliderName when MouseDown.
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit))
            {
                UpdateSelection(_hit);
            }

        }
    }

    #region Events
   
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------
    public void OnInputDownLocal()
    {
        
        InputDown = true;
        //-------------------------------------------------------
        // Update the colliderName when MouseDown.
        _ray = GazeManager.Instance.Rays[0];
        if (Physics.Raycast(_ray, out _hit))
        {
            PRGeo block = UpdateSelection(_hit);
        }
    }

    public void OnInputUpLocal()
    {
        InputDown = false;

    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------

    public void OnSpeechKeywordRecognizedLocal()
    {
        // If hit collider is PRCube or PRFace or PREdge or PRVertex - do the change
        if (GET_COLLIDER_TAG == "PRCube" || GET_COLLIDER_TAG == "PRVertex" || 
            GET_COLLIDER_TAG == "PREdge" || GET_COLLIDER_TAG == "PRFace")
        {
            Debug.Log("speech: " + EVENT_MANAGER.EventDataSpeech.RecognizedText);
            //SelectedGeo.gameObject.GetComponent<SpeachGeometyKeywords>().OnSpeechKeywordRecognizedLocal();
        }
    }

    #endregion //Events


    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Select the block I am hitting.
    /// </summary>
    /// <param name="hit">Raycast hit.</param>
    private PRGeo UpdateSelection(RaycastHit hit)
    {
        //Debug.Log("HitTag: " + hit.collider.tag);
        //Debug.Log("HitName: " + hit.collider.name);
        //print(hit.collider.tag);
        if (hit.collider.tag == "PRCube" && GIZMO.NEAR_AXIS == Axis.None)
        {
            //Debug.Log("PRCube hit");
            PRGeo geo = hit.collider.gameObject.GetComponent<PRGeo>();
            // If there is a Active block and user selects another one, deselect the already Active one.
            if (SelectedGeo && geo.GetInstanceID() != SelectedGeo.GetInstanceID())
            {
                SelectedGeo.DeselectCube(UnselectedMaterial);
                geo.SelectCube(SelectedMaterial);
                StartCoroutine(SelectedGeo.TurnOnCube());
                //Debug.Log("Select hit");
            }
            else
            {
                geo.SelectCube(SelectedMaterial);
                StartCoroutine(SelectedGeo.TurnOnCube());
            }
            return geo;
        }else if (hit.collider.tag == "ContexMenu" || hit.collider.tag == "CMSubmenu" ||
                  hit.collider.tag == "PREdge" || hit.collider.tag == "PRFace" || 
                  hit.collider.tag == "PRCubeVertex" || GIZMO.NEAR_AXIS != Axis.None)
        {
            return SelectedGeo;
        }
        else
        {
            Debug.Log("I don't know what are u hitting.");
            if (SelectedGeo)
            {
                // Make sure all the all transformation modes are off.
                StartCoroutine(SelectedGeo.TurnOffAllModes());
                // Deselect cube.
                SelectedGeo.DeselectCube(UnselectedMaterial);
            }
                
            return null;
        }
    }

    // TODO: remove the Mehtod
    private void DeselectBlock(PRGeo selected)
    {
        if (selected != null)
        {
            Material[] unselectedMats = new Material[selected.GetComponent<MeshRenderer>().materials.Length];
            for (int i = 0; i < selected.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                unselectedMats[i] = UnselectedMaterial;
            }
            selected.GetComponent<MeshRenderer>().materials = unselectedMats;
            selected.Selected = false;
            selected = null;
        }
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Searches and store the blocks that are already drawn in COLL_BLOCKS_OBJECTS list.
    /// It should be used only once at scene starup, later objects shoud be dynamicaly added and removed. 
    /// </summary>
    private void CountAndStoreBlocks()
    {
        GameObject[] alreadyCreated = GameObject.FindGameObjectsWithTag("PRCube");
        foreach (GameObject obj in alreadyCreated)
        {
            CollGeoObjects.Add(obj);
        }

    }

    #region Draw

    #endregion //Draw



}
