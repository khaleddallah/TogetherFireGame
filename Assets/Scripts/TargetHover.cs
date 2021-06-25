using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetHover : MonoBehaviour
{

    public GameObject moveTarget;
    public GameObject fireTarget;

    public GameObject targetParent;

    public float bigOffset = 1.50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnMouseEnter()
    {
        transform.localScale = transform.localScale*bigOffset;
    }

    void OnMouseExit()
    {
        transform.localScale = transform.localScale/bigOffset;
    }

    void OnMouseDown()
    {
        CreateTargetObj();
    }


    public void CreateTargetObj(){
        GameObject x;
        // if(Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type=="move"){
            x = Instantiate(moveTarget) as GameObject;
        // }
        // else if(Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].type=="fire"){
            // x = Instantiate(targetObj) as GameObject;
        // }
        Sdata.sdata.episodes[Sdata.sdata.episodeIndex].roleplays[Sdata.sdata.playerIndex].actions[Sdata.sdata.actionIndex].target = x;
    
        x.transform.position = transform.position;
        targetParent = GameObject.Find("TargetsParent");
        x.transform.SetParent(targetParent.transform);


        TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = (Sdata.sdata.actionIndex+1).ToString("0");
    }

}
