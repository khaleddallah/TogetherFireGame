using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class EpisodeMngr : MonoBehaviour
{

    [SerializeField] private GameObject actionSelectingUnit;
    [SerializeField] private GameObject targetsParent;


    public float actionTime = 3.0f; // How action will take
    public GameObject playersParent; // The parents of players

    // public int player = 0 ; // index of the current player
    public int action = 0 ; // index of the current action

    public bool previewPressed = false; // true when the "Preview" pressed, until the preview process ends.
    public bool working = false; // working indicator of single action.

    public bool submitPressed = false;

    public bool IsPerformPhase = false;

    public List<GameObject> gunTypesObjs = new List<GameObject>();
    
    public GameObject ActionParent;


    private Sdata sdata;
    public Button SubmitButton;

    // public bool isSubmitPressed; 
    // public List<int> episodeSubmitted = new List<int>();
    public int episodeSubmitted = -1;

    public TextMeshProUGUI ActionNumUI;


    void Start()
    {
        episodeSubmitted = -1 ;
        sdata = Sdata.sdata;
    }

    void Update()
    {

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
        actionMove-=1;
    }




    // fire bullet from playerObj to dest
    public IEnumerator fire(int parent, GameObject playerObj, GameObject dest, GameObject bullet) {
        GameObject x = Instantiate(bullet) as GameObject;
        x.transform.position = playerObj.transform.position;
        x.GetComponent<Gun_bullet>().dest = dest;
        x.GetComponent<Gun_bullet>().myparent = parent;
        x.GetComponent<Gun_bullet>().Start0();
        yield return null;
    }


    

    // perform phase
    public int actionMove = 0;
    public int actionFire = 0;
    public IEnumerator performAllPlayers(int Findex, int participantNum){
        Debug.Log(">>>@@>");
        actionSelectingUnit.SetActive(false);
        GM.gm.stopSubmitCoroutine();
        GM.gm.SubmitTimeText.text = "EP "+ sdata.episodeIndex.ToString() +" : Working"; 

        // Time.timeScale = 1.0f;
        actionMove = 0 ;
        actionFire = 0 ;
        for(int actionT = 0 ; actionT<sdata.actionsNum ; actionT++){
            ActionNumUI.text = actionT.ToString();
            Debug.Log("actionT :: "+actionT.ToString());
            for(int player=Findex; player<participantNum ; player++){
                if(sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].type == "move")
                    {
                        Debug.Log("Moveplayer:::"+player.ToString());
                        actionMove+=1;
                        StartCoroutine(move(
                            playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                            sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].target
                        ));
                    } 
            }
            Debug.Log("waitin for MOVE");
            while(actionMove>0){
                yield return null;
            }
            for(int player=Findex; player<participantNum ; player++){
                if(sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].type == "fire")
                    {
                        actionFire+=1;
                        Debug.Log("Fireplayer:::"+player.ToString());
                        StartCoroutine(fire(
                            player,
                            playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                            sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].target,
                            sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].gunTypeObj
                        ));
                    } 
            }
            Debug.Log("waitin for FIRE");
            while(actionFire>0){
                yield return null;
            }
            actionMove = 0 ;
            actionFire = 0 ;
            yield return null;
        }
        ActionNumUI.text = "";
        sdata.CreateNewEpisode();

        resetEpisodeUI();
        GM.gm.submitTime = GM.gm.submitTimeRef;
        GM.gm.submitTimerRoutine = StartCoroutine(GM.gm.submitDownTimer());
    
    }
        



    // when Preview pressed

    public void previewPress(){
        StartCoroutine(performAllPlayers(sdata.playerIndex,sdata.playerIndex+1));
    }

    // when Submit pressed
    public void submitPress(){
        if(episodeSubmitted < sdata.episodeIndex){
            SubmitButton.interactable = false;
            episodeSubmitted = sdata.episodeIndex;
            submitCurrentPlayerRoleplay();
        }
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
            // IsPerformPhase=true;
            
            StartCoroutine(performAllPlayers(0, sdata.participantNum));
        }
    }


    // post the data of the current player
    public void submitCurrentPlayerRoleplay() {
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
        StartCoroutine(submitpostToServer(msg,"submit"));
    }



    // file actions of OTHER players
    void fillEpisode(string x){
        int playerInd = 0; 
        int actionInd = 0; 
        Episode ep = JsonUtility.FromJson<Episode>(x);
        foreach (Roleplay rp in ep.roleplays){
            actionInd=0;
            foreach(Action a in rp.actions){
                if(a.type!="0"){
                    sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].type = a.type;

                    string[] data = a.ser.Split(char.Parse("/"));
                    if(a.type=="fire"){
                        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].gunTypeObj = getGameObjByName(gunTypesObjs,data[0]);
                        if(data[0]!="Sword"){
                            GameObject targetTemp = new GameObject();
                            targetTemp.transform.position = new Vector3(
                                float.Parse(data[1]),
                                float.Parse(data[2]),
                                float.Parse(data[3]));
                            targetTemp.transform.SetParent(targetsParent.transform);
                            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;
                        }
                    }
                    else if(a.type=="move"){
                        GameObject targetTemp = new GameObject();
                        targetTemp.transform.position = new Vector3(
                            float.Parse(data[0]),
                            float.Parse(data[1]),
                            float.Parse(data[2]));
                        targetTemp.transform.SetParent(targetsParent.transform);
                        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;   
                    }                    
                }
                actionInd+=1;

            }
            playerInd+=1;
        }
    }

    // reset UI & delete targets
    public void resetEpisodeUI(){
        actionSelectingUnit.SetActive(false);
        SubmitButton.interactable = true;

        for(int i=0; i<sdata.actionsNum ; i++){
            TextMeshProUGUI xt = ActionParent.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            xt.text = "";
        }

        foreach(Transform child in targetsParent.transform)
        {
            Destroy(child.gameObject);
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
