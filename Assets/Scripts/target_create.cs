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
        if(!GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].target)
        {
            CreateTargetObj();
        }
        else{
            GameObject x = GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].target;
            if (GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].type!=x.GetComponent<target_drag>().targetType)
            {
            Destroy(x);
            CreateTargetObj();
            }
        }
    }

    public void CreateTargetObj(){
        GameObject x = Instantiate(targetObj) as GameObject;
        GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].target = x;
    
        x.GetComponent<target_drag>().targetType=GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].type;
        x.transform.position = targetsParent.transform.position;
        x.transform.SetParent(targetsParent.transform);

        TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = (GM.gm.actionIndex+1).ToString("0");
    }
}
