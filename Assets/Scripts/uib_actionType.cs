using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class uib_actionType : MonoBehaviour
{
    [SerializeField] private GameObject movePage;
    [SerializeField] private GameObject firePage;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject fireButton;


    [SerializeField] private string actionType;

    public Color colorAfterPressed;

    // Start is called before the first frame update
    void Awake()
    {
        fireButton.GetComponent<Image>().color = Color.white;
        moveButton.GetComponent<Image>().color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActionTypePressed(){


        if(actionType=="move"){
            movePage.SetActive(true);
            firePage.SetActive(false);
            GetComponent<Image>().color = colorAfterPressed;
            fireButton.GetComponent<Image>().color = Color.white;
        }
        else if(actionType=="fire"){
            movePage.SetActive(false);
            firePage.SetActive(true);
            GetComponent<Image>().color = colorAfterPressed;
            moveButton.GetComponent<Image>().color = Color.white;
        }
            
        GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].type = actionType;


    }
}
