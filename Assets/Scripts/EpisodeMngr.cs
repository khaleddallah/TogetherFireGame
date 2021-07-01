using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class EpisodeMngr : MonoBehaviour
{

    [SerializeField] private GameObject actionSelectingUnit;
    [SerializeField] private GameObject targetsParent; //used to create other players targets & then Destroy them

    public float actionTime = 3.0f; // How much action will take
    public GameObject playersParent; // The parents of players // used to perform (move & fire)

    public List<GameObject> gunTypesObjs = new List<GameObject>(); // list of the gunType used in (getObjByName)
    public GameObject ActionParent; // used to reset actions text after perform ends
    public Button SubmitButton;
    public TextMeshProUGUI ActionNumUI;

    int episodeSubmitted = -1;
    Sdata sdata;

    // used in Perform routine
    public int actionMove = 0;
    public int actionFire = 0;


    void Start()
    {
        episodeSubmitted = -1 ;
        sdata = Sdata.sdata;
    }



    // when Submit pressed
    public void submitPress(){
        // assure that the episode is not performed before
        if(episodeSubmitted < sdata.episodeIndex){
            SubmitButton.interactable = false;
            episodeSubmitted = sdata.episodeIndex;
            submitCurrentPlayerRoleplay();
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
            if(!a.target.Equals(new Vector3(-999,-999,-999))){
                msg+= ",";
                msg+= "\"target\":\"";
                msg+= a.target.x.ToString()+"/";
                msg+= a.target.y.ToString()+"/";
                msg+= a.target.z.ToString();
                msg+= "\"";
            }
            msg+= "},";
        }
        msg = msg.Substring(0,msg.Length-1);
        msg+="]}";
        Debug.Log(msg);
        StartCoroutine(submitpostToServer(msg,"submit"));
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
            StartCoroutine(performAllPlayers(0, sdata.participantNum));
        }
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
                            Vector3 targetTemp = new Vector3(
                                float.Parse(data[1]),
                                float.Parse(data[2]),
                                float.Parse(data[3]));
                            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;
                        }
                    }
                    else if(a.type=="move"){
                        Vector3 targetTemp = new Vector3(
                            float.Parse(data[0]),
                            float.Parse(data[1]),
                            float.Parse(data[2]));
                        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].target = targetTemp;   
                    }                    
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
    

    // perform phase
    public IEnumerator performAllPlayers(int Findex, int participantNum){
        Debug.Log("Pefrom Starts");
        actionSelectingUnit.SetActive(false);
        GM.gm.stopSubmitCoroutine();
        GM.gm.SubmitTimeText.text = "EP "+ sdata.episodeIndex.ToString() +" : Working"; 
        // Time.timeScale = 0.3f;
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
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[actionT].targetObj);
            yield return new WaitForSeconds(1);
        }
        ActionNumUI.text = "";
        
        int checklose  = GM.gm.checkLose();
        if(checklose==1){
            yield return new WaitForSeconds(2);
            GM.gm.winLoseSignOff();
        }
        else if(checklose==2){
            yield return new WaitForSeconds(1);
            // GM.gm.winLoseSignOff();
            GM.gm.Winner();
            yield return new WaitForSeconds(3);
            GM.gm.loadFirstScene();
        }

        sdata.CreateNewEpisode();
        resetEpisodeUI();
        GM.gm.startSubmitCoroutine();
    }
        


    // move object to distination
    public IEnumerator move(GameObject playerObj, Vector3 dest) {
        Vector3 movementDir =  (dest-playerObj.transform.position).normalized;         
        float distance = Vector3.Distance(dest, playerObj.transform.position);
        float speed0 = distance/actionTime;
        // float t0 = 0f;
        while (distance>0.2f) {
            distance = Vector3.Distance(dest, playerObj.transform.position);
            playerObj.transform.position += Time.deltaTime * movementDir * speed0;
            // t0+=Time.deltaTime;
            yield return null;
        }
        playerObj.transform.position = dest;
        actionMove-=1;
    }


    // fire bullet from playerObj to dest
    public IEnumerator fire(int parent, GameObject playerObj, Vector3 dest, GameObject bullet) {
        GameObject x = Instantiate(bullet) as GameObject;
        x.transform.position = playerObj.transform.position;
        x.GetComponent<GunBullet>().dest = dest;
        x.GetComponent<GunBullet>().launch(parent);
        yield return null;
    }



    // reset UI
    public void resetEpisodeUI(){
        TargetAssignHelper.tah.moveError = false;
        actionSelectingUnit.SetActive(false);
        SubmitButton.interactable = true;
        // reset ui Actions to ""
        for(int i=0; i<sdata.actionsNum ; i++){
            TextMeshProUGUI xt = ActionParent.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            xt.text = "";
        }
    }


}
