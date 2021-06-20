using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ServerLogin : MonoBehaviour
{
    public string mainScene;
    private Sdata sdata;
    public string tempName;

    public TMP_InputField nameField;
    public TMP_InputField serverField;

    public TextMeshProUGUI ErrorField;

    // Start is called before the first frame update
    void Start()
    {
        sdata = Sdata.sdata;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // reload the scene
    public void LoadMainScene(){
        sdata.serverURL = serverField.text;
        if(nameField.text.Length>0){
            tempName = nameField.text;
            string msg = "{\"name\":";
            msg += "\""+nameField.text+"\"}";
            Debug.Log("msg:"+msg);
            StartCoroutine(postToServer(msg,"reg"));
        }
        else{
            string error0 = "enter your name, please";
            ErrorField.text = error0;
        }
    }


    IEnumerator postToServer(string msg, string route) {
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www = new UnityWebRequest(sdata.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log("@@@error@@@");
            Debug.Log(www.error);
            string error0 = "ServerError:" + www.error;
            ErrorField.text = error0;
        }
        else {
            Debug.Log("@@@RESPONSE###");
            int data = int.Parse(www.downloadHandler.text);
            Debug.Log(data);
            if(data>=0){
                sdata.playerIndex=data;
                sdata.myName=tempName;
                SceneManager.LoadScene(mainScene);
            }
            else if(data==-1){
                string error0 = "Error:full";
                ErrorField.text = error0;
                Debug.Log(error0);
            }
            else if(data==-2){
                string error1 = "Error:NameAlreadyExists";
                ErrorField.text = error1;
                Debug.Log(error1);
            }

        }
    }

}
