using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// stored data
public class Sdata : MonoBehaviour
{
    public static Sdata sdata;

    // main data
    public float gridCellSize;
    public float maxRadius;
    public int actionIndex = 0;
    public int playerIndex;
    public int episodeIndex = -1;
    public List<Episode> episodes; 
    public List<VitalData> vitalDatas; 
    public int participantNum = 4;
    public int actionsNum = 3;
    public int howMuchPlayersStarted = 0;
    public string gamePlayMode;

    void Awake()
    {
        if(sdata != null){
            GameObject.Destroy(sdata);
        }
        else{
            sdata = this;
        }

        DontDestroyOnLoad(this);
    }

    void Start(){
        SetSomeInitialValues();
    }

    private void SetSomeInitialValues(){
        episodes = new List<Episode>(); 
        vitalDatas = new List<VitalData>();
        sdata.playerIndex=0;

        // set players health & golds
        for(int h=0; h< participantNum; h++){
            VitalData x = new VitalData();
            sdata.vitalDatas.Add(x);
            sdata.vitalDatas[h].health=100.0f;
            sdata.vitalDatas[h].golds=0;
        }

        CreateNewEpisode();
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
