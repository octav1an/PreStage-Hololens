using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using RuntimeGizmos;

public class Manager : MonoBehaviour
{

    public static Manager Instance;
    public TransformGizmo GIZMO
    {
        get { return Camera.main.gameObject.GetComponent<TransformGizmo>(); }
    }
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

    public Vector3 HIT_LOCATION
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

    public PRCube SelectedGeo;
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
    public static List<GameObject> CollBlocksObjects = new List<GameObject>();
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
    }

    void Start () {

    }
	
	void Update () {
        //--------------------------------------------
        // Run all the time.
        //--------------------------------------------
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


    //-------------------------------------------------EVENTS--------------------------------------------------
    // By having the events for mouse down and up here, solved the problem of not activating the Input.GetMouseDown or Up
    // because, I think this event runs first. Moved everything to LateUpdate in BlockPrim, and works.
    //private void OnEnable()
    //{
    //    //EventManager.MouseDownGlobal += OnMouseDownGlobal;
    //    //EventManager.MouseUpGlobal += OnMouseUpGlobal;
    //}
    //private void OnDisable()
    //{
    //    //EventManager.MouseDownGlobal -= OnMouseDownGlobal;
    //    //EventManager.MouseUpGlobal -= OnMouseUpGlobal;
    //}
    //---------------------------------------------------------------------------------------------------

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
    public void OnAirtapDown()
    {
        
        InputDown = true;
        //-------------------------------------------------------
        // Update the colliderName when MouseDown.
        _ray = GazeManager.Instance.Rays[0];
        if (Physics.Raycast(_ray, out _hit))
        {
            PRCube block = UpdateSelection(_hit);
        }
    }

    public void OnAirtapUp()
    {
        InputDown = false;
    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------

    #endregion //Events

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Crate a block. Used in button.
    /// </summary>
    public void CreateBlock()
    {
        GameObject freshObj = (GameObject)Instantiate(BlockPrefab, new Vector3(), Quaternion.identity);
        CollBlocksObjects.Add(freshObj);
    }

    //---------------------------------------------------------------------------------------------------
    /// <summary>
    /// Select the block I am hitting.
    /// </summary>
    /// <param name="hit">Raycast hit.</param>
    private PRCube UpdateSelection(RaycastHit hit)
    {
        //print(hit.collider.tag);
        if (hit.collider.tag == "PRCube" && GIZMO.NEAR_AXIS == Axis.None)
        {
            PRCube geo = hit.collider.gameObject.GetComponent<PRCube>();
            // If there is a Active block and user selects another one, deselect the already Active one.
            if (SelectedGeo && geo.GetInstanceID() != SelectedGeo.GetInstanceID())
            {
                SelectedGeo.DeselectCube(UnselectedMaterial);
                geo.SelectCube(SelectedMaterial);
            }
            else
            {
                geo.SelectCube(SelectedMaterial);
            }
            return geo;
        }else if (hit.collider.tag == "ContexMenu" || hit.collider.tag == "PREdge" ||
                  hit.collider.tag == "PRFace" || hit.collider.tag == "PRVertex" || GIZMO.NEAR_AXIS != Axis.None)
        {
            return SelectedGeo;
        }
        else
        {
            Debug.Log("I don't know what are u hitting.");
            if(SelectedGeo)
                SelectedGeo.DeselectCube(UnselectedMaterial);
            return null;
        }
    }

    private void DeselectBlock(PRCube selected)
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
        GameObject[] alreadyCreated = GameObject.FindGameObjectsWithTag("BlockPrim");
        foreach (GameObject obj in alreadyCreated)
        {
            CollBlocksObjects.Add(obj);
        }

    }

    #region Draw

    #endregion //Draw



}
