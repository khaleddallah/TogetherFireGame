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
    public Sprite fireIcon;
 
    Sdata sdata;

    Color dgunColor;

    void Start()
    {
        sdata = Sdata.sdata;
        // GetComponent<Image>().color = Color.white;

        dgunColor = fireButtonS.transform.GetChild(0).GetComponent<Image>().color;
    }


    public void GunTypePressed(){
        TargetAssignHelper.tah.DestroyMarkers();
        GetComponent<Image>().color = colorAfterPressed;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj = gunTypeObj; 
        
        for(int i=0; i<fireButtonS.transform.childCount; i++){
            if(fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunTypeObj.transform.name!=gunTypeObj.transform.name){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = dgunColor;
            }
        }

        // TextMeshProUGUI xt = ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // xt.text = gunTypeObj.transform.name;
        // xt.color = colorAfterPressed;
        ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<Image>().sprite = fireIcon;

    }
}
