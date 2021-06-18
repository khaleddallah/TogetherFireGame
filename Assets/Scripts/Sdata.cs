using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sdata : MonoBehaviour
{
    public static Sdata sdata;
    public int actionIndex = 0;
    public int episodeIndex = -1;


    public GameData gd ;
    public int participantNum = 4;
    public int actionsNum = 4;
    public Episode templateEpisode;


    void Awake()
    {
        if(sdata != null){
            GameObject.Destroy(sdata);
        }
        else{
            sdata = this;
        }

        // have to get this from the server
        sdata.gd.playerIndex=0;

        // set players health & Golds
        for(int h=0; h< participantNum; h++){
            VitalData x = new VitalData();
            sdata.gd.VitalDatas.Add(x);
            sdata.gd.VitalDatas[h].health=100.0f;
            sdata.gd.VitalDatas[h].Golds=0;
        }


        templateEpisode = new Episode();

        for(int i=0; i<participantNum; i++){
            Roleplay x = new Roleplay();
            templateEpisode.roleplays.Add(x);

            for(int a=0; a<actionsNum; a++){
                Action y = new Action();
                templateEpisode.roleplays[i].actions.Add(y);
            }
        }

        episodeIndex+=1;
        sdata.gd.episodes.Add(templateEpisode);

        DontDestroyOnLoad(this);
    }

}
