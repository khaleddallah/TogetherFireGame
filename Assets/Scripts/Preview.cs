using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preview : MonoBehaviour
{
    public GameObject myChar;
    public ActionsData aD;
    public RectTransform targetsCanvas;

    bool submitPressed=false;
    Vector3 diff;
    Vector3 initDiff;

    public float speed;
    Camera cam;
    Vector3 tmpTarget;

    int actionNum=0;
    bool firstActionTime=true;

    public GameObject bulletPrefab;
    public GameObject tempBullet;
    public Transform bulletStarter;
    public float speedBullet;
    float rotz;
    float dist;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(submitPressed){
            diff = aD.actions[aD.actionTempNum-1].target.transform.position - myChar.transform.position;
        }
    }

    void FixedUpdate() {
        if(actionNum>=3){
            submitPressed=false;
        }

        if(submitPressed){
            MoveFire(actionNum);
        }
    }

    public void Submit(){
        submitPressed = true;
    }


    void MoveFire(int an){

        if(aD.actions[an].type=="move"){
            if(firstActionTime){
                firstActionTime=false;
                getTargetWorld(an);
                initDiff = tmpTarget - myChar.transform.position;
            }

            diff = tmpTarget - myChar.transform.position;
            if(diff.magnitude>0.1f){
                myChar.transform.position += initDiff.normalized * Time.fixedDeltaTime * speed;
            }
            else{
                initDiff=new Vector3(0f,0f,0f);
                actionNum+=1;
                firstActionTime=true;
            }
        }


        else if(aD.actions[an].type=="fire"){
            if(firstActionTime){
                firstActionTime=false;
                getTargetWorld(an);
                initDiff = tmpTarget - bulletStarter.transform.position;
                rotz = Mathf.Atan2(initDiff.y,initDiff.x)*Mathf.Rad2Deg;    
                dist = initDiff.magnitude;
                tempBullet = Instantiate(bulletPrefab) as GameObject;
                tempBullet.transform.position = bulletStarter.transform.position;
                tempBullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz);
            }
            diff = tempBullet.transform.position - bulletStarter.transform.position;
            Debug.Log("diff: "+diff.magnitude);
            Debug.Log("dist: "+dist);
            if(diff.magnitude<dist){
                Debug.Log("&&&");
                tempBullet.transform.position += initDiff.normalized * Time.fixedDeltaTime * speedBullet;
            }
            else{
                initDiff=new Vector3(0f,0f,0f);
                actionNum+=1;
                firstActionTime=true;
            }

        }
    }


    void getTargetWorld(int an){
        Vector3 t2 = aD.actions[an].target.transform.position;
        tmpTarget =  cam.ScreenToWorldPoint(t2);    
        tmpTarget.z = 0f;
    }
}
