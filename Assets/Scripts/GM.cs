using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
public class GM : MonoBehaviour
{
    public static GM gm;

    public GameObject PlayersNames;
    public GameObject PlayersParent;
    public GameObject winLoseSign;
    public GameObject waitingUI;

    public int submitTimeRef;
    public int submitTime;
    public TextMeshProUGUI SubmitTimeText;

    public GameObject ActionsUnit;

    public string firstSceneName;

    Coroutine submitTimerRoutine;
    Sdata sdata;
    EpisodeMngr episodeMngr;

    public TextMeshProUGUI money;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI health;

    public GameObject GoldParent;

    public Color loseColor;
    public Color winColor;
    public GameObject wizard;

    List<int> deadfuneral;

    void Awake()
    {
        deadfuneral = new List<int>();
        try{
            Sdata.sdata.playerIndex = LTD.ltd.playerIndex;
        }

        catch (Exception e) {
            Sdata.sdata.playerIndex = 0;
            print("error:"+e);
        }  

        if(gm != null){
            GameObject.Destroy(gm);
        }
        else{
            gm = this;
        }

        DontDestroyOnLoad(this);
    }



    void Start()
    {
        winLoseSign.GetComponent<Animator>().SetBool("waiting", true);
        winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);

        sdata = Sdata.sdata;
        submitTime = submitTimeRef;
        episodeMngr = GetComponent<EpisodeMngr>();
        try{
            PlayersNames.transform.GetChild(sdata.playerIndex).GetComponent<TextMeshProUGUI>().text = LTD.ltd.myName;
        }
        catch(Exception e) {
            PlayersNames.transform.GetChild(sdata.playerIndex).GetComponent<TextMeshProUGUI>().text = "tester0";
            print("error:"+e);
        }
        
        StartCoroutine(postIsStarted());
    }

 

    // send ifstarted signal & get players Names and Indexes
    IEnumerator postIsStarted() {
        string route = "isstarted";

        string msg = "{\"index\":\""+sdata.playerIndex.ToString()+"\",";
        msg += "\"howmplayerstarted\":\""+sdata.howmplayerstarted.ToString()+"\"}";

        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(LTD.ltd.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = www.downloadHandler.text;
            // Debug.Log("IsStartedResponse::"+data);



            string[] players = data.Split('/');
            sdata.howmplayerstarted = players.Length;

            for(int i=0; i<players.Length; i++){
                PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = players[i];
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = players[i];
                PlayersParent.transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
            }

            // if all player started start            
            if(players.Length==sdata.participantNum){
                winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
                winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);
                winLoseSign.GetComponent<TextMeshProUGUI>().text = "";
                // winLoseSign.SetActive(false);
                wizard.SetActive(true);
                ActionsUnit.SetActive(true);
                // startSubmitCoroutine();
                www.Dispose();
            }
            // if not all player started re-request
            else{
                www.Dispose();
                StartCoroutine(postIsStarted());
            }

  
        }
    }

    public void stopSubmitCoroutine(){
        SubmitTimeText.text="";
        StopCoroutine(submitTimerRoutine);
    }

    public void startSubmitCoroutine(){
        submitTime = submitTimeRef;
        submitTimerRoutine = StartCoroutine(submitDownTimer());

    }

    // submit Timer
    public IEnumerator submitDownTimer() {
        while(submitTime>=0){
            SubmitTimeText.text = "Round Time : "+ submitTime.ToString()+" sec";
            submitTime -= 1;
            yield return new WaitForSeconds(1f);
        }
        episodeMngr.submitPress();
        yield return null;
    }


    // function of the home button ( in Participants names corner)
    public void loadFirstScene(){
        Destroy(sdata.gameObject);
        Destroy(TargetAssignHelper.tah.gameObject);
        Destroy(GM.gm.gameObject);

        SceneManager.LoadScene(firstSceneName);
    }

    // Get gold
    public void updataMGH(){
        Debug.Log("LLL:"+sdata.vitalDatas[sdata.playerIndex].health);
        gold.text = ""+sdata.vitalDatas[sdata.playerIndex].golds.ToString();
        health.text = ""+sdata.vitalDatas[sdata.playerIndex].health.ToString()+"%";
        for(int i=0; i<sdata.participantNum; i++){
            PlayersParent.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(2).GetComponent<Slider>().value=sdata.vitalDatas[i].health/100;
        }
    }


    public int checkLose(){
        int howmlose = 0;
        winLoseSign.GetComponent<TextMeshProUGUI>().color = loseColor;
        winLoseSign.GetComponent<TextMeshProUGUI>().text = "";
        for(int i = 0 ; i<sdata.participantNum ; i++){
            Debug.Log("chLose:i:"+i);
            Debug.Log("health:"+sdata.vitalDatas[i].health);
            if (sdata.vitalDatas[i].health<=0){
                howmlose+=1;
                if(!deadfuneral.Contains(i)){
                    if(i==sdata.playerIndex){
                        // buyHealthWindow.SetActive(true);
                        
                        winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
                        winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);

                        string looserName = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
                        deadfuneral.Add(i);

                        Color x = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color;
                        PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = new Color(x.r, x.g, x.b, 0.32f);     
                        
                        ActionsUnit.SetActive(false);
                        winLoseSign.GetComponent<TextMeshProUGUI>().text += "YOU Lose\n";
                        StartCoroutine(sendLose());        
                    }
                    else{
                        winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
                        winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);

                        string looserName = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
                        deadfuneral.Add(i);

                        Color x = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color;
                        PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = new Color(x.r, x.g, x.b, 0.32f);
                        winLoseSign.GetComponent<TextMeshProUGUI>().text += looserName+" Lose\n";
                    }
                }
            }
        }


        if(howmlose==sdata.participantNum-1){
            winLoseSign.SetActive(true);
            return 2;
        }
        else if(winLoseSign.GetComponent<TextMeshProUGUI>().text.Length>0){
            winLoseSign.SetActive(true);
            return 1;
        }    
        else{
            return 0;
        }
    }

    public void winLoseSignOff(){
        winLoseSign.SetActive(false);
    }


    public void Winner(){
        for(int i = 0 ; i<sdata.participantNum ; i++){
            if (sdata.vitalDatas[i].health>0){
                winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
                winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);
                winLoseSign.GetComponent<Animator>().enabled=false;
                winLoseSign.GetComponent<TextMeshProUGUI>().color = winColor;
                if(i==sdata.playerIndex){
                    ActionsUnit.SetActive(false);
                    winLoseSign.GetComponent<TextMeshProUGUI>().text = "You WIIIIN\n";
                }
                else{
                    string winnerName = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
                    winLoseSign.GetComponent<TextMeshProUGUI>().text = winnerName+" WIIIIN\n";
                }
                break;

            }
        }
        winLoseSign.GetComponent<Animator>().enabled=false;

    }

    IEnumerator sendLose() {
        string route = "lose";
        string msg = "{\"pindex\":\""+sdata.playerIndex.ToString()+"\"}";
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(LTD.ltd.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = www.downloadHandler.text;
            Debug.Log("loseResponse::"+data);
            www.Dispose();
        }
    }

    public IEnumerator CheckGoldWinner(){
        Debug.Log("g#:"+GoldParent.transform.transform.childCount);
        if(GoldParent.transform.transform.childCount==1){
            Debug.Log("winner");
            int winner = -1; 
            int maxGolds = 0; 
            for(int i=0 ; i<sdata.participantNum; i++){
                if(sdata.vitalDatas[i].golds>maxGolds){
                    maxGolds = sdata.vitalDatas[i].golds;
                    winner = i;
                }
            }
            Debug.Log("winner"+winner);

            winLoseSign.SetActive(true);
            winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
            winLoseSign.GetComponent<Animator>().SetBool("epBefore", false);
            winLoseSign.GetComponent<Animator>().enabled=false;
            winLoseSign.GetComponent<TextMeshProUGUI>().color = winColor;
            if(winner==sdata.playerIndex){
                ActionsUnit.SetActive(false);
                winLoseSign.GetComponent<TextMeshProUGUI>().text = "You WIIIIN\n";
            }
            else{
                string winnerName = PlayersNames.transform.GetChild(winner).GetComponent<TextMeshProUGUI>().text;
                winLoseSign.GetComponent<TextMeshProUGUI>().text = winnerName+" WIIIIN\n";
            }
            yield return new WaitForSeconds(3);
            winLoseSign.SetActive(false);
            loadFirstScene();

        }
        else{
            yield return null;
        }
    }

    public void StartCheckGoldWinner(){
        StartCoroutine(CheckGoldWinner());
    }



}
