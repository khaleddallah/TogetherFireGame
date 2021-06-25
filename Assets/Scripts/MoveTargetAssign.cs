using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetAssign : MonoBehaviour
{
    public GameObject myChrc;
    public GameObject chrcParent;

    public GameObject lrobjcet;
    public GameObject circle0;
    public LayerMask layer0;

    Sdata sdata;

    float vhstep;
    float slstep;
    // Start is called before the first frame update
    void Start()
    {
        
        sdata = Sdata.sdata;
        myChrc = chrcParent.transform.GetChild(sdata.playerIndex).transform.GetChild(0).gameObject;
        vhstep = sdata.gridLen;
        slstep = Mathf.Sqrt(2*Mathf.Pow(vhstep,2));
        Debug.Log("slstep::::"+slstep);
        Debug.Log("vhstep::::"+vhstep);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void moveTarget(){
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector2 dir = new Vector2(x,y);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit = Physics2D.Raycast(myChrc.transform.position, dir, Mathf.Infinity, ~layer0);
                if(hit.distance>0){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
                    
                    // draw line for available directions
                    GameObject gl = Instantiate(lrobjcet) as GameObject;
                    LineRenderer lRend = gl.GetComponent<LineRenderer>();
                    lRend.startColor=Color.white;
                    lRend.endColor=Color.white;                    
                    lRend.startWidth=0.05f;
                    lRend.endWidth=0.05f;
                    Vector3 last ; 
                    float lastPoint;
                    if(x==0 || y==0){
                        lastPoint = Mathf.Floor(hit.distance/vhstep);
                        Debug.Log("vhstep::"+vhstep);
                        Debug.Log("lastPoint::"+lastPoint);
                    }
                    else{
                        lastPoint = Mathf.Floor(hit.distance/slstep);
                        Debug.Log("slstep::"+slstep);
                        Debug.Log("lastPoint::"+lastPoint);
                    }
                    last = myChrc.transform.position+(new Vector3(x, y, 0)*vhstep*lastPoint);
                    lRend.SetPosition(0, myChrc.transform.position);
                    lRend.SetPosition(1, last); 

                    //draw points for available targets
                    for(int p=1; p<=lastPoint; p++){
                        GameObject cr = Instantiate(circle0) as GameObject;
                        cr.transform.position =  myChrc.transform.position+new Vector3(x, y, 0)*p*vhstep;
                    }
                }
            }
        }


    }

}
