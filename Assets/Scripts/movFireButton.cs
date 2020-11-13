using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class movFireButton : MonoBehaviour
{
    [SerializeField] private RectTransform movFireList;
    [SerializeField] private RectTransform gunList;
    [SerializeField] private float offset;

    [SerializeField] private ActionsData aD;

    public GameObject tmov;


    public void TglGun(){
        if(gunList.gameObject.activeSelf){
            gunList.gameObject.SetActive(false);
        }
        else{
            // gunList.transform.position = new Vector3(gunList.transform.position.x, transform.position.y+offset, 0f);
            gunList.anchoredPosition = new Vector2(gunList.anchoredPosition.x, movFireList.anchoredPosition.y+offset);
            gunList.gameObject.SetActive(true);
            aD.actions[aD.actionTempNum-1].type="fire";
        }
    }

    public void move(){
        //hide lists
        movFireList.gameObject.SetActive(false);
        
        //save mov as main action
        Debug.Log("mov: actions Num: "+ (aD.actionTempNum-1));
        aD.actions[aD.actionTempNum-1].type="move";

        //change label

        //run choose MovTarget process
        GameObject x = Instantiate(target) as GameObject;
        x.transform.position = new Vector3(aD.actionTempNum-2.0f, 0f, 0f);
    }

}
