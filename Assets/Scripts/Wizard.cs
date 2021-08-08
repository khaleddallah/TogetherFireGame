using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wizard : MonoBehaviour
{

    [SerializeField] private GameObject gunWindow;
    [SerializeField] private GameObject submitButton;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject middleSign;
    [SerializeField] private EpisodeMngr episodeMngr;
    [SerializeField] private GameObject fireButtonsParent;
    [SerializeField] private GameObject GoldParent, DiamondParent, StoneParent, PlayersParent;
    [SerializeField] private TextMeshProUGUI helpText;

    Sdata sdata;
    Color defaultGunColor;
    Coroutine GetUserInputRoutine;

    // Start is called before the first frame update
    void Start()
    {
        sdata = Sdata.sdata;
        defaultGunColor = fireButtonsParent.transform.GetChild(0).GetComponent<Image>().color;
        gunWindow.SetActive(false);
        StartCoroutine(downTimerFirstEp());
        GetUserInputRoutine=StartCoroutine(GetUserInput());
    }   



    public IEnumerator GetUserInput(){
        ResetGunTypeColors();
        DisableUI();
        yield return new WaitUntil(() => !middleSign.activeSelf);

        // ==== move act ====
        Debug.Log("Move Selecting");
        SetActionNumType(0,"move");
        helpText.text = "please choose WHERE to MOVE";
        TargetAssignHelper.tah.DrawMoveMarkers();
        yield return new WaitUntil(() => sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target != new Vector3(-999f,-999f,-999f) );

        // ==== fire act ====
        Debug.Log("Fire Selecting");
        SetActionNumType(1,"fire");
        gunWindow.SetActive(true);
        resetButton.SetActive(true);
        helpText.text = "please choose your GUN and TARGET";
        yield return new WaitUntil(() => sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target != new Vector3 (-999f,-999f,-999f) );
        
        
        gunWindow.SetActive(false);
        helpText.text = "";
        submitButton.SetActive(true);
        resetButton.SetActive(true);
    }


    private void SetActionNumType(int actionNum, string mofi){
        sdata.actionIndex=actionNum;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type = mofi;
    }

    private void DisableUI(){
        gunWindow.SetActive(false);
        submitButton.SetActive(false);
        resetButton.SetActive(false);
    }

    private void ResetGunTypeColors(){
        for(int i=0; i<fireButtonsParent.transform.childCount; i++){
            fireButtonsParent.transform.GetChild(i).GetComponent<Image>().color = defaultGunColor;
        }
    }

    public void ResetActions(){
        for(int ind = 0 ; ind<sdata.actionsNum ; ind++){
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].type="0"; // type of the action (move | fire)
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].target = new Vector3 (-999f,-999f,-999f); // target of the action either move or fire
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].gunTypeObj = null;
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj){
                Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj);
            }        
        }
        StopCoroutine(GetUserInputRoutine);
        GetUserInputRoutine=StartCoroutine(GetUserInput());
    }

    public IEnumerator downTimerFirstEp(){
        middleSign.SetActive(true);
        middleSign.GetComponent<Animator>().SetBool("waiting", false);
        middleSign.GetComponent<Animator>().SetBool("epBefore", true);
        middleSign.GetComponent<TextMeshProUGUI>().color = episodeMngr.epTimerColor;

        float timeTempEp = episodeMngr.timeBeforeEp;
        while(timeTempEp>0){
            middleSign.GetComponent<TextMeshProUGUI>().text = "Round "+(sdata.episodeIndex+1).ToString()+" starts in "+timeTempEp.ToString();
            yield return new WaitForSeconds(1);
            timeTempEp-=1;
        }
        middleSign.GetComponent<TextMeshProUGUI>().text = "";
        middleSign.SetActive(false);
        GM.gm.startSubmitCoroutine();
    }






}
