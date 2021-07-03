using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

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



    void Awake()
    {
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
        sdata = Sdata.sdata;
        sdata.playerIndex = LTD.ltd.playerIndex;
        submitTime = submitTimeRef;
        episodeMngr = GetComponent<EpisodeMngr>();
        PlayersNames.transform.GetChild(sdata.playerIndex).GetComponent<TextMeshProUGUI>().text = LTD.ltd.myName;
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
            }

            // if all player started start            
            if(players.Length==sdata.participantNum){
                waitingUI.SetActive(false);
                ActionsUnit.SetActive(true);
                startSubmitCoroutine();
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
        StopCoroutine(submitTimerRoutine);
    }

    public void startSubmitCoroutine(){
        submitTime = submitTimeRef;
        submitTimerRoutine = StartCoroutine(submitDownTimer());

    }

    // submit Timer
    public IEnumerator submitDownTimer() {
        while(submitTime>=0){
            SubmitTimeText.text = "EP "+ sdata.episodeIndex.ToString() +" : "+ submitTime.ToString();
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
        gold.text = "Golds : "+sdata.vitalDatas[sdata.playerIndex].golds.ToString();
        health.text = "Health : "+sdata.vitalDatas[sdata.playerIndex].health.ToString()+"%";
    }


    public int checkLose(){
        int howmlose = 0;
        winLoseSign.GetComponent<TextMeshProUGUI>().text = "";
        for(int i = 0 ; i<sdata.participantNum ; i++){
            Debug.Log("chLose:i:"+i);
            Debug.Log("health:"+sdata.vitalDatas[i].health);
            if (sdata.vitalDatas[i].health<=0){
                howmlose+=1;
                if(PlayersParent.transform.GetChild(i).GetChild(0).gameObject.activeSelf){
                    string looserName = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
                    PlayersParent.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

                    Color x = PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color;
                    PlayersNames.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = new Color(x.r, x.g, x.b, 0.32f);
                    
                    if(i==sdata.playerIndex){
                        ActionsUnit.SetActive(false);
                        winLoseSign.GetComponent<TextMeshProUGUI>().text += "YOU Lose\n";
                        StartCoroutine(sendLose());        
                    }
                    else{
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
                winLoseSign.GetComponent<TextMeshProUGUI>().color = Color.green;
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


}
