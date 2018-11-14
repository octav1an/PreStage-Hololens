using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class Manager : MonoBehaviour, IInputHandler
{

    public static Manager Instance;
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

    public BlockPrim SelectedBlock;
    /// <summary>
    /// Highlight Material when the block is selected.
    /// </summary>
    public Material SelectedMaterial;
    public Material UnselectedMaterial;
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
    private static RaycastHit _hit;
    //--------------------------------------------
    public static bool InputDown;


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

    // Use this for initialization
    void Start () {
        InputManager.Instance.AddGlobalListener(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        //--------------------------------------------
        // Run all the time.
        //--------------------------------------------
    }

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
    /// Method that is activated once when the mouse right click is released and the block is selected.
    /// </summary>
    private void OnMouseUpGlobal()
    {
        if (Input.GetMouseButtonUp(0))
        {

        }
    }

    //---------------------------------------------MOUSE DOWN------------------------------------------------------
    /// <summary>
    /// Method that is activated once when the mouse right click is pressed and the block is selected.
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
                SelectBlock(_hit);
            }

        }
    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------
    public void OnInputDown(InputEventData eventData)
    {
        InputDown = true;
        //-------------------------------------------------------
        // Update the colliderName when MouseDown.
        _ray = GazeManager.Instance.Rays[0];
        if (Physics.Raycast(_ray, out _hit))
        {
            BlockPrim block = SelectBlock(_hit);
            if(SelectedBlock)SelectedBlock.OnInputDownLocal();
        }
        //eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    public void OnInputUp(InputEventData eventData)
    {
        InputDown = false;
        //DeselectBlock(SelectedBlock);
        // Update the Block info on Tap Up.
        if(SelectedBlock) SelectedBlock.OnInputUpLocal();
        //eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
    //---------------------------------------------HOLOLENS INPUTS------------------------------------------------------

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
    private BlockPrim SelectBlock(RaycastHit hit)
    {
        if (hit.collider.tag == "BlockFace")
        {
            BlockPrim block = hit.collider.gameObject.GetComponent<BlockFace>().BLOCK_COMP;
            // If there is a selected block and user selects another one, deselect the already selected one.
            if (SelectedBlock && block.BlockId != SelectedBlock.BlockId)
            {
                DeselectBlock(SelectedBlock);
                block.Selected = true;
                block.GetComponent<MeshRenderer>().material = SelectedMaterial;
                SelectedBlock = block;
            }
            else
            {
                block.Selected = true;
                block.GetComponent<MeshRenderer>().material = SelectedMaterial;
                SelectedBlock = block;
            }
            return block;
        }else if (hit.collider.tag == "BlockMenu")
        {
            BlockPrim block = hit.collider.gameObject.GetComponent<BlockFace>().BLOCK_COMP;
            return block;
        }
        else
        {
            Debug.Log("I don't know what are u hitting.");
            DeselectBlock(SelectedBlock);
            return null;
        }
    }

    private void DeselectBlock(BlockPrim selected)
    {
        if (selected != null)
        {
            selected.GetComponent<MeshRenderer>().material = UnselectedMaterial;
            selected.Selected = false;
            selected.OnInputUpLocal();
            selected = null;
        }
    }

    private void SaveMouseLocation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SavedMouseLoc = Input.mousePosition;
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

}
