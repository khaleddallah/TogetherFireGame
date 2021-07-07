using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wizard : MonoBehaviour
{

    public GameObject gunWindow;
    public GameObject submitButton;
    public GameObject resetButton;

    public TextMeshProUGUI helpText;
    public GameObject winLoseSign;

    public EpisodeMngr episodeMngr;
    public GameObject fireButtonS;

    Sdata sdata;
    Color dgunColor;
    Coroutine GetUserInputRoutine;

    // Start is called before the first frame update
    void Start()
    {
        sdata = Sdata.sdata;
        dgunColor = fireButtonS.transform.GetChild(0).GetComponent<Image>().color;
        gunWindow.SetActive(false);
     
        StartCoroutine(downTimerFirstEp());
        GetUserInputRoutine=StartCoroutine(GetUserInput());
    }   

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator GetUserInput(){
        Debug.Log("start getting User input");
        resetGunTypeColors();
        gunWindow.SetActive(false);
        submitButton.SetActive(false);
        resetButton.SetActive(false);

        yield return new WaitUntil(() => !winLoseSign.activeSelf);

        // ==== move act ====
        Debug.Log("Move Selecting");
        actionNumType(0,"move");
        helpText.text = "please choose WHERE to MOVE";
        TargetAssignHelper.tah.DestroyMarkers();
        TargetAssignHelper.tah.MoveMarkers();
        yield return new WaitUntil(() => sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target != new Vector3 (-999f,-999f,-999f) );


        // ==== fire act ====
        // choose gun
        Debug.Log("Fire Selecting");
        actionNumType(1,"fire");
        gunWindow.SetActive(true);
        resetButton.SetActive(true);
        helpText.text = "please choose your GUN and TARGET";

        // yield return new WaitUntil(() => sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj);
        // TargetAssignHelper.tah.DestroyMarkers();
        // TargetAssignHelper.tah.DrawTargetLines();
        yield return new WaitUntil(() => sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target != new Vector3 (-999f,-999f,-999f) );
        gunWindow.SetActive(false);

        helpText.text = "";
        submitButton.SetActive(true);
        resetButton.SetActive(true);
    }


    public void actionNumType(int actionNum, string mofi){
        sdata.actionIndex=actionNum;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type = mofi;
    }


    public void resetActionS(){
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

        winLoseSign.SetActive(true);
        winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
        winLoseSign.GetComponent<Animator>().SetBool("epBefore", true);

        float timeTempEp = episodeMngr.timeBeforeEp;
        winLoseSign.GetComponent<TextMeshProUGUI>().color = episodeMngr.epTimerColor;
        while(timeTempEp>0){
            winLoseSign.GetComponent<TextMeshProUGUI>().text = "Round "+(sdata.episodeIndex+1).ToString()+" starts in "+timeTempEp.ToString();
            yield return new WaitForSeconds(1);
            timeTempEp-=1;
        }
        winLoseSign.GetComponent<TextMeshProUGUI>().text = "";
        winLoseSign.SetActive(false);
        GM.gm.startSubmitCoroutine();
    }


    public void resetGunTypeColors(){
        for(int i=0; i<fireButtonS.transform.childCount; i++){
            fireButtonS.transform.GetChild(i).GetComponent<Image>().color = dgunColor;
        }
    }

}
