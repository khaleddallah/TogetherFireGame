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
            TextMeshProUGUI xt = ActionParent.transform.GetChild(Sdata.sdata.actionIndex).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            xt.text = "move";
            xt.color = colorAfterPressed;
        }
        
        else if(actionType=="fire"){
            movePage.SetActive(false);
            firePage.SetActive(true);
            GetComponent<Image>().color = colorAfterPressed;
            moveButton.GetComponent<Image>().color = Color.white;
            GameObject x = Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].target;
            Destroy(x);
        }
            
        Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type = actionType;


    }
}
