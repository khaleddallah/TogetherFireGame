using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetAssignHelper : MonoBehaviour
{

    public static TargetAssignHelper tah;

    [SerializeField] private GameObject targetsParent;
    public GameObject actionSelectingUnit;
    public GameObject ActionParent;

    public GameObject fireTarget;
    public GameObject chrcParent;

    public GameObject lrobjcet;
    public GameObject circle0;
    public GameObject lcParent;

    public LayerMask layerMove;
    public LayerMask layerFire;

    public Vector3 currentPos;

    public Color fireLines;
    public Color fireLineFinal;
    
    public Color moveLines;
    public Color moveLineFinal;

    public bool moveError = false;

    Sdata sdata;
    GameObject myChrc;
    float vhstep;
    float slstep;
    
    Color tempColor;

    bool markerExist;


    void Awake()
    {
        if(tah != null){
            GameObject.Destroy(tah);
        }
        else{
            tah = this;
        }
        DontDestroyOnLoad(this);
    }



    void Start()
    {
        tempColor = Color.white;
        moveError = false;
        sdata = Sdata.sdata;
        myChrc = chrcParent.transform.GetChild(sdata.playerIndex).transform.GetChild(0).gameObject;
        vhstep = sdata.gridLen;
        slstep = Mathf.Sqrt(2*Mathf.Pow(vhstep,2));
    }



    public void DrawTargetLines(){
        // select the current pos 
        currentPos = myChrc.transform.position;
        // if prior action will move the chrc
        for(int i=sdata.actionIndex-1; i>=0; i--){
            Debug.Log("select the current pos:::"+i);
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].type=="move"){
                currentPos = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].target;
                Debug.Log("CurrentPos:::>>"+currentPos);
                break;
            }
        }

        // if sword
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj){
            if (sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name=="Sword"){
                // Destroy prior objects
                if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj){
                    Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
                }
                        
                // destroy lines & circles after choose the target
                foreach(Transform child in lcParent.transform)
                {
                    Destroy(child.gameObject);
                }
                
                GameObject x = Instantiate(fireTarget) as GameObject;
                x.transform.position = currentPos;
                x.transform.SetParent(targetsParent.transform);
                TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                xt.text = (sdata.actionIndex+1).ToString("0");  
                sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;

                return;
            }
        }

        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 dir = new Vector3(x,y,0f);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit;
                // int hitN;
                // bool ishit;
                float distance0=0f;
                // RaycastHit2D[] results = new RaycastHit2D[10];
                // ContactFilter2D contactFilter = new ContactFilter2D();
                hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
                    // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
                
                if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
                    hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
                    // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
                    // Debug.Log("hit:::::"+hit.distance);
                    distance0 = hit.distance;
                    tempColor = moveLines;
                }
                else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
                    hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerFire);
                    // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
                    // Debug.Log("hit:::::"+hit.distance);
                    distance0 = hit.distance;
                    tempColor = fireLines;
                }

                if(distance0>vhstep){
                // if(ishit){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
                    // draw line for available directions
                    GameObject gl = Instantiate(lrobjcet) as GameObject;
                    gl.transform.SetParent(lcParent.transform);

                    LineRenderer lRend = gl.GetComponent<LineRenderer>();
                    lRend.startColor=tempColor;
                    lRend.endColor=tempColor;                    
                    lRend.startWidth=0.05f;
                    lRend.endWidth=0.05f;
                    Vector3 last ; 
                    float lastPoint;
                    if(x==0 || y==0){
                        lastPoint = Mathf.Floor(distance0/vhstep);
                        // Debug.Log("vhstep::"+vhstep);
                        // Debug.Log("lastPoint::"+lastPoint);
                    }
                    else{
                        lastPoint = Mathf.Floor(distance0/slstep);
                        // Debug.Log("slstep::"+slstep);
                        // Debug.Log("lastPoint::"+lastPoint);
                    }
                    last = currentPos+(new Vector3(x, y, 0)*vhstep*lastPoint);
                    lRend.SetPosition(0, currentPos);
                    lRend.SetPosition(1, last); 

                    //draw points for available targets
                    for(int p=1; p<=lastPoint; p++){
                        GameObject cr = Instantiate(circle0) as GameObject;
                        cr.transform.SetParent(lcParent.transform);
                        cr.transform.position =  currentPos+new Vector3(x, y, 0)*p*vhstep;
                        cr.transform.position = new Vector3(cr.transform.position.x, cr.transform.position.y, -3f);
                    }
                }

            }
        }


    }






    public void InstTarget(Vector3 pos0){
        // reset colors of actions 
        ActionParent.transform.GetChild(0).GetComponent<uib_action>().resetActionColors();

        // disactive selecting unit 
        actionSelectingUnit.SetActive(false);

        // if changed (move to move | fire to move | move to fire) Destroy next actions
        if(moveError){
            if(pos0!=sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target){
                for(int i = sdata.actionIndex+1 ; i<sdata.actionsNum ; i++){
                    resetAction(i);
                }
            }
            moveError=false;
        }

        // Destroy prior objects
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
        }

        // destroy lines & circles after choose the target
        DestroyMarkers();

        // if Move
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
            GameObject x = Instantiate(myChrc) as GameObject;
            Destroy(x.GetComponent<PlayerReaction>());
            Destroy(x.GetComponent<BoxCollider2D>());
            Destroy(x.GetComponent<Rigidbody2D>());
            Destroy(x.transform.GetChild(1).gameObject);
            Destroy(x.transform.GetChild(0).GetChild(1).gameObject);
            x.transform.position = pos0;
            x.transform.SetParent(targetsParent.transform);
            TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            xt.text = (sdata.actionIndex+1).ToString("0");
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
            x.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);


            GameObject gl = Instantiate(lrobjcet) as GameObject;
            gl.transform.SetParent(x.transform);
            LineRenderer lRend = gl.GetComponent<LineRenderer>();
            lRend.startColor=moveLineFinal;
            lRend.endColor=moveLineFinal;                    
            lRend.startWidth=0.1f;
            lRend.endWidth=0.1f;
            lRend.SetPosition(0, currentPos);
            lRend.SetPosition(1, pos0); 


        }
        // if Fire && not "Sword"
        else if (sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name!="Sword"){
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
            GameObject x = Instantiate(fireTarget) as GameObject;
            x.transform.position = pos0;
            x.transform.SetParent(targetsParent.transform);
            TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            xt.text = (sdata.actionIndex+1).ToString("0");           
            sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
        
            GameObject gl = Instantiate(lrobjcet) as GameObject;
            gl.transform.SetParent(x.transform);
            LineRenderer lRend = gl.GetComponent<LineRenderer>();
            lRend.startColor=fireLineFinal;
            lRend.endColor=fireLineFinal;                    
            lRend.startWidth=0.1f;
            lRend.endWidth=0.1f;
            lRend.SetPosition(0, currentPos);
            lRend.SetPosition(1, pos0); 
        }

    }

    public void DestroyMarkers(){
        // destroy lines & circles after choose the target
        foreach(Transform child in lcParent.transform)
        {
            Destroy(child.gameObject);
        }
    }


    public void resetAction(int ind){
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].type="0"; // type of the action (move | fire)
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].target = new Vector3 (-999f,-999f,-999f); // target of the action either move or fire
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].gunTypeObj = null;
        ActionParent.transform.GetChild(ind).transform.GetChild(0).GetComponent<Image>().color = new Color(0f,0f,0f,0f);
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[ind].targetObj);
        }
    }


}
