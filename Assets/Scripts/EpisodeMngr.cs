using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class EpisodeMngr : MonoBehaviour
{
    [SerializeField] private GameObject playersParent;
    [SerializeField] private List<GameObject> gunTypesObjects; // list of the gunType used in (getObjByName)
    [SerializeField] private AIPlayers ai;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Wizard wizard;
    [SerializeField] private float machineGunShiftOffset;
    [SerializeField] private GameObject middleSign;


    public int actionMove = 0;
    public int actionFire = 0;
    public Color epTimerColor;
    public float timeBeforeEpisode = 3;



    int episodeSubmitted ;
    Sdata sdata;


    void Start()
    {
        episodeSubmitted = -1 ;
        sdata = Sdata.sdata;
    }



    public void submitPress(){
        if(CheckNotSubmittedBefore()){
            episodeSubmitted = sdata.episodeIndex;

            if(sdata.gamePlayMode=="ai"){
                ai.psai();
                StartCoroutine(performAllPlayers(0, sdata.participantNum));
            }

            else{
                StartCoroutine(PostSubmit_FillData_PerformAll());
            }
        }
    }

    private bool CheckNotSubmittedBefore(){
        return episodeSubmitted < sdata.episodeIndex;
    }

    IEnumerator PostSubmit_FillData_PerformAll() {
        string msg = BuildPlayerDataMsg();
        string route = "submit";
        UnityWebRequest www = PostRequest(msg, route);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            www.Dispose();
        }
        else {
            string data = (www.downloadHandler.text);
            Debug.Log(data);
            fillEpisode(data);
            www.Dispose();
            StartCoroutine(performAllPlayers(0, sdata.participantNum));
        }
    }

    private string BuildPlayerDataMsg() {
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
        return msg;
    }

    private UnityWebRequest PostRequest(string msg, string route){
        byte[] jsonBinary = System.Text.Encoding.UTF8.GetBytes(msg);    
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        UnityWebRequest www = new UnityWebRequest(LongTermData.longTermData.serverURL+route, "POST", downloadHandlerBuffer, uploadHandlerRaw);
        return www;
    }

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
                        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[actionInd].gunTypeObj = getGameObjByName(gunTypesObjects,data[0]);
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

    private GameObject getGameObjByName(List<GameObject> list0, string x){
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
    
    private IEnumerator performAllPlayers(int Findex, int participantNum){
        Debug.Log("Pefrom Starts");
        GM.gm.stopSubmitCoroutine();
        actionMove = 0 ;
        actionFire = 0 ;

        // Extract performBasic routine
        for(int actionT = 0 ; actionT<sdata.actionsNum ; actionT++){
            Debug.Log("actionT :: "+actionT.ToString());
            actionMove = 0 ;
            actionFire = 0 ;
            for(int player=Findex; player<participantNum ; player++){
                if(sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].type == "move")
                    {
                        Debug.Log("Moveplayer:::"+player.ToString());
                        actionMove+=1;
                        if(player==sdata.playerIndex){
                            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[actionT].targetObj);
                        }
                        StartCoroutine(move(
                            playersParent.transform.GetChild(player).transform.GetChild(sdata.playerIndex).GetChild(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].characterIndex).transform.gameObject,
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
                            playersParent.transform.GetChild(player).transform.GetChild(sdata.playerIndex).GetChild(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].characterIndex).transform.gameObject,
                            sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].target,
                            sdata.episodes[sdata.episodeIndex].roleplays[player].actions[actionT].gunTypeObj
                        ));
                    } 
            }
            Debug.Log("waitin for FIRE");
            while(actionFire>0){
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
        
        // Extract LoseWinChecker routine
        GM.gm.updataMGH();
        int checklose  = GM.gm.CheckLose();
        if(checklose==1){
            yield return new WaitForSeconds(2);
            middleSignOff();
        }
        else if(checklose==2){
            yield return new WaitForSeconds(1);
            GM.gm.Winner();
            yield return new WaitForSeconds(3);
            GM.gm.loadFirstScene();
            middleSign.transform.parent.gameObject.SetActive(false);
        }


        // down timer for the new episode
        float timeTempEp = timeBeforeEpisode;
        middleSign.SetActive(true);
        middleSign.GetComponent<Animator>().SetBool("waiting", false);
        middleSign.GetComponent<Animator>().SetBool("epBefore", true);
        middleSign.GetComponent<TextMeshProUGUI>().color = epTimerColor;
        while(timeTempEp>0){
            middleSign.GetComponent<TextMeshProUGUI>().text = "Round "+(sdata.episodeIndex+1).ToString()+" starts in "+timeTempEp.ToString();
            yield return new WaitForSeconds(1);
            timeTempEp-=1;
        }
        middleSign.SetActive(false);

        // New Episode
        sdata.CreateNewEpisode();
        GM.gm.startSubmitCoroutine();
        if(GM.gm.GetWholeHealth(sdata.playerIndex)<=0){
            submitPress();
        }
        else{
            StartCoroutine(wizard.GetUserInput());
        }
    }
        

    private void middleSignOff(){
        middleSign.SetActive(false);
    }

    public IEnumerator move(GameObject playerObj, Vector3 dest) {
        Vector3 movementDir =  (dest-playerObj.transform.position).normalized;         
        float distance = Vector3.Distance(dest, playerObj.transform.position);
        float lastdistance = distance+1f;

        while (lastdistance>distance) {
            playerObj.transform.position += Time.deltaTime * movementDir * moveSpeed;
            lastdistance = distance;
            distance = Vector3.Distance(dest, playerObj.transform.position);
            yield return null;
        }
        playerObj.transform.position = dest;
        actionMove-=1;
    }

    public IEnumerator fire(int parent, GameObject playerObj, Vector3 dest, GameObject bullet) {
        if(bullet.transform.name=="MachineGun"){
            for(int b=-1; b<2 ;b++){                            
                Vector3 distortion = GetMachineGunDistoredDestination(playerObj, dest, b);
                CreateBullet(playerObj, distortion, parent, bullet);
            }
        }
        else{
            CreateBullet(playerObj, dest, parent, bullet);
        }
        yield return null;
    }

    private void CreateBullet(GameObject parent, Vector3 dest, int parentIndex, GameObject bullet){
        actionFire+=1;
        GameObject x = Instantiate(bullet) as GameObject;
        x.transform.position = parent.transform.position;
        x.GetComponent<BulletData>().destination = dest;
        x.GetComponent<BulletData>().myParent = parentIndex;
    }

    private Vector3 GetMachineGunDistoredDestination(GameObject playerObj, Vector3 dest, int b){
        float m = 0 ;
        float xdistortion = 0 ;
        float ydistortion = 0 ;

        if(dest.y==playerObj.transform.position.y){
            m=9999999f;
            xdistortion = dest.x;
            ydistortion = b*machineGunShiftOffset+dest.y;
        }
        else{
            m = -(dest.x-playerObj.transform.position.x)/(dest.y-playerObj.transform.position.y);
            xdistortion = (b*machineGunShiftOffset)/(Mathf.Sqrt(Mathf.Pow(m,2)+1)) + dest.x;
            ydistortion = m*(xdistortion-dest.x)+dest.y;
        }

        Vector3 distortion = new Vector3(xdistortion, ydistortion, 0f);
        return distortion;
    }

}
