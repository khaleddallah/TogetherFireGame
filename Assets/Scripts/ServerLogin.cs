using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ServerLogin : MonoBehaviour
{
    [SerializeField] private string mainScene;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField serverInputField;
    [SerializeField] private TextMeshProUGUI ErrorText;

    string tempName;
    LongTermData longTermData;

    // Start is called before the first frame update
    void Start()
    {
        longTermData = LongTermData.longTermData;
        nameInputField.text = longTermData.myName;
        serverInputField.text = longTermData.serverURL;
        StartCoroutine(PostGetPlayersNum());
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadMainScene();
        }
    }


    public void LoadMainScene(){
        longTermData.serverURL = serverInputField.text;
        if(CheckNameValid()){
            StartCoroutine(PostReg_LoadMainScene());
        }
        else{
            ShowError("enter your name, please");
        }
    }

    private bool CheckNameValid(){
        return nameInputField.text.Length>0;
    }

    private void ShowError(string e){
        ErrorText.text = e;
    }

    IEnumerator PostReg_LoadMainScene() {
        yield return new WaitUntil(() => longTermData.participantNum!=-1);
        string route = "reg";
        string msg = "{\"name\":\""+nameInputField.text+"\"}";
        UnityWebRequest www = PostRequest(msg, route);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            ShowError("ServerError:" + www.error);
            www.Dispose();
        }
        else {
            int data = int.Parse(www.downloadHandler.text);
            Debug.Log("res:reg:"+data);
            if(data>=0){
                longTermData.myName=nameInputField.text;
                longTermData.playerIndex=data;
                SceneManager.LoadScene(mainScene);
            }
            else if(data==-1){
                ShowError("Error:full");
            }
            else if(data==-2){
                ShowError("Error:NameAlreadyExists");
            }
            www.Dispose();
        }
    }



    IEnumerator PostGetPlayersNum() {
        string route = "playerNum";
        string msg = "{\"ignore\":\"1\"}";
        UnityWebRequest www = PostRequest(msg, route);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = www.downloadHandler.text;
            longTermData.participantNum = int.Parse(data);
            www.Dispose();
        }
    }


    private UnityWebRequest PostRequest(string msg, string route){
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www = new UnityWebRequest(longTermData.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        return www;
    }

}
