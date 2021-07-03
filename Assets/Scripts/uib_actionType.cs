using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class uib_actionType : MonoBehaviour
{
    [SerializeField] private GameObject movePage;
    [SerializeField] private GameObject firePage;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject fireButton;

    [SerializeField] private GameObject ActionParent;

    public string actionType;
    public Color colorAfterPressed;
    public Sprite moveIcon;

    Sdata sdata;

    Color dmoveColor;
    Color dfireColor;
    Color dgunColor;

    void Start()
    {
        sdata = Sdata.sdata;
        dmoveColor = moveButton.GetComponent<Image>().color;
        dfireColor = fireButton.GetComponent<Image>().color;
        // fireButton.GetComponent<Image>().color = Color.white;
        // moveButton.GetComponent<Image>().color = Color.white;
    }



    public void ActionTypePressed(){
        TargetAssignHelper.tah.DestroyMarkers();
        if(actionType=="move"){

            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
                Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
                for(int i = sdata.actionIndex ; i<sdata.actionsNum ; i++){
                    resetAction(i);
                }
            }
            else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
                TargetAssignHelper.tah.moveError = true;
            }
            movePage.SetActive(true);
            firePage.SetActive(false);
            GetComponent<Image>().color = colorAfterPressed;
            fireButton.GetComponent<Image>().color = dfireColor;

            ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<Image>().color = Color.white;
            ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<Image>().sprite = moveIcon;
        }
        
        else if(actionType=="fire"){
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
                Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
                // TargetAssignHelper.tah.moveError = true;
                for(int i = sdata.actionIndex ; i<sdata.actionsNum ; i++){
                    resetAction(i);
                }

            }
            movePage.SetActive(false);
            firePage.SetActive(true);
            GetComponent<Image>().color = colorAfterPressed;
            moveButton.GetComponent<Image>().color = dmoveColor;

        }
            
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type = actionType;


    }


    public void resetAction(int ind){
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].type="0"; // type of the action (move | fire)
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].target = new Vector3 (-999f,-999f,-999f); // target of the action either move or fire
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].gunTypeObj = null;
        ActionParent.transform.GetChild(ind).transform.GetChild(0).GetComponent<Image>().color = new Color(0f,0f,0f,0f);
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj);
        }
    }
}
