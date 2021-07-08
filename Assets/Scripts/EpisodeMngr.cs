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
    public AIPlayers ai;

    // used in Perform routine
    public int actionMove = 0;
    public int actionFire = 0;

    public float timeBeforeEp = 3;

    public GameObject winLoseSign;
    public Color epTimerColor;

    public GameObject swordTarget;

    public Wizard wizard;

    public float moveSpeed;
    public float machineGunOffset;
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
            checkActions();
            // submitCurrentPlayerRoleplay();
            if(sdata.gamePlayMode=="ai"){
                ai.psai();
                StartCoroutine(performAllPlayers(0, sdata.participantNum));
            }
            else{
                submitCurrentPlayerRoleplay();
            }
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
        UnityWebRequest www = new UnityWebRequest(LTD.ltd.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
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
        ActionParent.transform.GetChild(0).GetComponent<uib_action>().resetActionColors();

        actionSelectingUnit.SetActive(false);
        GM.gm.stopSubmitCoroutine();
        // GM.gm.SubmitTimeText.text = "EP "+ sdata.episodeIndex.ToString() +" : Working"; 
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
                        if(player==sdata.playerIndex){
                            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[actionT].targetObj);
                        }
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
                        Debug.Log("Fireplayer:::"+player.ToString());
                        if(player==sdata.playerIndex){
                            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[actionT].targetObj);
                        }
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

            winLoseSign.transform.parent.gameObject.SetActive(false);
        }


        // down timer for the new episode
        float timeTempEp = timeBeforeEp;
        winLoseSign.SetActive(true);
        winLoseSign.GetComponent<Animator>().SetBool("waiting", false);
        winLoseSign.GetComponent<Animator>().SetBool("epBefore", true);

        winLoseSign.GetComponent<TextMeshProUGUI>().color = epTimerColor;
        while(timeTempEp>0){
            winLoseSign.GetComponent<TextMeshProUGUI>().text = "Round "+(sdata.episodeIndex+1).ToString()+" starts in "+timeTempEp.ToString();
            yield return new WaitForSeconds(1);
            timeTempEp-=1;
        }
        winLoseSign.SetActive(false);

        sdata.CreateNewEpisode();
        resetEpisodeUI();
        GM.gm.startSubmitCoroutine();

        if(sdata.vitalDatas[sdata.playerIndex].health<=0){
            submitPress();
        }
        else{
            StartCoroutine(wizard.GetUserInput());
        }
    }
        


    // move object to distination
    public IEnumerator move(GameObject playerObj, Vector3 dest) {
        Vector3 movementDir =  (dest-playerObj.transform.position).normalized;         
        float distance = Vector3.Distance(dest, playerObj.transform.position);
        // float speed0 = distance/actionTime;
        float lastdistance = distance+1f;
        // float t0 = 0f;
        // float lastdistance = 9999999.0f;
        while (lastdistance>distance) {
            playerObj.transform.position += Time.deltaTime * movementDir * moveSpeed;

            // Debug.Log("l:d");

            lastdistance = distance;

            distance = Vector3.Distance(dest, playerObj.transform.position);
            // t0+=Time.deltaTime;
            yield return null;
        }
        Debug.Log("stop after distance get biggger");
        playerObj.transform.position = dest;
        actionMove-=1;
    }


    // fire bullet from playerObj to dest
    public IEnumerator fire(int parent, GameObject playerObj, Vector3 dest, GameObject bullet) {

        if(bullet.transform.name=="MachineGun"){
            actionFire+=3;
            for(int b=-1; b<2 ;b++){                            
                GameObject x0 = Instantiate(bullet) as GameObject;
                x0.transform.position = playerObj.transform.position;
                // Debug.Log(dest);
                Debug.Log("dest.normalized:"+dest.normalized);
                Vector3 distortion = new Vector3(dest.normalized.x*machineGunOffset*b, dest.normalized.y*machineGunOffset*b, 0f);
                Debug.Log("dest:"+dest);
                x0.GetComponent<BulletData>().dest = dest + distortion;
                x0.GetComponent<BulletData>().myparent = parent;
            }
        }
        else{
            actionFire+=1;
            GameObject x = Instantiate(bullet) as GameObject;
            x.transform.position = playerObj.transform.position;
            x.GetComponent<BulletData>().dest = dest;
            x.GetComponent<BulletData>().myparent = parent;
        }
        yield return null;

    }


    // public IEnumerator swordLaunch(int parent, GameObject playerObj, Vector3 dest, GameObject bullet) {
    //     Debug.Log("parent:sword:"+parent);
    //     GameObject x = Instantiate(bullet) as GameObject;
    //     x.transform.position = playerObj.transform.position;
    //     x.GetComponent<SwordBehaviour>().launch(parent);
    //     yield return null;
    // }

    // public IEnumerator grenadeLaunch(int parent, GameObject playerObj, Vector3 dest, GameObject bullet) {
    //     Debug.Log("parent:grenade:"+parent);
    //     GameObject x = Instantiate(bullet) as GameObject;
    //     x.transform.position = playerObj.transform.position;
    //     x.GetComponent<GrenadeBehaviour>().launch(parent);
    //     yield return null;
    // }


    // reset UI
    public void resetEpisodeUI(){
        TargetAssignHelper.tah.moveError = false;
        actionSelectingUnit.SetActive(false);
        SubmitButton.interactable = true;
        // reset ui Actions to ""
        for(int i=0; i<sdata.actionsNum ; i++){
            ActionParent.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(0f,0f,0f,0f);
        }
    }


    public void checkActions(){
        for(int i=0; i<sdata.actionsNum; i++){
            string type0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].type;
            Vector3 target0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].target;
            GameObject gunTypeObj0 = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].gunTypeObj;
            if(type0=="move"){
                if(target0==new Vector3(-999f,-999f,-999f)){
                    Debug.Log("moveERRR:"+i);
                    resetAction(i);
                }
            }
            else if(type0=="fire"){
                if(!gunTypeObj0){
                    Debug.Log("FireERRR1:"+i);
                    resetAction(i);
                }
                else if(gunTypeObj0.transform.name!="Sword"){
                    if(target0==new Vector3(-999f,-999f,-999f)){
                        Debug.Log("FireERRR2:"+i);
                        resetAction(i);
                    }
                }
            }
        }
    }

    public void resetAction(int ind){
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].type="0"; // type of the action (move | fire)
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].target = new Vector3 (-999f,-999f,-999f); // target of the action either move or fire
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].gunTypeObj = null;
        // ActionParent.transform.GetChild(ind).transform.GetChild(0).GetComponent<Image>().color = new Color(0f,0f,0f,0f);
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj);
        }
    }


}
