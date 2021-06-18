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

    public void ActionPressed(){
        actionSelectingUnit.SetActive(true);
        Sdata.sdata.actionIndex=actionNum;

        // Editing Time
        // First Time
        if(Sdata.sdata.gd.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.gd.playerIndex].actions[Sdata.sdata.actionIndex].type=="0"){
            moveButton.GetComponent<Image>().color = Color.white;
            fireButton.GetComponent<Image>().color = Color.white;
            movePage.SetActive(false);
            firePage.SetActive(false);
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }

        // if move already
        else if(Sdata.sdata.gd.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.gd.playerIndex].actions[Sdata.sdata.actionIndex].type=="move"){
            movePage.SetActive(true);
            firePage.SetActive(false);
            moveButton.GetComponent<Image>().color = moveButton.GetComponent<uib_actionType>().colorAfterPressed;
            fireButton.GetComponent<Image>().color = Color.white;
        }


        else if(Sdata.sdata.gd.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.gd.playerIndex].actions[Sdata.sdata.actionIndex].type=="fire"){
            movePage.SetActive(false);
            firePage.SetActive(true);
            moveButton.GetComponent<Image>().color = Color.white;
            fireButton.GetComponent<Image>().color = fireButton.GetComponent<uib_actionType>().colorAfterPressed;
            for(int i=0; i<fireButtonS.transform.childCount; i++){
                if(Sdata.sdata.gd.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.gd.playerIndex].actions[Sdata.sdata.actionIndex].gunTypeObj.transform.name==fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunTypeObj.transform.name)
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

