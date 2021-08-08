﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float healthDecreaseValue;
    [SerializeField] private List<string> barriers = new List<string>();
    [SerializeField] private GameObject bloodObject;
    [SerializeField] private GameObject fireParticleSystemObject;
    [SerializeField] private float fireParticleSystemWaitTime;


    private Vector3 destination;
    private int myParent;
    private Coroutine coroutineFire;
    private bool isActive;
    private Sdata sdata;
    private bool isLaunched;
    private float SwordMaxAngle;
    private Vector3 movementDirection;

    void Start()
    {
        sdata = Sdata.sdata;
        isActive = true;
        isLaunched = false;
        destination = GetComponent<BulletData>().destination;
        myParent = GetComponent<BulletData>().myParent;
        coroutineFire = StartCoroutine(Fire());
    }


    public IEnumerator Fire(){
        RotateBullet();
        InstantiateFireParticleSystem();
        yield return new WaitForSeconds(fireParticleSystemWaitTime);

        transform.position += movementDirection * launchSpeed;
        while (isActive) {
            transform.position += Time.deltaTime * movementDirection * speed;
            yield return null;
            bool isOutsideEnv = Vector3.Distance(Vector3.zero, transform.position)>=(1.3*TargetAssignHelper.tah.radiousEnv);
            if(isOutsideEnv){
                Debug.Log("outside");
                DestroySpecial();
            }
            if(!isLaunched){
                isLaunched=true;
            }
        }
        yield return null;
    }

    bool IsSwordCompleteCircle(float angle){
        return (isActive && angle<=SwordMaxAngle); 
    }

    void InstantiateFireParticleSystem(){
        GameObject fireParticleSystem = Instantiate(fireParticleSystemObject) as GameObject;
        fireParticleSystem.transform.position = transform.position;
        fireParticleSystem.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz);
    }


    void DestroySpecial()
    {
        if(gameObject && isActive){
            isActive=false; // stop the movement of the bullet
            Debug.Log("!! Destroy :: "+gameObject.transform.name);
            StopCoroutine(coroutineFire); 
            GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1; // complete to the other action
            Destroy(gameObject);
        }
    }
    
    void RotateBullet(){
        movementDirection =  (destination-transform.position).normalized;
        float rotz = Mathf.Atan2(movementDirection.y,movementDirection.x)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz-90);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(!isLaunched){
            Debug.Log("notisLaunched_exit");
            return;
        }


        if(other.gameObject.CompareTag("Gold")){  
            DestroySpecial();
        }

        Debug.Log(transform.name+":: Exit ::"+other.transform.name);
        if(transform.name.Substring(0,5) == "Sword") return;
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isLaunched){
            Debug.Log("notisLaunched_enter");
            return;
        }
        Debug.Log(transform.name+":: Enter ::"+other.transform.name);
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }

        if(other.gameObject.CompareTag("Gold")){  
            DestroySpecial();
        }

        // if bullet bumped into Player (character)
        if(other.gameObject.CompareTag("Player")){

            Debug.Log("..P..");
            int plind= int.Parse(other.transform.name[1].ToString());
            if(plind != myParent){
                // show BLOOODD
                GameObject blood = Instantiate(bloodObject) as GameObject;
                blood.transform.position = other.transform.position;
                // blood.transform.position = other.transform.rotation;
                float rotz = sdata.playerIndex*(-90);
                blood.transform.rotation = Quaternion.Euler( 0, 0, rotz);

                Debug.Log("000DD))");
                sdata.vitalDatas[plind].health-=healthDecreaseValue;
                // Disable chrc if died
                if(sdata.vitalDatas[plind].health<=0){
                    other.gameObject.SetisActive(false);
                }
                GM.gm.updataMGH();
                DestroySpecial();
            }
            else{
                return;
            }
        }

        // if bullet bumped into barriers (that are assigned into the Inspector )
        else if(barriers.Contains(other.gameObject.tag)) {
            Debug.Log("ddddBarrires");
            DestroySpecial();
        }
    }




}
