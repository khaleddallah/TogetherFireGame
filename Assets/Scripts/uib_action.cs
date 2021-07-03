using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class uib_action : MonoBehaviour
{
    [SerializeField] private GameObject actionSelectingUnit;
    [SerializeField] private GameObject movePage;
    [SerializeField] private GameObject firePage;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject fireButton;
    [SerializeField] private GameObject fireButtonS;
    [SerializeField] private GameObject ActionParent;

    [SerializeField] private int actionNum;

    Sdata sdata;
    Color dmoveColor;
    Color dfireColor;
    Color dgunColor;


    void Start()
    {
        sdata = Sdata.sdata;
        dmoveColor = moveButton.GetComponent<Image>().color;
        dfireColor = fireButton.GetComponent<Image>().color;
        dgunColor = fireButtonS.transform.GetChild(0).GetComponent<Image>().color;
    }

    public void ActionPressed(){
        checkActions();
        TargetAssignHelper.tah.DestroyMarkers();
        TargetAssignHelper.tah.moveError = false;
        actionSelectingUnit.SetActive(true);
        sdata.actionIndex=actionNum;

        // First Time
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="0"){
            moveButton.GetComponent<Image>().color = dmoveColor;
            fireButton.GetComponent<Image>().color = dfireColor;
            movePage.SetActive(false);
            firePage.SetActive(false);
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = dgunColor;
            }
        }

        // if move already
        else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
            movePage.SetActive(true);
            firePage.SetActive(false);
            moveButton.GetComponent<Image>().color = moveButton.GetComponent<uib_actionType>().colorAfterPressed;
            fireButton.GetComponent<Image>().color = dfireColor;
        }


        else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
            Debug.Log("fireBefor");
            movePage.SetActive(false);
            firePage.SetActive(true);
            moveButton.GetComponent<Image>().color = dmoveColor;
            fireButton.GetComponent<Image>().color = fireButton.GetComponent<uib_actionType>().colorAfterPressed;
            //highlight the pre-assigned gunType in FirePage
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name==fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunTypeObj.transform.name)
                {
                    fireButtonS.transform.GetChild(i).GetComponent<Image>().color = fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().colorAfterPressed;
                }
                else{
                    fireButtonS.transform.GetChild(i).GetComponent<Image>().color = dgunColor;
                }
            }
        }

    }

    public void checkActions(){
        for(int i=0; i<sdata.actionsNum; i++){
            string type0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].type;
            Vector3 target0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].target;
            GameObject gunTypeObj0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].gunTypeObj;
            if(type0=="move"){
                if(target0==new Vector3(-999f,-999f,-999f)){
                    Debug.Log("moveERRR:"+i);
                    resetAction(i);
                }
            }
            else if(type0=="fire"){
                if(!gunTypeObj0){
                    Debug.Log("FireERRR1:"+i);
                    resetAction(i);
                }
                else if(gunTypeObj0.transform.name!="Sword"){
                    if(target0==new Vector3(-999f,-999f,-999f)){
                        Debug.Log("FireERRR2:"+i);
                        resetAction(i);
                    }
                }
            }
        }
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


    public void resetCurrentAction(){
        for(int i = sdata.actionIndex ; i<sdata.actionsNum ; i++){
            resetAction(i);
            moveButton.GetComponent<Image>().color = dmoveColor;
            fireButton.GetComponent<Image>().color = dfireColor;
            movePage.SetActive(false);
            firePage.SetActive(false);
        }
    }


}

