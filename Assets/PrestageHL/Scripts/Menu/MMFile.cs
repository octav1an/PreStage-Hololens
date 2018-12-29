using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMFile : MonoBehaviour {


	void Start () {
		
	}
	
	void Update () {
		
	}

    public void SwitchSubButtons()
    {
        GameObject obj = transform.Find("SubButtons").gameObject;
        obj.SetActive(!obj.activeInHierarchy);
    }
}
