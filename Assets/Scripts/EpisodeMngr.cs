using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EpisodeMngr : MonoBehaviour
{

    public float actionTime = 3.0f; // How action will take
    public GameObject playersParent; // The parents of players

    public int player = 0 ; // index of the current player
    public int action = 0 ; // index of the current action

    public bool previewPressed = false; // true when the "Preview" pressed, until the preview process ends.
    public bool working = false; // working indicator of single action.

    public bool submitPressed = true;

    public List<GameObject> gunTypesObjs = new List<GameObject>();

    private Sdata sdata;
    public Button SubmitButton;

    void Start()
    {
        sdata = Sdata.sdata;
    }

    void Update()
    {
        if(previewPressed){
            run1PlayerEpisode();
        }
    }

    // move object to distination
    public IEnumerator move(GameObject playerObj, GameObject dest) {
        Vector3 movementDir =  (dest.transform.position-playerObj.transform.position).normalized;         
        float distance = Vector3.Distance(dest.transform.position, playerObj.transform.position);
        float speed0 = distance/actionTime;
        float t0 = 0f;
        while (t0<actionTime) {
            playerObj.transform.position += Time.deltaTime * movementDir * speed0;
            t0+=Time.deltaTime;
            yield return null;
        }
        working=false;
        action+=1;
    }


    // fire bullet from playerObj to dest
    public IEnumerator fire(GameObject playerObj, GameObject dest, GameObject bullet) {
        GameObject x = Instantiate(bullet) as GameObject;
        x.transform.position = playerObj.transform.position;
        x.GetComponent<Gun_bullet>().dest = dest;
        StartCoroutine(x.GetComponent<Gun_bullet>().fire());
        yield return null;
    }


    // part of the preview function
    public void run1PlayerEpisode(){
        if(action<Sdata.sdata.actionsNum){
            if (working){
                return;
            }
            else if(Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[player].actions[action].type == "move")
            {
                working=true;
                StartCoroutine(move(
                    playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                    Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[player].actions[action].target
                ));
            }   
            else if(Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[action].type == "fire")
            {
                working=true;
                StartCoroutine(fire(
                    playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                    Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[player].actions[action].target,
                    Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[action].gunTypeObj
                ));
            }
            else
            {
                action+=1;
                return;
            }
        }
        else{
            previewPressed=false;
        }
    }
    

    // when Preview pressed
    public void previewPress(){
        submitPressed=false;
        previewPressed=true;
    }

    // when Submit pressed
    public void submitPress(){
        submitPressed=true;
        previewPressed=false;
        submitCurrentPlayerRoleplay();
    }



    // post the data of the current player
    IEnumerator submitpostToServer(string msg, string route) {
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www = new UnityWebRequest(sdata.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log("@@@error@@@");
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = (www.downloadHandler.text);
            Debug.Log(data);
            Debug.Log("@@@RESPONSE###");
            fillEpisode(data);
            www.Dispose();
        }
    }


    // post the data of the current player
    void submitCurrentPlayerRoleplay() {
        string msg = "{";
        msg+= "\"pindex\":";
        msg+= "\""+sdata.playerIndex.ToString()+"\"" ;
        msg+= ",";
        msg+="\"data\":[";
        foreach(Action a in sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions){
            msg+= "{\"type\":\""+a.type+"\"";
            if(a.gunTypeObj){
                msg+= ",";
                msg+= "\"gunType\":\""+a.gunTypeObj.transform.name+"\"";
            }

            if(a.target){
                msg+= ",";
                msg+= "\"target\":\"";
                msg+= a.target.transform.position.x.ToString()+"/";
                msg+= a.target.transform.position.y.ToString()+"/";
                msg+= a.target.transform.position.z.ToString();
                msg+= "\"";
            }
            msg+= "},";
        }
        msg = msg.Substring(0,msg.Length-1);
        msg+="]}";
        Debug.Log(msg);
        SubmitButton.interactable = false;
        StartCoroutine(submitpostToServer(msg,"submit"));
    }



    // file actions of OTHER players
    void fillEpisode(string x){
        Debug.Log(x);
        int playerInd = 0; 
        int actionInd = 0; 
        Episode ep = JsonUtility.FromJson<Episode>(x);
        foreach (Roleplay rp in ep.roleplays){
            actionInd=0;
            foreach(Action a in rp.actions){
                sdata.episodes[Sdata.sdata.episodeIndex].roleplays[playerInd].actions[actionInd].type = a.type;
                string[] data = a.ser.Split(char.Parse("/"));
                if(a.type=="fire"){
                    sdata.episodes[Sdata.sdata.episodeIndex].roleplays[playerInd].actions[actionInd].gunTypeObj = getGameObjByName(gunTypesObjs,data[0]);
                    if(data[0]!="Sword"){
                        GameObject targetTemp = new GameObject();
                        targetTemp.transform.position = new Vector3(
                            float.Parse(data[1]),
                            float.Parse(data[2]),
                            float.Parse(data[3]));
                        Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;
                    }
                }
                else if(a.type=="move"){
                    GameObject targetTemp = new GameObject();
                    targetTemp.transform.position = new Vector3(
                        float.Parse(data[0]),
                        float.Parse(data[1]),
                        float.Parse(data[2]));
                    Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;   
                }
                actionInd+=1;
            }
            playerInd+=1;
        }
    }


    // get object by string from list of objects
    GameObject getGameObjByName(List<GameObject> list0, string x){
        foreach (GameObject gob in list0){
            if(x==gob.transform.name){
                return(gob);
            }
            else{
                continue;
            }
        }
        Debug.Log("error");
        return(new GameObject());
    }




}
