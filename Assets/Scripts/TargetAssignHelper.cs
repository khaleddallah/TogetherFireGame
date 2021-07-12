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
    public GameObject circleGrenade;

    public GameObject circlePistol;


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

    public GameObject swordTarget;


    public float radiousEnv;

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



    // public void DrawTargetLines(){


    //     // Disable sword markers 
    //     swordTarget.SetActive(false);

    //     // select the current pos 
    //     currentPos = myChrc.transform.position;
    //     // if prior action will move the chrc
    //     for(int i=sdata.actionIndex-1; i>=0; i--){
    //         Debug.Log("select the current pos:::"+i);
    //         if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].type=="move"){
    //             currentPos = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].target;
    //             Debug.Log("CurrentPos:::>>"+currentPos);
    //             break;
    //         }
    //     }

    //     // // if sword
    //     // if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj){
    //     //     if (sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name=="Sword"){
    //     //         // Destroy prior objects
    //     //         if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj){
    //     //             Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
    //     //         }
                        
    //     //         // destroy lines & circles after choose the target
    //     //         foreach(Transform child in lcParent.transform)
    //     //         {
    //     //             Destroy(child.gameObject);
    //     //         }
                
    //     //         GameObject x = Instantiate(fireTarget) as GameObject;
    //     //         x.transform.position = currentPos;
    //     //         x.transform.SetParent(targetsParent.transform);
    //     //         TextMeshProUGUI xt = x.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    //     //         xt.text = (sdata.actionIndex+1).ToString("0");  
    //     //         sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;

    //     //         return;
    //     //     }
    //     // }

    //     // Draw lines & points
    //     for(int x=-1 ; x<=1 ; x++){
    //         for(int y=-1 ; y<=1 ; y++){
    //             Vector3 dir = new Vector3(x,y,0f);
    //             Debug.Log("dir::"+dir);
    //             RaycastHit2D hit;
    //             // int hitN;
    //             // bool ishit;
    //             float distance0=0f;
    //             // RaycastHit2D[] results = new RaycastHit2D[10];
    //             // ContactFilter2D contactFilter = new ContactFilter2D();
    //             hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
    //                 // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
                
    //             if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
    //                 hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
    //                 // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
    //                 // Debug.Log("hit:::::"+hit.distance);
    //                 distance0 = hit.distance;
    //                 tempColor = moveLines;
    //             }
    //             else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire"){
    //                 hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerFire);
    //                 // hitN=Physics2D.Raycast(currentPos, dir, contactFilter, results);
    //                 // Debug.Log("hit:::::"+hit.distance);
    //                 distance0 = hit.distance;
    //                 tempColor = fireLines;
    //             }

    //             if(distance0>vhstep){
    //             // if(ishit){
    //                 Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
    //                 // draw line for available directions
    //                 GameObject gl = Instantiate(lrobjcet) as GameObject;
    //                 gl.transform.SetParent(lcParent.transform);

    //                 LineRenderer lRend = gl.GetComponent<LineRenderer>();
    //                 lRend.startColor=tempColor;
    //                 lRend.endColor=tempColor;                    
    //                 lRend.startWidth=0.05f;
    //                 lRend.endWidth=0.05f;
    //                 Vector3 last ; 
    //                 float lastPoint;
    //                 if(x==0 || y==0){
    //                     lastPoint = Mathf.Floor(distance0/vhstep);
                        
    //                     // Debug.Log("vhstep::"+vhstep);
    //                     // Debug.Log("lastPoint::"+lastPoint);
    //                 }
    //                 else{
    //                     lastPoint = Mathf.Floor(distance0/slstep);
    //                     // Debug.Log("slstep::"+slstep);
    //                     // Debug.Log("lastPoint::"+lastPoint);
    //                 }
    //                 last = currentPos+(new Vector3(x, y, 0)*vhstep*lastPoint);
    //                 lRend.SetPosition(0, currentPos);

    //                 lRend.SetPosition(1, last); 

    //                 //draw points for available targets
    //                 for(int p=1; p<=lastPoint; p++){
    //                     Vector3 posTemp = currentPos+new Vector3(x, y, 0)*p*vhstep;
    //                     bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
    //                     if(isOutsideEnv){
    //                         continue;
    //                     }
    //                     GameObject cr = Instantiate(circle0) as GameObject;
    //                     cr.transform.SetParent(lcParent.transform);
    //                     cr.transform.position =  currentPos+new Vector3(x, y, 0)*p*vhstep;
    //                     cr.transform.position = new Vector3(cr.transform.position.x, cr.transform.position.y, -3f);
    //                 }
    //             }

    //         }
    //     }

    //     // if move put a point in same chrc position
    //     if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
    //         GameObject cr = Instantiate(circle0) as GameObject;
    //         cr.transform.SetParent(lcParent.transform);
    //         cr.transform.position =  currentPos;
    //         cr.transform.position = new Vector3(cr.transform.position.x, cr.transform.position.y, -3f);
    //     }

    // }


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
            
            float rotz = sdata.playerIndex*(-90);
            x.transform.rotation = Quaternion.Euler( 0, 0, rotz);

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



    // ============================== sword ==================================

    public void MarkerSword(){
        DestroyMarkers();

        // select the current pos 
        currentPos = myChrc.transform.position;
        for(int i=sdata.actionIndex-1; i>=0; i--){
            Debug.Log("select the current pos:::"+i);
            if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].type=="move"){
                currentPos = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[i].target;
                Debug.Log("CurrentPos:::>>"+currentPos);
                break;
            }
        }

        swordTarget.SetActive(true);
        swordTarget.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        swordTarget.transform.position = currentPos;
        float rotz = sdata.playerIndex*(-90);
        swordTarget.transform.rotation = Quaternion.Euler( 0, 0, rotz);
        // x.transform.SetParent(targetsParent.transform);
        // sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj = x;
    }


    public void MarkerSwordApply(){
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = currentPos;
        // GameObject x = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj;
        TextMeshProUGUI xt = swordTarget.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        xt.text = (sdata.actionIndex+1).ToString("0");  
    }




    // ============================== Grende ==================================

    public void DrawGrendeMarkers(){
        // Disable sword markers 
        swordTarget.SetActive(false);

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

        // Draw points  
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridLen);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            for (int j=-(int)NumLines ; j<NumLines; j++ ){
                Vector3 posTemp = new Vector3(i*sdata.gridLen, j*sdata.gridLen, 0.0f);
                bool isCurrentPos = i*sdata.gridLen==currentPos.x && j*sdata.gridLen==currentPos.y ; 
                bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
                if(isCurrentPos || isOutsideEnv){
                    continue;
                }
                else{
                    GameObject cr = Instantiate(circleGrenade) as GameObject;
                    cr.transform.SetParent(lcParent.transform);
                    cr.transform.position = new Vector3(i*sdata.gridLen, j*sdata.gridLen, -0.03f);
                }
            }
        }
    }

    // ============================== Move ==================================

    public void MoveMarkers(){
        // Disable sword markers 
        swordTarget.SetActive(false);
        
        // select the current pos 
        currentPos = myChrc.transform.position;


        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 dir = new Vector3(x,y,0f);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit;
                float distance0=0f;

                
                hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
                distance0 = hit.distance;
                tempColor = moveLines;

                if(hit.distance > 0){
                    Debug.Log(hit.transform.gameObject.tag);
                    if(hit.collider.gameObject.CompareTag("Gold")){
                        distance0+=vhstep;
                    }
                }

                if(distance0>vhstep){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
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
                    }
                    else{
                        lastPoint = Mathf.Floor(distance0/slstep);
                    }
                    last = currentPos+(new Vector3(x, y, 0)*vhstep*lastPoint);
                    lRend.SetPosition(0, currentPos);
                    lRend.SetPosition(1, last); 

                    //draw points for available targets
                    for(int p=1; p<=lastPoint; p++){
                        Vector3 posTemp = currentPos+new Vector3(x, y, 0)*p*vhstep;
                        bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
                        if(isOutsideEnv){
                            posTemp = currentPos+new Vector3(x, y, 0)*(p-1)*vhstep;
                            lRend.SetPosition(1, posTemp); 
                            continue;
                        }
                        GameObject cr0 = Instantiate(circle0) as GameObject;
                        cr0.transform.SetParent(lcParent.transform);
                        cr0.transform.position =  currentPos+new Vector3(x, y, 0)*p*vhstep;
                        cr0.transform.position = new Vector3(cr0.transform.position.x, cr0.transform.position.y, -0.03f);
                    }
                }
            }
        }

        GameObject cr = Instantiate(circle0) as GameObject;
        cr.transform.SetParent(lcParent.transform);
        cr.transform.position =  currentPos;
        cr.transform.position = new Vector3(cr.transform.position.x, cr.transform.position.y, -0.03f);

    }



    // ============================== Piston&MachineGun ==================================


    public void PistolMarkers(){
        // Disable sword markers 
        swordTarget.SetActive(false);

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


        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 dir = new Vector3(x,y,0f);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit;
                float distance0=0f;

                hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerFire);
                distance0 = hit.distance;
                tempColor = fireLines;
            

                if(distance0>vhstep){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
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


                    last = currentPos+(new Vector3(x, y, 0)*vhstep);
                    lRend.SetPosition(0, currentPos);
                    lRend.SetPosition(1, last); 


                    GameObject cr0 = Instantiate(circlePistol) as GameObject;
                    cr0.transform.SetParent(lcParent.transform);
                    
                    Vector3 movementDir =  (last-currentPos).normalized;    
                    float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
                    cr0.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz);

                    cr0.transform.position =  last;
                    cr0.transform.position = new Vector3(cr0.transform.position.x, cr0.transform.position.y, -0.03f);

                }

            }
        }
    }
}
