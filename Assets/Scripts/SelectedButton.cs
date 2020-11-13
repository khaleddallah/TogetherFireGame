using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectedButton : MonoBehaviour
{
    [SerializeField] private GameObject movFireList;
    [SerializeField] private GameObject gunList;

    [SerializeField] private ActionsData aD;

    public void TglMovFire(){
        if(movFireList.activeSelf){
            movFireList.SetActive(false);
            gunList.SetActive(false);
        }
        else{
            movFireList.transform.position = new Vector3(movFireList.transform.position.x, transform.position.y, 0f);
            movFireList.SetActive(true);
            aD.actionTempNum = int.Parse(transform.parent.name);
        }
    }


}
