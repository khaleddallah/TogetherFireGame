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

    [SerializeField] private GameObject PlayersParent;
    [SerializeField] private GameObject middleSign;
    [SerializeField] private GameObject GoldParent;
    [SerializeField] private GameObject wizard;
    [SerializeField] private TextMeshProUGUI SubmitTimeText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color winColor;
    [SerializeField] private string firstSceneName;
    [SerializeField] private int submitTimeRef;

    private readonly string[] MIDDLE_SIGN_ANIMATIONS = {"waiting","epBefore"};
    int submitTime;
    Coroutine submitTimerRoutine;
    Sdata sdata;
    EpisodeMngr episodeMngr;
    List<int> deads;


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
        submitTime = submitTimeRef;
        deads = new List<int>();
        episodeMngr = GetComponent<EpisodeMngr>();

        Sdata.sdata.playerIndex = LongTermData.longTermData.playerIndex;
        sdata.vitalDatas[sdata.playerIndex].name = LongTermData.longTermData.myName;

        SetMiddleSignAnimation("waiting");
        StartCoroutine(PostIsStarted_GetPlayers());
    }




    private void SetMiddleSignAnimation(string animSelected){
        ResetMiddleSignAnimation();
        middleSign.GetComponent<Animator>().SetBool(animSelected, true);
    }
    
    private void ResetMiddleSignAnimation(){
        foreach(string animation in MIDDLE_SIGN_ANIMATIONS)
        {
            middleSign.GetComponent<Animator>().SetBool(animation, false);
        }
    }

    public void TurnOffMiddleSign(){
        middleSign.SetActive(false);
    }

    private void SetMiddleSignText(string x){
        middleSign.GetComponent<TextMeshProUGUI>().text = x;
    }

    IEnumerator PostIsStarted_GetPlayers() {
        string route = "isstarted";
        string msg = "{\"index\":\""+sdata.playerIndex.ToString()+"\",";
        msg += "\"howMuchPlayersStarted\":\""+sdata.howMuchPlayersStarted.ToString()+"\"}";
        UnityWebRequest www = PostRequest(msg, route);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = www.downloadHandler.text;
            string[] players = data.Split('/');
            sdata.howMuchPlayersStarted = players.Length;

            for(int i=0; i<players.Length; i++){
                sdata.vitalDatas[i].name  = players[i];
                EnableNewPlayer(i,players[i]);
            }

            if(players.Length==sdata.participantNum){
                ResetMiddleSignAnimation();
                SetMiddleSignText("");
                wizard.SetActive(true);
                www.Dispose();
            }
            else{
                www.Dispose();
                StartCoroutine(PostIsStarted_GetPlayers());
            }
        }
    }

    private void EnableNewPlayer(int playerIndex, string playerName){
        PlayersParent.transform.GetChild(playerIndex).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerName;
        PlayersParent.transform.GetChild(playerIndex).GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        PlayersParent.transform.GetChild(playerIndex).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    private UnityWebRequest PostRequest(string msg, string route){
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www = new UnityWebRequest(LongTermData.longTermData.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        return www;
    }

    public void stopSubmitCoroutine(){
        SubmitTimeText.text="";
        StopCoroutine(submitTimerRoutine);
    }

    public void startSubmitCoroutine(){
        submitTimerRoutine = StartCoroutine(submitDownTimer());
    }

    public IEnumerator submitDownTimer(){
        submitTime = submitTimeRef;
        while(submitTime>=0){
            SubmitTimeText.text = "Round Time : "+ submitTime.ToString()+" sec";
            submitTime -= 1;
            yield return new WaitForSeconds(1f);
        }
        episodeMngr.submitPress();
        yield return null;
    }

    public void loadFirstScene(){
        Destroy(sdata.gameObject);
        Destroy(TargetAssignHelper.tah.gameObject);
        Destroy(GM.gm.gameObject);
        SceneManager.LoadScene(firstSceneName);
    }

    public void updataMGH(){
        goldText.text = ""+sdata.vitalDatas[sdata.playerIndex].golds.ToString();
        for(int i=0; i<sdata.participantNum; i++){
            PlayersParent.transform.GetChild(i).GetChild(0).GetChild(0).GetChild(2).GetComponent<Slider>().value=sdata.vitalDatas[i].health/100;
        }
    }

    public int checkLose(){
        int losersNumber = 0;
        middleSign.GetComponent<TextMeshProUGUI>().color = loseColor;
        SetMiddleSignText("");
        for(int i = 0 ; i<sdata.participantNum ; i++){
            if (sdata.vitalDatas[i].health<=0){
                losersNumber+=1;
                if(!deads.Contains(i)){
                    ResetMiddleSignAnimation();
                    PlayersParent.transform.GetChild(i).gameObject.SetActive(false);
                    deads.Add(i);
                    string looserName = sdata.vitalDatas[i].name;
                    if(i==sdata.playerIndex){                        
                        middleSign.GetComponent<TextMeshProUGUI>().text += "YOU Lose\n";
                        StartCoroutine(sendLose());
                    }
                    else{
                        middleSign.GetComponent<TextMeshProUGUI>().text += looserName+" Lose\n";
                    }
                }
            }
        }


        if(losersNumber==sdata.participantNum-1){
            middleSign.SetActive(true);
            return 2;
        }

        else if(middleSign.GetComponent<TextMeshProUGUI>().text.Length>0){
            middleSign.SetActive(true);
            return 1;
        }

        else{
            return 0;
        }

    }

    public void Winner(){
        for(int i = 0 ; i<sdata.participantNum ; i++){
            if (sdata.vitalDatas[i].health>0){
                ResetMiddleSignAnimation();
                middleSign.GetComponent<Animator>().enabled=false;
                middleSign.GetComponent<TextMeshProUGUI>().color = winColor;
                if(i==sdata.playerIndex){
                    middleSign.GetComponent<TextMeshProUGUI>().text = "You WIIIIN\n";
                }
                else{
                    string winnerName = sdata.vitalDatas[i].name;
                    middleSign.GetComponent<TextMeshProUGUI>().text = winnerName+" WIIIIN\n";
                }
                break;
            }
        }
    }

    IEnumerator sendLose() {
        string route = "lose";
        string msg = "{\"pindex\":\""+sdata.playerIndex.ToString()+"\"}";
        UnityWebRequest www = PostRequest(msg, route);
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

    public IEnumerator CheckGoldWinner_LoadFirstScene(){
        if(GoldParent.transform.transform.childCount==1){

            int winner = GetMaxGoldWinner();
            Debug.Log("winner"+winner);

            middleSign.SetActive(true);
            ResetMiddleSignAnimation();
            middleSign.GetComponent<Animator>().enabled=false;
            middleSign.GetComponent<TextMeshProUGUI>().color = winColor;

            if(winner==sdata.playerIndex){
                SetMiddleSignText("You WIIIIN\n");
            }
            else{
                string winnerName = sdata.vitalDatas[winner].name;
                SetMiddleSignText(winnerName+" WIIIIN\n");
            }
            yield return new WaitForSeconds(3);
            middleSign.SetActive(false);
            loadFirstScene();
        }
        else{
            yield return null;
        }
    }

    private int GetMaxGoldWinner(){
        int winner = -1; 
        int maxGolds = 0; 
        for(int i=0 ; i<sdata.participantNum; i++){
            if(sdata.vitalDatas[i].golds>maxGolds){
                maxGolds = sdata.vitalDatas[i].golds;
                winner = i;
            }
        }
        return winner;
    }

    public void StartCheckGoldWinner(){
        StartCoroutine(CheckGoldWinner_LoadFirstScene());
    }

}
