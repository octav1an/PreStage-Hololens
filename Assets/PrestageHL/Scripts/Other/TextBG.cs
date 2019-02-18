using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBG : MonoBehaviour
{
    private GameObject EdgeDimGO;

	void Start ()
	{
	    EdgeDimGO = transform.parent.gameObject;
	    GetComponent<TextMesh>().text = EdgeDimGO.GetComponent<TextMesh>().text;
	}

	void Update ()
	{
	    UpdateText();
	}

    private void UpdateText()
    {
        if (GetComponent<TextMesh>().text != EdgeDimGO.GetComponent<TextMesh>().text)
        {
            GetComponent<TextMesh>().text = EdgeDimGO.GetComponent<TextMesh>().text;
        }
    }
}
