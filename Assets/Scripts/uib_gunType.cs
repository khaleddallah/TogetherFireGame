using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class uib_gunType : MonoBehaviour
{
    [SerializeField] private GameObject gunTypeButtonsParent;
    [SerializeField] private GameObject gunTypeObj;
    [SerializeField] private Color colorAfterPressed;
    Color defaultGunColor;
    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
        defaultGunColor = gunTypeButtonsParent.transform.GetChild(0).GetComponent<Image>().color;
    }


    public void GunTypePressed(){
        ResetGunTypeButtonsColors();
        GetComponent<Image>().color = colorAfterPressed;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj = gunTypeObj; 
    }

    void ResetGunTypeButtonsColors(){
        for(int i=0; i<gunTypeButtonsParent.transform.childCount; i++){
            gunTypeButtonsParent.transform.GetChild(i).GetComponent<Image>().color = defaultGunColor;
        }
    }

}