using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phys_action : MonoBehaviour
{

    public float actionTime = 3.0f;
    public GameObject playersParent;

    public int player = 0 ;
    public int action = 0 ; 

    public bool previewPressed = false;
    public bool working = false;

    void Start()
    {

    }

    void Update()
    {
        if(previewPressed){
            run1PlayerEpisode();
        }
    }


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




}
