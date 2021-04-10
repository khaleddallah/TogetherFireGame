using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uib_apply : MonoBehaviour
{
    [SerializeField] private GameObject actionSelectingUnit;

    public void ApplyPressed(){
        actionSelectingUnit.SetActive(false);
    }

}
