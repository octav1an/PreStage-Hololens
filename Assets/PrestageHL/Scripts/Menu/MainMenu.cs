using System.Runtime.InteropServices;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject SceneContentGo;
    public GameObject SceneScalerPrefab;
    public GameObject SpacialMappingGo;
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    /// <summary>
    /// Offset of Main menu on vertical axis.
    /// </summary>
    public float YOffset;
    private bool SpatialActive = true;

    public GameObject[] coll = new GameObject[8];

    /// <summary>
    /// SceneScaler GameObject.
    /// </summary>
    private GameObject _sceneScalerGo;

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


	}
    #endregion //Unity

    #region InstanciatePrefabs
    public void InstanciatePrefab1()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab1, new Vector3(0,-1, 2), Quaternion.identity, SceneContentGo.transform);
    }
    public void InstanciatePrefab2()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab2, new Vector3(0, -1, 2), Quaternion.identity, SceneContentGo.transform);
    }
    public void InstanciatePrefab3()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab3, new Vector3(0, -1, 2), Quaternion.identity, SceneContentGo.transform);
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
        Bounds bounds = GetColliderBounds(SceneContentGo.transform);
        Vector3 center = bounds.center;
        Vector3 extends = bounds.extents;
        Vector3 pos = center + new Vector3(0, extends.y + yOffset, 0);
        transform.position = pos;
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
    #endregion //UpdateElements


    public void MoveModelOn()
    {
        //GameObject parent = transform.parent.gameObject;
        SceneContentGo.GetComponent<TapToPlacePR>().enabled = true;
        //SceneContentGo.GetComponent<TapToPlaceOC>().IsBeingPlaced = true;
        SpacialMappingGo.SetActive(true);
        Debug.Log("ModelButtonOn");
        Debug.Log("Enable: " + SceneContentGo.GetComponent<TapToPlacePR>().enabled);
        Debug.Log("IsBeingPlaced: " + SceneContentGo.GetComponent<TapToPlacePR>().IsBeingPlaced);
    }

    public void MoveModelOff()
    {
        SceneContentGo.GetComponent<TapToPlacePR>().IsBeingPlaced = false;
        SceneContentGo.GetComponent<TapToPlacePR>().enabled = false;
        SpacialMappingGo.SetActive(false);
    }

    public void TurnOffSpacialMapping()
    {
        if (SpatialActive)
        {
            SpatialActive = false;
            SpacialMappingGo.SetActive(false);
        }
        else
        {
            SpatialActive = true;
            SpacialMappingGo.SetActive(true);
        }
    }

    public void ScaleModelOn()
    {
        Bounds sceneBounds = TapToPlacePR.GetColliderBounds(SceneContentGo.transform);
        Vector3 center = sceneBounds.center;
        Vector3 extents = sceneBounds.extents;
        // Create and adjust scaler object position.
        _sceneScalerGo = (GameObject) Instantiate(SceneScalerPrefab, center, Quaternion.identity);
        // Activate the TwoHandManipulation script for scale manipulations.
        _sceneScalerGo.GetComponent<TwoHandManipulatable>().enabled = true;
        // Create collider box
        BoxCollider boxCol = _sceneScalerGo.AddComponent<BoxCollider>();
        // Scale collider box using scene bounds.
        boxCol.size = extents * 2.05f;
        // Create BoundingBox mesh from BoxCollider
        Mesh boundingMesh = new Mesh();
        boundingMesh.vertices = SetUpBoundingBoxMeshVertices();
        int[] meshIndecies = new int[24] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23};
        boundingMesh.SetIndices(meshIndecies, MeshTopology.Quads, 0);
        boundingMesh.RecalculateBounds();
        boundingMesh.RecalculateNormals();
        boundingMesh.RecalculateTangents();
        _sceneScalerGo.GetComponent<MeshFilter>().mesh = boundingMesh;
        // Parent SceneContent to SceneScaler.
        SceneContentGo.transform.parent = _sceneScalerGo.transform;
    }

    public void ScaleModelOn2()
    {
        Bounds sceneBounds = TapToPlacePR.GetColliderBounds(SceneContentGo.transform);
        Vector3 center = sceneBounds.center;
        Vector3 extents = sceneBounds.extents;
        // Create and adjust scaler object position.
        _sceneScalerGo = (GameObject)Instantiate(SceneScalerPrefab, center, Quaternion.identity);
        // Activate the TwoHandManipulation script for scale manipulations.
        _sceneScalerGo.GetComponent<TwoHandManipulatable>().enabled = true;
        // Create collider box
        BoxCollider boxCol = _sceneScalerGo.AddComponent<BoxCollider>();
        _sceneScalerGo.transform.localScale = extents * 2 + new Vector3(0.02f, 0.02f, 0.02f);
        // Parent SceneContent to SceneScaler.
        SceneContentGo.transform.parent = _sceneScalerGo.transform;
    }

    public void ScaleModeOff()
    {
        SceneContentGo.transform.parent = null;
        Destroy(_sceneScalerGo);
    }

    Vector3[] GetColliderVertexPositions(GameObject obj)
    {
        var vertices = new Vector3[8];
        var thisMatrix = obj.transform.localToWorldMatrix;
        var storedRotation = obj.transform.rotation;
        obj.transform.rotation = Quaternion.identity;
       
        var extents = obj.GetComponent<BoxCollider>().bounds.extents;
        vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
        vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
        vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
        vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);
       
        obj.transform.rotation = storedRotation;

        return vertices;
    }

    Vector3[] SetUpBoundingBoxMeshVertices()
    {
        // Collection with all the vertices for the box.
        Vector3[] vertCollAll = new Vector3[24];
        Vector3[] vertColl = GetColliderVertexPositions(_sceneScalerGo);

        int[] meshIndecies = new int[24]
        {
            1,5,4,0,
            0,4,6,2,
            2,6,7,3,
            3,7,5,1,
            0,2,3,1,
            4,6,7,5
        };

        for (int i = 0; i < vertCollAll.Length; i++)
        {
            vertCollAll[i] = _sceneScalerGo.transform.InverseTransformPoint(vertColl[meshIndecies[i]]);
        }
        return vertCollAll;
    }

}
