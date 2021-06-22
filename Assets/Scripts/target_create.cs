using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class target_create : MonoBehaviour
{

    [SerializeField] private GameObject targetObj;
    [SerializeField] private GameObject targetsParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstTarget(){
        // NOT exist before 
        if(!Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].target)
        {
            // if Move
            if(Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type=="move"){
                CreateTargetObj();
            }
            // if Fire && not "Sword"
            else{
                if (Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].gunTypeObj.transform.name!="Sword"){
                    CreateTargetObj();
                }
            }
        }
        // exist before
        else{
            GameObject x = Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].target;
            
            // if Gun "sword" pressed destroy Target
            if (Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].gunTypeObj.transform.name=="Sword"){
                Destroy(x);
            }
            
            // Move before & Fire Now || or conversely 
            else if (Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type!=x.GetComponent<target_drag>().targetType)
            {
                Destroy(x);
                CreateTargetObj();
            }

        }
    }

    public void CreateTargetObj(){
        GameObject x = Instantiate(targetObj) as GameObject;
        Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].target = x;
    
        x.GetComponent<target_drag>().targetType=Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type;
        x.transform.position = targetsParent.transform.position;
        x.transform.SetParent(targetsParent.transform);


        TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = (Sdata.sdata.actionIndex+1).ToString("0");
    }
}
