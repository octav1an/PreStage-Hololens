using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public delegate void OnMouseDownGlobal();
    public static event OnMouseDownGlobal MouseDownGlobal;

    public delegate void OnMouseUpGlobal();
    public static event OnMouseUpGlobal MouseUpGlobal;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //MouseDownGlobal();
        }

        if (Input.GetMouseButtonUp(0))
        {
            //MouseUpGlobal();
        }
    }


}
