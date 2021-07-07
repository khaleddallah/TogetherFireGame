using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{

    public float grenadeSpeed;
    public float healthDec;
    public List<string> barriers = new List<string>();

    public Vector3 dest;

    public GameObject bloodObj;

    public GameObject firePSObj;
    Coroutine coroutineFire;
    bool active;
    Sdata sdata;
    public int myparent;

    public float radiousExplosion;
    public LayerMask chrcLayer;

    public GameObject GrenadeEffect;


    void Start()
    {
        sdata = Sdata.sdata;
        active = true;

        dest = GetComponent<BulletData>().dest;
        myparent = GetComponent<BulletData>().myparent;

        coroutineFire = StartCoroutine(fire());
    }


  public IEnumerator fire(){


        Vector3 movementDir =  (dest-transform.position).normalized;    

        float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz-90);

        float distance = Vector3.Distance(dest, transform.position);
        float lastdistance = distance+1f;

        while (lastdistance>distance) {
            transform.position += Time.deltaTime * movementDir * grenadeSpeed;
            lastdistance = distance;
            distance = Vector3.Distance(dest, transform.position);
            yield return null;
        }
        Debug.Log("stop after distance get biggger Grende");
        transform.position = dest;

        // effect eplosion
        GameObject firePS = Instantiate(GrenadeEffect) as GameObject;
        firePS.transform.position = transform.position;
        // yield return new WaitForSeconds(0.2f);


        // circle raycast        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radiousExplosion, chrcLayer);
        foreach(Collider2D hit in hits){
            if(hit.gameObject.CompareTag("Player")){

                Debug.Log("hittt:"+hit.name);
                int plind= int.Parse(hit.transform.name[1].ToString());
                float dis0 = Vector3.Distance(hit.transform.position,transform.position);
                
                // show BLOOODD
                GameObject blood = Instantiate(bloodObj) as GameObject;
                blood.transform.position = hit.transform.position;

                if (dis0<0.8f*radiousExplosion){                
                    sdata.vitalDatas[plind].health-= healthDec;
                }
                else{
                    sdata.vitalDatas[plind].health-= healthDec*0.5f;
                }
                    // Disable chrc if died
                if(sdata.vitalDatas[plind].health<=0){
                    hit.gameObject.SetActive(false);
                }
                GM.gm.updataMGH();
            }
        }


        // tell episodeMngr Im done
        GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1;
        DestroySpecial();
        yield return null;

    }





    void DestroySpecial()
    {
        if(gameObject && active){
            active=false; // stop the movement of the bullet
            Debug.Log("!! Destroy :: "+gameObject.transform.name);
            StopCoroutine(coroutineFire); 
            GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1; // complete to the other action
            Destroy(gameObject);
        }
    }

}
