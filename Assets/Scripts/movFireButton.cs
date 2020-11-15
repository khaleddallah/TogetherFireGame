using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class movFireButton : MonoBehaviour
{
    [SerializeField] private RectTransform movFireList;
    [SerializeField] private RectTransform gunList;
    [SerializeField] private float offset;

    [SerializeField] private ActionsData aD;

    public GameObject tmov;
    public GameObject targets;

    public Color color;


    public void TglGun(){
        if(gunList.gameObject.activeSelf){
            gunList.gameObject.SetActive(false);
        }
        else{
            // gunList.transform.position = new Vector3(gunList.transform.position.x, transform.position.y+offset, 0f);
            gunList.anchoredPosition = new Vector2(gunList.anchoredPosition.x, movFireList.anchoredPosition.y+offset);
            gunList.gameObject.SetActive(true);
        }
    }

    public void move(){

        //run choose MovTarget process
        if(aD.actions[aD.actionTempNum-1].type=="0"){
            Debug.Log("### mov 0");
            InstTarget();
        }
        else if(aD.actions[aD.actionTempNum-1].type=="fire"){
            Debug.Log("### mov Gun");
            Destroy(aD.actions[aD.actionTempNum-1].target);
            InstTarget();
        }

        //save mov as main action
        Debug.Log("mov: actions Num: "+ (aD.actionTempNum-1));
        aD.actions[aD.actionTempNum-1].type="move";

        //change label
        TextMeshProUGUI xt = aD.actions[aD.actionTempNum-1].txt.GetComponent<TextMeshProUGUI>();
        xt.text = "move";
        xt.color = color;

        //hide lists
        movFireList.gameObject.SetActive(false);
    }


    public void InstTarget(){
        GameObject x = Instantiate(tmov) as GameObject;
        
        x.transform.SetParent(targets.transform);

        TextMeshProUGUI xt = x.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = aD.actionTempNum.ToString("0");

        RectTransform xr = x.GetComponent<RectTransform>();
        xr.anchoredPosition = new Vector3(aD.actionTempNum-2.0f, 0f, 0f);

        aD.actions[aD.actionTempNum-1].target=x;
    }
}
