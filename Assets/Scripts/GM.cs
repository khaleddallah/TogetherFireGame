using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class GM : MonoBehaviour
{
    private Sdata sdata;
    public static GM gm;
    public GameObject PlayersNames;
    public GameObject waitingUI;

    public int submitTimeRef;
    public int submitTime;
    public TextMeshProUGUI SubmitTimeText;

    public GameObject ActionsUnit;
    public EpisodeMngr episodeMngr;

    public string firstSceneName;

    public Coroutine submitTimerRoutine = null;

    // private bool isStarted;

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

    // Update is called once per frame
    void Update()
    {

    }


 

    // check if started sign
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
            Debug.Log(data);
            Debug.Log("YEEES");
            waitingUI.SetActive(false);
            string[] players = data.Split('/');
            for(int i=1; i<players.Length; i++){
                PlayersNames.transform.GetChild(i-1).GetComponent<TextMeshProUGUI>().text = players[i];
            }

            ActionsUnit.SetActive(true);
            startSubmitCoroutine();
            www.Dispose();
        }
    }

    public void stopSubmitCoroutine(){
        StopCoroutine(submitTimerRoutine);
    }

    public void startSubmitCoroutine(){
        submitTimerRoutine = StartCoroutine(submitDownTimer());

    }

    public IEnumerator submitDownTimer() {
        while(submitTime>=0){
            SubmitTimeText.text = "EP "+ sdata.episodeIndex.ToString() +" : "+ submitTime.ToString();
            submitTime -= 1;
            yield return new WaitForSeconds(1f);
        }
        episodeMngr.submitPress();
        yield return null;
    }



    public void loadFirstScene(){
        Destroy(sdata.gameObject);
        SceneManager.LoadScene(firstSceneName);
    }


}
