using System.Runtime.InteropServices;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public float YOffset;


    #region Unity
    void Start () {
		
	}
	
	void Update ()
	{
	    OrientCanvasToCamera();
	    AlignToCenter(YOffset);

	}
    #endregion //Unity

    public void InstanciatePrefab1()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab1, new Vector3(0,-1, 2), Quaternion.identity);
        Debug.Log("New");
    }
    public void InstanciatePrefab2()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab2, new Vector3(0, -1, 2), Quaternion.identity);
    }
    public void InstanciatePrefab3()
    {
        GameObject freshObj = (GameObject)Instantiate(prefab3, new Vector3(0, -1, 2), Quaternion.identity);
    }

    /// <summary>
    /// Method that Orients the menu panel to camera view.
    /// </summary>
    private void OrientCanvasToCamera()
    {
        Vector3 canvasPos = transform.position;
        Vector3 cameraPos = Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(canvasPos - cameraPos, Vector3.up);
    }

    private void AlignToCenter(float yOffset)
    {
        Bounds bounds = GetColliderBounds(transform.parent);
        Vector3 center = bounds.center;
        Vector3 extends = bounds.extents;
        Vector3 pos = center + new Vector3(extends.x, extends.y + yOffset, extends.z);
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
        Bounds bounds = transform.GetChild(1).GetComponent<Collider>().bounds;
        for (int i = 1; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Collider>())
                bounds.Encapsulate(transform.GetChild(i).GetComponent<Collider>().bounds);

        }
        return bounds;
    }

    public void MoveModelOn()
    {
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<TapToPlacePR>().enabled = true;
    }

    public void MoveModelOff()
    {
        GameObject parent = transform.parent.gameObject;
        parent.GetComponent<TapToPlacePR>().enabled = false;
    }

}
