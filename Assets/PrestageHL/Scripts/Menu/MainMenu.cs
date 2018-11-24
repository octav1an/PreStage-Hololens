using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public float YOffset;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    OrientCanvasToCamera();
	    AlignToCenter(YOffset);

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
