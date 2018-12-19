using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityCollider : MonoBehaviour
{

    public List<GameObject> closeBlocksColl = new List<GameObject>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "ProximityCollider")
        {
            //print("Enter - Collider tag: " + collider.tag);
            //print("BlockID: " + collider.GetComponentInParent<BlockPrim>().blockID);
            closeBlocksColl.Add(collider.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "ProximityCollider")
        {
            //print("Exit - Collider tag: " + collider.tag);
            //print("BlockID: " + collider.GetComponentInParent<BlockPrim>().blockID);
            closeBlocksColl.Remove(collider.transform.parent.gameObject);
        }
    }

    void OnGUI()
    {
        GUI.color = new Color(1f, 0.1f, 0f, 1f);
        if(this.transform.parent.name == "Block" &&
            closeBlocksColl.Count > 0)GUI.Label(new Rect(20, 50, 400, 100), ("ProxiBlock 0: " + closeBlocksColl[0].GetComponent<BlockPrim>().BlockId));
        if (this.transform.parent.name == "Block" &&
            closeBlocksColl.Count == 2) GUI.Label(new Rect(20, 70, 400, 100), ("ProxiBlock 1: " + closeBlocksColl[1].GetComponent<BlockPrim>().BlockId));
    }
}
