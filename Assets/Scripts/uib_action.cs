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

    [SerializeField] private int actionNum;

    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
    }

    public void ActionPressed(){
        TargetAssignHelper.tah.DestroyMarkers();
        TargetAssignHelper.tah.moveError = false;
        actionSelectingUnit.SetActive(true);
        sdata.actionIndex=actionNum;

        // First Time
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="0"){
            moveButton.GetComponent<Image>().color = Color.white;
            fireButton.GetComponent<Image>().color = Color.white;
            movePage.SetActive(false);
            firePage.SetActive(false);
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }

        // if move already
        else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
            movePage.SetActive(true);
            firePage.SetActive(false);
            moveButton.GetComponent<Image>().color = moveButton.GetComponent<uib_actionType>().colorAfterPressed;
            fireButton.GetComponent<Image>().color = Color.white;
        }


        else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
            movePage.SetActive(false);
            firePage.SetActive(true);
            moveButton.GetComponent<Image>().color = Color.white;
            fireButton.GetComponent<Image>().color = fireButton.GetComponent<uib_actionType>().colorAfterPressed;
            //highlight the pre-assigned gunType in FirePage
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name==fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunTypeObj.transform.name)
                {
                    fireButtonS.transform.GetChild(i).GetComponent<Image>().color = fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().colorAfterPressed;
                }
                else{
                    fireButtonS.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }
        }

    }


}

