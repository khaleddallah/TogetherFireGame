using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressOutSide : MonoBehaviour
{
    public GameObject actionSelectingUnit;

    void OnMouseDown(){
        Debug.Log("outside");
        actionSelectingUnit.SetActive(false);
        TargetAssignHelper.tah.DestroyMarkers();
    }
}
