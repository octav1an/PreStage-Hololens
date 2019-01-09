using System.Collections;
using System.Runtime.InteropServices;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public float ScaleMagnitude = 0.001f;
    public GameObject SceneMoverPrefab;
    public GameObject SceneScalerPrefab;
    public GameObject SceneRotatorPrefab;
    public GameObject SceneCenter;

    public GameObject Prefab0;
    public GameObject Prefab1;
    public GameObject Prefab2;
    public GameObject Prefab3;
    public GameObject Prefab4;
    public GameObject Prefab5;
    // TODO: Remove if sceneMove works fine.
    private bool _spatialActive = true;

    private GameObject _sceneMoverGo;
    private GameObject _sceneScalerGo;
    private GameObject _sceneRotatorGo;

    // TODO: remove when the tool tip at cursor will be done.
    public GameObject ParentTextGo;

    public GameObject InstantiatedObject;
    public Bounds InstantiatedObjectBounds;
    private bool _isPlacing;

    #region Unity

    void Awake()
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
		
	}
	
	void Update ()
	{
	    OrientCanvasToCamera();
	    AlignMenuPosition();
	    PlancingNewObject();

        Text paretnText = ParentTextGo.GetComponent<Text>();
        if(Manager.Instance.EVENT_MANAGER.EventDataSpeech != null) paretnText.text = "Recognized: " + Manager.Instance.EVENT_MANAGER.EventDataSpeech.RecognizedText;

        // Scane Main menu in relation to distance from camera.
        Manager.Instance.ScaleToDistance(gameObject, ScaleMagnitude);
    }
    #endregion //Unity

    #region InstanciatePrefabs

    public void PlancingNewObject()
    {
        if (_isPlacing)
        {
            // Change object opacity while placing.
            InstantiatedObject.GetComponent<PRGeo>().ChangeObjectOpacity(0.5f);
            // Change Object layer to IgnoreRayCast
            InstantiatedObject.layer = 2;
            // Do the position updates
            if (!Manager.Instance.IS_HIT || Manager.Instance.IS_GUI_HIT)
            {
                Vector3 dir = Camera.main.transform.forward;
                InstantiatedObject.transform.position = Camera.main.transform.position + (dir * 4);
            }
            else
            {
                //InstantiatedObject.transform.position = Manager.Instance.GET_HIT_LOCATION;
                InstantiatedObject.transform.position = Manager.Instance.GET_HIT_LOCATION + GetPlacementOffset();
            }
            // If input down then place
            if (Manager.Instance.GIZMO.InputDown)
            {
                // Place obj.
                Manager.Instance.CollGeoObjects.Add(InstantiatedObject);
                InstantiatedObject.GetComponent<PRGeo>().ChangeObjectOpacity(1f);
                // Change object layer to Geometry.
                InstantiatedObject.layer = 9;
                InstantiatedObject = null;
                _isPlacing = false;
            }
        }
    }
    /// <summary>
    /// Calculates the offset vector of a geometry to aboid intersection when placing the geometry.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlacementOffset()
    {
        if (_isPlacing)
        {
            Bounds bounds = InstantiatedObject.GetComponent<Collider>().bounds;
            Vector3 projected = Vector3.Project(bounds.extents, Manager.Instance.GET_HIT_NORMAL);
            Vector3 offset = Manager.Instance.GET_HIT_NORMAL * projected.magnitude;
            return offset;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Instantiates a prefab from the given name. See naming in FindPrefabByName method.
    /// </summary>
    /// <param name="str"> Prefab name to be created. </param>
    public void GeneralInstantiatePrefab(string str)
    {
        GameObject freshObj = (GameObject)Instantiate(FindPrefabByName(str), Vector3.zero, Quaternion.identity);
        InstantiatedObject = freshObj;
        InstantiatedObjectBounds = freshObj.GetComponent<Collider>().bounds;
        _isPlacing = true;
    }

    private GameObject FindPrefabByName(string prefabName)
    {
        switch (prefabName.ToLower())
        {
            case "prefab0":
                return Prefab0;
            case "prefab1":
                return Prefab1;
            case "prefab2":
                return Prefab2;
            case "prefab3":
                return Prefab3;
            case "prefab4":
                return Prefab4;
            case "prefab5":
                return Prefab5;
            default:
                return null;
        }
    }
    #endregion //InstanciatePrefabs

    #region UpdateElements

    /// <summary>
    /// Method that Orients the menu panel to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }

    private void AlignMenuPosition()
    {
        transform.position = BaseTable.Instance.MENU_POSITION;
    }

    /// <summary>
    /// Align Main Menu position to be the centroid of the avarage of the all drawn geometry.
    /// </summary>
    /// <param name="yOffset"> Offset on Y axix, which is the vertical. </param>
    private void AlignToCenter(float yOffset)
    {
        if (Manager.Instance.CollGeoObjects.Count > 0)
        {
            Bounds bounds = GetColliderBoundsNew();
            Vector3 center = bounds.center;
            Vector3 extends = bounds.extents;
            Vector3 pos = center + new Vector3(0, extends.y + yOffset, 0);
            transform.position = pos;
        }
        else
        {
            transform.position = new Vector3(0, 0, 2);
        }
    }

    /// <summary>
    /// Calculates the bounds of all the colliders attached to this GameObject and all it's children
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Bounds GetColliderBounds(Transform transform)
    {
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        //GameObject children = transform.ge
        if (transform.childCount == 0) { return new Bounds(); }
        // Use 1 to get the second child in SceneContainer, because the first one is MainMenu
        Bounds bounds = transform.GetChild(0).GetComponent<Collider>().bounds;
        for (int i = 1; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Collider>())
                bounds.Encapsulate(transform.GetChild(i).GetComponent<Collider>().bounds);

        }
        return bounds;
    }

    public Bounds GetColliderBoundsNew()
    {
        if(Manager.Instance.CollGeoObjects.Count == 0) return new Bounds();
        Bounds bounds = Manager.Instance.CollGeoObjects[0].GetComponent<Collider>().bounds;
        for (int i = 1; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            if (geo.GetComponent<Collider>())
            {
                bounds.Encapsulate(geo.GetComponent<Collider>().bounds);
            }
        }
        return bounds;
    }

    #endregion //UpdateElements

    #region MenuCallFunctions

    public void MoveModelOn()
    {
        // Create the prefab in the center of the scene
        _sceneMoverGo = (GameObject)Instantiate(SceneMoverPrefab, GetColliderBoundsNew().center, Quaternion.identity);
        _sceneMoverGo.GetComponent<TapToPlacePR>().enabled = true;
        // Parent all scene geometry to SceneMoverPrefab
        for(int i = 0; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            geo.transform.parent = _sceneMoverGo.transform;
        }
        //SceneContentGo.GetComponent<TapToPlaceOC>().IsBeingPlaced = true;
        Manager.Instance.SpatialMappingGo.SetActive(true);
    }

    public void MoveModelOff()
    {
        // Unparent scene geometry
        for (int i = 0; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            geo.transform.parent = null;
        }
        Destroy(_sceneMoverGo);
        Manager.Instance.SpatialMappingGo.SetActive(false);
    }

    // TODO: Remove is scene move works fine.
    //public void TurnOffSpacialMapping()
    //{
    //    if (_spatialActive)
    //    {
    //        _spatialActive = false;
    //        Manager.Instance.SpatialMappingGo.SetActive(false);
    //    }
    //    else
    //    {
    //        _spatialActive = true;
    //        Manager.Instance.SpatialMappingGo.SetActive(true);
    //    }
    //}

    public void ScaleModelOn()
    {
        // First destroy immediatly the rotator object if it exists.
        if (_sceneRotatorGo) DestroyImmediate(_sceneRotatorGo);

        Bounds sceneBounds = GetColliderBoundsNew();
        Vector3 center = sceneBounds.center;
        Vector3 extents = sceneBounds.extents;
        // Compute the center of bounds that is at the buttom.
        Vector3 offsetCenter = center + new Vector3(0, -extents.y, 0); 
        // Create and adjust scaler object position.
        _sceneScalerGo = (GameObject)Instantiate(SceneScalerPrefab, offsetCenter, Quaternion.identity);
        // Activate the TwoHandManipulation script for scale manipulations.
        _sceneScalerGo.GetComponent<TwoHandManipulatable>().enabled = true;
        // Create collider box
        BoxCollider boxCol = _sceneScalerGo.AddComponent<BoxCollider>();
        // Adjust the colider center to compensate for center offset.
        boxCol.center += new Vector3(0, boxCol.size.y / 2, 0);
        
        // Adjust the mesh to compensate for the center offset.
        OffsetMesh(_sceneScalerGo.GetComponent<MeshFilter>().mesh, new Vector3(0, boxCol.size.y/2, 0));
        _sceneScalerGo.transform.localScale = extents * 2 + new Vector3(0.02f, 0.02f, 0.02f);
        
        // Parent scene geometry to SceneScaler.
        for (int i = 0; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            geo.transform.parent = _sceneScalerGo.transform;
        }

    }

    public void RotateModelOn()
    {
        // First destroy immediatly the scaler object if it exists.
        if(_sceneScalerGo)DestroyImmediate(_sceneScalerGo);

        Bounds sceneBounds = GetColliderBoundsNew();
        Vector3 center = sceneBounds.center;
        Vector3 extents = sceneBounds.extents;
        // Create and adjust scaler object position.
        _sceneRotatorGo = (GameObject)Instantiate(SceneRotatorPrefab, center, Quaternion.identity);
        // Activate the TwoHandManipulation script for scale manipulations.
        _sceneRotatorGo.GetComponent<TwoHandManipulatable>().enabled = true;
        // Create collider box
        BoxCollider boxCol = _sceneRotatorGo.AddComponent<BoxCollider>();
        // Adjust Rotator scale to match the sceneBounds size.
        _sceneRotatorGo.transform.localScale = extents * 2 + new Vector3(0.02f, 0.02f, 0.02f);

        // Parent scene geometry to SceneScaler.
        for (int i = 0; i < Manager.Instance.CollGeoObjects.Count; i++)
        {
            GameObject geo = Manager.Instance.CollGeoObjects[i];
            geo.transform.parent = _sceneRotatorGo.transform;
        }
    }

    #endregion // MenuCallFunctions

    /// <summary>
    /// Move the vertices of a mesh with a vector.
    /// </summary>
    /// <param name="mesh"> Mesh to move. </param>
    /// <param name="offset"> Offset vector. </param>
    private void OffsetMesh(Mesh mesh, Vector3 offset)
    {
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            verts[i] += offset;
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();
    }

    #region Other

    public void CloseAllSubmenus()
    {
        transform.Find("B_File").transform.Find("SubButtons").gameObject.SetActive(false);
        transform.Find("B_Settings").transform.Find("SubButtons").gameObject.SetActive(false);
        transform.Find("B_Help").transform.Find("SubButtons").gameObject.SetActive(false);
    }

    #endregion // Other
}
