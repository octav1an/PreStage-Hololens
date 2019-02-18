using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        print("ENterCol");
        foreach (ContactPoint contact in collision.contacts)
        {
            print("Enter");
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

    }

    //void OnCollisionStay(Collision collisionInfo)
    //{
    //    print("Stay");
    //    // Debug-draw all contact points and normals
    //    foreach (ContactPoint contact in collisionInfo.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{

    //    //Debug.Log("entered");
    //}
}
