using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class FirstSpatialMapping : MonoBehaviour {

	
	void Start ()
	{
	    Manager.Instance.SpatialMappingGo.SetActive(true);
    }
	
	void Update () {
	    if (Time.fixedTime > 8)
	    {
	        Manager.Instance.SpatialMappingGo.SetActive(false);
	        this.enabled = false;
	        Debug.Log("Scanning Done!!");
        }
    }
}
