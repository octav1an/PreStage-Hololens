using System.Runtime.InteropServices;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject SceneContentGo;
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public float YOffset;
    private bool SpatialActive = true;
    public GameObject SpacialGo;

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
        Debug.Log("New");
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
        SpacialGo.SetActive(true);
        Debug.Log("ModelButtonOn");
        Debug.Log("Enable: " + SceneContentGo.GetComponent<TapToPlacePR>().enabled);
        Debug.Log("IsBeingPlaced: " + SceneContentGo.GetComponent<TapToPlacePR>().IsBeingPlaced);
    }

    public void MoveModelOff()
    {
        SceneContentGo.GetComponent<TapToPlacePR>().IsBeingPlaced = false;
        SceneContentGo.GetComponent<TapToPlacePR>().enabled = false;
        SpacialGo.SetActive(false);
    }

    public void TurnOffSpacialMapping()
    {
        if (SpatialActive)
        {
            SpatialActive = false;
            SpacialGo.SetActive(false);
        }
        else
        {
            SpatialActive = true;
            SpacialGo.SetActive(true);
        }
    }

}
