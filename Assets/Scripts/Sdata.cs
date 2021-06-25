using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sdata : MonoBehaviour
{
    public static Sdata sdata;

    // main data
    public float gridLen;
    public float maxRadius;

    public string serverURL = "";
    public string myName = "";

    public int actionIndex = 0;
    public int playerIndex; // the index of the player
    public int episodeIndex = -1;

    public List<Episode> episodes = new List<Episode>(); // The episodes data
    public List<VitalData> vitalDatas = new List<VitalData>(); // the vital data of all players 

    // STATIC
    public int participantNum = 4;
    public int actionsNum = 4;


    void Awake()
    {
        if(sdata != null){
            GameObject.Destroy(sdata);
        }
        else{
            sdata = this;
        }

        // have to get this from the server
        sdata.playerIndex=0;

        // set players health & golds
        for(int h=0; h< participantNum; h++){
            VitalData x = new VitalData();
            sdata.vitalDatas.Add(x);
            sdata.vitalDatas[h].health=100.0f;
            sdata.vitalDatas[h].golds=0;
        }

        CreateNewEpisode();
        DontDestroyOnLoad(this);
    }

    public void CreateNewEpisode(){
        Episode templateEpisode = new Episode();
        for(int i=0; i<participantNum; i++){
            Roleplay x = new Roleplay();
            templateEpisode.roleplays.Add(x);

            for(int a=0; a<actionsNum; a++){
                Action y = new Action();
                templateEpisode.roleplays[i].actions.Add(y);
            }
        }
        episodeIndex+=1;
        sdata.episodes.Add(templateEpisode);
    }

}
