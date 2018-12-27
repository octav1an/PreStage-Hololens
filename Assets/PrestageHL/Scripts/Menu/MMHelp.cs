using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMHelp : MonoBehaviour {

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
