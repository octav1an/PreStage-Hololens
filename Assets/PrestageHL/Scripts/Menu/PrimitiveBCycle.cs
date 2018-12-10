using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveBCycle : MonoBehaviour
{
    private Vector3 _centerPos = new Vector3(0, -87.6f, 0);
    private Vector3 _centerScale = new Vector3(20, 20, 10);

    private Vector3 _leftPos = new Vector3(-46.7f, -75.3f, 0);
    private Vector3 _leftScale = new Vector3(18, 18, 10);

    private Vector3 _rightPos = new Vector3(46.7f, -75.3f, 0);
    private Vector3 _rightScale = new Vector3(18, 18, 10);

    public GameObject[] PrefabCollGo = new GameObject[5];
    public int _bLeftIndex;
    public int _bCenterIndex;
    public int _bRightIndex;

    #region Unity

    void Start ()
    {
        DefaultSetup();
    }
	
	void Update () {
		
	}
    #endregion // Unity

    #region Menu Calls
    public void CycleRight()
    {
        if(_bRightIndex == 0)
        {
            // Right
            AssignRightButton(_bRightIndex + 1);
            // Center
            AssignCenterButton(_bRightIndex - 1);
            // Left
            AssignLeftButton(PrefabCollGo.Length - 1);
        }
        else if (_bRightIndex < PrefabCollGo.Length - 1)
        {
            // Right
            AssignRightButton(_bRightIndex + 1);
            // Center
            AssignCenterButton(_bRightIndex - 1);
            // Left
            AssignLeftButton(_bRightIndex - 2);
        }
        else
        {
            // Right
            AssignRightButton(0);
            // Center
            AssignCenterButton(PrefabCollGo.Length - 1);
            // Left
            AssignLeftButton(PrefabCollGo.Length - 2);
        }
    }

    public void CycleLeft()
    {
        if (_bLeftIndex == 0)
        {
            // Left
            AssignLeftButton(PrefabCollGo.Length - 1);
            // Center
            AssignCenterButton(0);
            // Right
            AssignRightButton(1);
        }
        else if (_bLeftIndex < PrefabCollGo.Length - 1)
        {
            // Left
            AssignLeftButton(_bLeftIndex - 1);
            // Center
            AssignCenterButton(_bLeftIndex + 1);
            // Right
            AssignRightButton(_bLeftIndex + 2);
        }
        else
        {
            // Left
            AssignLeftButton(_bLeftIndex - 1);
            // Center
            AssignCenterButton(_bLeftIndex + 1);
            // Right
            AssignRightButton(0);
        }
    }
    #endregion // Menu Calls


    private void AssignLeftButton(int index)
    {
        // Close the current button.
        PrefabCollGo[_bLeftIndex].SetActive(false);
        // Assign the button.
        GameObject obj = PrefabCollGo[index];
        obj.SetActive(true);
        obj.transform.localPosition = _leftPos;
        obj.transform.localScale = _leftScale;
        _bLeftIndex = index;
    }

    private void AssignCenterButton(int index)
    {
        // Close the current button.
        PrefabCollGo[_bCenterIndex].SetActive(false);
        // Assign the button.
        GameObject obj = PrefabCollGo[index];
        obj.SetActive(true);
        obj.transform.localPosition = _centerPos;
        obj.transform.localScale = _centerScale;
        _bCenterIndex = index;
    }

    private void AssignRightButton(int index)
    {
        // Close the current button.
        PrefabCollGo[_bRightIndex].SetActive(false);
        // Assign the button.
        GameObject obj = PrefabCollGo[index];
        obj.SetActive(true);
        obj.transform.localPosition = _rightPos;
        obj.transform.localScale = _rightScale;
        _bRightIndex = index;
    }

    /// <summary>
    /// Default assignment of the buttons at start.
    /// </summary>
    private void DefaultSetup()
    {
        GameObject bLeft = PrefabCollGo[0];
        bLeft.SetActive(true);
        bLeft.transform.localPosition = _leftPos;
        bLeft.transform.localScale = _leftScale;
        _bLeftIndex = 0;

        GameObject bCenter = PrefabCollGo[1];
        bCenter.SetActive(true);
        bCenter.transform.localPosition = _centerPos;
        bCenter.transform.localScale = _centerScale;
        _bCenterIndex = 1;

        GameObject bRight = PrefabCollGo[2];
        bRight.SetActive(true);
        bRight.transform.localPosition = _rightPos;
        bRight.transform.localScale = _rightScale;
        _bRightIndex = 2;

    }
}
