using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneContentPR : MonoBehaviour {

    public static SceneContentPR Instance;
    public GameObject SceneContentGo
    {
        get { return GetComponent<GameObject>().gameObject; }
    }

    private void Awake()
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
	
	void Update () {
		
	}
}
