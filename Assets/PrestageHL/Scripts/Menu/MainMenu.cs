using System.Runtime.InteropServices;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject SceneMoverPrefab;
    public GameObject SceneScalerPrefab;
    public GameObject SceneRotatorPrefab;
    public GameObject SceneCenter;

    public GameObject prefab0;
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    public GameObject prefab5;
    /// <summary>
    /// Offset of Main menu on vertical axis.
    /// </summary>
    public float YOffset;
    private bool SpatialActive = true;

    private GameObject _sceneMoverGo;
    private GameObject _sceneScalerGo;
    private GameObject _sceneRotatorGo;

    public GameObject parentTextGo;
    /// <summary>
    /// Scene center.
    /// </summary>
    private Vector3 _sceneCenter;

    private Vector3 _sceneCenterOffseted
    {
        get { return _sceneCenter - Camera.main.transform.forward.normalized * 0.5f; }
    }

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
	    AlignToCenter(YOffset);

	    Text paretnText = parentTextGo.GetComponent<Text>();

        if(Manager.Instance.EVENT_MANAGER.EventDataSpeech != null) paretnText.text = "Recognized: " + Manager.Instance.EVENT_MANAGER.EventDataSpeech.RecognizedText;
	    _sceneCenter = GetColliderBoundsNew().center;
        //SceneCenter.transform.position = _sceneCenterOffseted;
    }
    #endregion //Unity

    #region InstanciatePrefabs
    public void InstanciatePrefab0()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab0, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
    }
    public void InstanciatePrefab1()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab1, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
    }
    public void InstanciatePrefab2()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab2, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
    }
    public void InstanciatePrefab3()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab3, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
    }
    public void InstanciatePrefab4()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab4, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
    }
    public void InstanciatePrefab5()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab5, _sceneCenterOffseted, Quaternion.identity);
        Manager.Instance.CollGeoObjects.Add(freshObj);
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

    public void TurnOffSpacialMapping()
    {
        if (SpatialActive)
        {
            SpatialActive = false;
            Manager.Instance.SpatialMappingGo.SetActive(false);
        }
        else
        {
            SpatialActive = true;
            Manager.Instance.SpatialMappingGo.SetActive(true);
        }
    }

    public void ScaleModelOn()
    {
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
}
