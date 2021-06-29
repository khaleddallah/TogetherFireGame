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
        submitTime = submitTimeRef;
        episodeMngr = GetComponent<EpisodeMngr>();
        sdata = Sdata.sdata;
        PlayersNames.transform.GetChild(sdata.playerIndex).GetComponent<TextMeshProUGUI>().text = sdata.myName;
        StartCoroutine(postIsStarted());
    }

 

    // send ifstarted signal & get players Names and Indexes
    IEnumerator postIsStarted() {
        string route = "isstarted";
        string msg = "{\"index\":\""+sdata.playerIndex.ToString()+"\"}";
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UnityWebRequest www = new UnityWebRequest(sdata.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = www.downloadHandler.text;
            Debug.Log("IsStartedResponse::"+data);

            waitingUI.SetActive(false);
            ActionsUnit.SetActive(true);

            string[] players = data.Split('/');
            for(int i=1; i<players.Length; i++){
                PlayersNames.transform.GetChild(i-1).GetComponent<TextMeshProUGUI>().text = players[i];
            }

            startSubmitCoroutine();
            www.Dispose();
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
        SceneManager.LoadScene(firstSceneName);
    }

    // Get gold
    public void updataMGH(){
        gold.text = "GOLDS : "+sdata.vitalDatas[sdata.playerIndex].golds.ToString();
        health.text = "HEALTH : "+sdata.vitalDatas[sdata.playerIndex].health.ToString();

    }


}
