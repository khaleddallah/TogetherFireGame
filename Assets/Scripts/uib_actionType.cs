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

    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
        fireButton.GetComponent<Image>().color = Color.white;
        moveButton.GetComponent<Image>().color = Color.white;
    }



    public void ActionTypePressed(){
        TargetAssignHelper.tah.DestroyMarkers();
        if(actionType=="move"){
            TargetAssignHelper.tah.moveError = true;
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
                Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
            }
            movePage.SetActive(true);
            firePage.SetActive(false);
            GetComponent<Image>().color = colorAfterPressed;
            fireButton.GetComponent<Image>().color = Color.white;
            TextMeshProUGUI xt = ActionParent.transform.GetChild(sdata.actionIndex).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            xt.text = "move";
            xt.color = colorAfterPressed;


        }
        
        else if(actionType=="fire"){
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
                Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
                TargetAssignHelper.tah.moveError = true;
            }
            movePage.SetActive(false);
            firePage.SetActive(true);
            GetComponent<Image>().color = colorAfterPressed;
            moveButton.GetComponent<Image>().color = Color.white;

        }
            
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type = actionType;


    }
}
