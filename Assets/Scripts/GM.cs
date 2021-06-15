using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public static GM gm;
    public int actionIndex = 0;
    public int episodeIndex = -1;


    public GameData gd ;
    public int participantNum = 4;
    public int actionsNum = 4;
    public Episode templateEpisode;


    void Awake()
    {
        if(gm != null){
            GameObject.Destroy(gm);
        }
        else{
            gm = this;
        }

        // have to get this from the server
        gm.gd.playerIndex=0;

        // set players health & Golds
        for(int h=0; h< participantNum; h++){
            VitalData x = new VitalData();
            gm.gd.VitalDatas.Add(x);
            gm.gd.VitalDatas[h].health=100.0f;
            gm.gd.VitalDatas[h].Golds=0;
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
        gm.gd.episodes.Add(templateEpisode);

        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            reload();
        }
    }

    // reload the scene
    public void reload(){
        Destroy(GM.gm.gameObject);
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
 

}
