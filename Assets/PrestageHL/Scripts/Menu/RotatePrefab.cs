using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatePrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsHighlighted;
    public GameObject prefabGo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (IsHighlighted)
	    {
            prefabGo.transform.Rotate(prefabGo.transform.up, Time.deltaTime * 50, Space.World);
        }
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHighlighted = false;
        prefabGo.transform.localRotation = Quaternion.identity;
    }
}
