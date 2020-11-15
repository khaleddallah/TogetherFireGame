using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GunsButton : MonoBehaviour
{

    [SerializeField] private RectTransform movFireList;
    [SerializeField] private RectTransform gunList;
    [SerializeField] private float offset;
    [SerializeField] private ActionsData aD;

    public GameObject tfire;
    public GameObject targets;

    public Color color;

    public void ChooseGun(){

        //run choose ShootTarget process
        if(aD.actions[aD.actionTempNum-1].type=="0"){
            InstTarget();
        }
        else if(aD.actions[aD.actionTempNum-1].type=="move"){
            Destroy(aD.actions[aD.actionTempNum-1].target);
            InstTarget();
        }

        //save type of gun
        aD.actions[aD.actionTempNum-1].type="fire";
        aD.actions[aD.actionTempNum-1].gun=transform.name;
        

        //change label
        TextMeshProUGUI xt = aD.actions[aD.actionTempNum-1].txt.GetComponent<TextMeshProUGUI>();
        xt.text = transform.name;
        xt.color = color;

        //hide lists
        movFireList.gameObject.SetActive(false);
        gunList.gameObject.SetActive(false);
    }


    public void InstTarget(){
        GameObject x = Instantiate(tfire) as GameObject;
        
        x.transform.SetParent(targets.transform);

        TextMeshProUGUI xt = x.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = aD.actionTempNum.ToString("0");
        
        RectTransform xr = x.GetComponent<RectTransform>();
        xr.anchoredPosition = new Vector3(aD.actionTempNum-2.0f, 0f, 0f);

        aD.actions[aD.actionTempNum-1].target=x;
    }
}
