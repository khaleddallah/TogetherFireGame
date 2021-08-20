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
    public int charactersNum;

    void Awake()
    {
        if(sdata != null){
            GameObject.Destroy(sdata);
        }
        else{
            sdata = this;
        }
        SetSomeInitialValues();
        DontDestroyOnLoad(this);

    }

    void Start(){
    }

    private void SetSomeInitialValues(){
        sdata.participantNum = LongTermData.longTermData.participantNum;

        episodes = new List<Episode>(); 
        vitalDatas = new List<VitalData>();

        // set players health & golds
        for(int h=0; h< participantNum; h++){
            VitalData x = new VitalData();
            sdata.vitalDatas.Add(x);
            sdata.vitalDatas[h].golds=0;
            for(int c=0; c<sdata.charactersNum; c++){
                float f = 100f;
                sdata.vitalDatas[h].health.Add(f);
            }
        }

        CreateNewEpisode();

        Sdata.sdata.playerIndex = LongTermData.longTermData.playerIndex;
        sdata.vitalDatas[sdata.playerIndex].name = LongTermData.longTermData.myName;
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
