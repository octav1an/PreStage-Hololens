using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMenu : MonoBehaviour
{
    private Plane menuPlane;
    public float yOffset = 0.3f;
    public float speed = 2;
    public float distMag = 2;
    private bool stopDirUpdate = false;
    private float maxAngle = 44;

	void Start () {
		
	}

	void Update ()
	{
	    ActivateMove();
        //if (!stopDirUpdate)AlightToForwardDir();
	    //AlightToForwardDir();

    }


    IEnumerator ActiveteMenuPosition()
    {
        Vector3 frw = Camera.main.transform.forward;
        Vector3 pos = new Vector3(frw.x * distMag, yOffset, frw.z * distMag);
        transform.position = new Vector3(Camera.main.transform.position.x, yOffset, Camera.main.transform.position.z);
        Vector3 saveCam = Camera.main.transform.position;
        float timeLeft = 3f;
        while (timeLeft >= 0.0f)
        {

            timeLeft -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, saveCam + pos, Time.deltaTime * speed);
            if(Vector3.Angle(Camera.main.transform.forward, Vector3.down) > maxAngle) yield break;
            yield return null;
        }
        
    }

    IEnumerator DeactivateMenuPosition()
    {
        float timeLeft = 3f;
        while (timeLeft >= 0.0f)
        {
            Vector3 pos = new Vector3(Camera.main.transform.position.x, yOffset, Camera.main.transform.position.z);
            timeLeft -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
            if (Vector3.Angle(Camera.main.transform.forward, Vector3.down) < maxAngle) yield break;
            yield return null;
        }
    }

    private void ActivateMove()
    {

        float angle = Vector3.Angle(Camera.main.transform.forward, Vector3.down);
        if (angle < maxAngle)
        {
            // make the radius bigger.
            distMag = 1.2f;

            if (!stopDirUpdate)
            {
                StartCoroutine(ActiveteMenuPosition());
            }
            stopDirUpdate = true;
        }
        else
        {
            // make the radius smaller.
            distMag = 0.2f;
            if (stopDirUpdate)
            {
                StartCoroutine(DeactivateMenuPosition());
            }
            stopDirUpdate = false;
        }
    }
}
