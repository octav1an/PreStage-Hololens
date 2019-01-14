using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTable : MonoBehaviour
{

    public static BaseTable Instance;
    public float Radius = 1;
    public float ScaleOffset = 0.2f;
    public Vector3 MENU_POSITION
    {
        get
        {
            Vector3 cameraProjected = GetProjectedOnPlane(GetBottomPlane(), Camera.main.transform.position);
            Vector3 diff = (cameraProjected - GetBottomCenter()).normalized * (GetColliderBounds().extents.magnitude + Radius);
            return GetBottomCenter() + diff;
        }
    }

    float scrollSpeed = 0.5f;
    Renderer rend;

    void Awake()
    {
        // Makes sure that I use always a game control even if my next scence already has one.
        // The instance of the object from the scene that is current will persist in the next scene.
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        // Turn on the elements of the table at Start.
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

	void Start () {
	    rend = GetComponent<Renderer>();
    }
	
	void Update ()
	{
	    SetupPosition();
        //print(GetColliderBounds().extents.magnitude);

	    
    }

    private void SetupPosition()
    {
        // Scale base table.
        float boundsScale = GetColliderBounds().extents.magnitude + ScaleOffset;
        transform.localScale = new Vector3(boundsScale, boundsScale, boundsScale);

        rend.material.SetTextureScale("_MainTex", new Vector2(boundsScale, boundsScale));
        // Position base table.
        transform.position = GetBottomCenter();
    }

    private Vector3 GetProjectedOnPlane(Plane pl, Vector3 loc)
    {
        return pl.ClosestPointOnPlane(loc);
    }

    private Plane GetBottomPlane()
    {
        return new Plane(Vector3.up, GetBottomCenter());
    }

    private Vector3 GetBottomCenter()
    {
        Vector3 center = GetColliderBounds().center;
        Vector3 extents = GetColliderBounds().extents;
        Vector3 botCenter = new Vector3(center.x, center.y - extents.y, center.z);
        return botCenter;
    }

    private Bounds GetColliderBounds()
    {

        if (Manager.Instance.CollGeoObjects.Count == 0) return new Bounds();
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
}
