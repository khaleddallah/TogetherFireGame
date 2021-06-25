using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class uib_gunType : MonoBehaviour
{
    [SerializeField] private GameObject fireButtonS;
    [SerializeField] private GameObject ActionParent;

    public GameObject gunTypeObj;
    public Color colorAfterPressed;

    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
        GetComponent<Image>().color = Color.white;
    }


    public void GunTypePressed(){
        GetComponent<Image>().color = colorAfterPressed;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj = gunTypeObj; 
        
        for(int i=0; i<fireButtonS.transform.childCount; i++){
            if(fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunTypeObj.transform.name!=gunTypeObj.transform.name){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }

        TextMeshProUGUI xt = ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        xt.text = gunTypeObj.transform.name;
        xt.color = colorAfterPressed;
    }
}
