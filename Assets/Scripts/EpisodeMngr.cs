using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class EpisodeMngr : MonoBehaviour
{

    public float actionTime = 3.0f;
    public GameObject playersParent;

    public int player = 0 ;
    public int action = 0 ; 

    public bool previewPressed = false;
    public bool working = false;

    public List<GameObject> gunTypes = new List<GameObject>();
    public List<GameObject> TargetTypes = new List<GameObject>();


    void Start()
    {

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



    public void run1PlayerEpisode(){
        if(action<GM.gm.actionsNum){
            if (working){
                return;
            }

            else if(GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[player].actions[action].type == "move")
            {
                working=true;
                StartCoroutine(move(
                    playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                    GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[player].actions[action].target
                ));
            }   
            else if(GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[action].type == "fire")
            {
                working=true;
                StartCoroutine(fire(
                    playersParent.transform.GetChild(player).transform.GetChild(0).transform.gameObject,
                    GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[player].actions[action].target,
                    GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[action].gunTypeObj
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
    


    public void previewPress(){
        previewPressed=true;
    }

    public void submitPress(){
        StartCoroutine(GetText());

    }


    IEnumerator GetText() {
        UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:5000");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            fillEpisode(www.downloadHandler.text);
            // GM.gm.gd.episodes[0] = JsonUtility.FromJson<Episode>(www.downloadHandler.text);
        }
    }


    void fillEpisode(string x){
        Episode ep = JsonUtility.FromJson<Episode>(x);
        foreach (Roleplay rp in ep.roleplays){
            foreach(Action a in rp.actions){
                string[] data = a.ser.Split(char.Parse("/"));
                foreach(string s in data){
                    Debug.Log(s);
                }
            }
        }
    }




}
