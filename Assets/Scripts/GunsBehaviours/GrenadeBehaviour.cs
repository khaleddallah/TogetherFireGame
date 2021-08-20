using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private float healthDecreaseValue;
    [SerializeField] private List<string> barriers;
    [SerializeField] private GameObject bloodObject;
    [SerializeField] private float radiousExplosion;
    [SerializeField] private GameObject GrenadeEffect;
    [SerializeField] private LayerMask playersLayer;


    Vector3 destination;
    Coroutine coroutineFire;
    bool active;
    Sdata sdata;
    int myParent;




    void Start()
    {
        sdata = Sdata.sdata;
        active = true;
        destination = GetComponent<BulletData>().destination;
        myParent = GetComponent<BulletData>().myParent;
        barriers = new List<string>();
        coroutineFire = StartCoroutine(fire());
    }


    public IEnumerator fire(){
        Vector3 movementDir =  (destination-transform.position).normalized;    
        float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz-90);

        float distance = Vector3.Distance(destination, transform.position);
        float lastdistance = distance+1f;

        while (lastdistance>distance) {
            transform.position += Time.deltaTime * movementDir * grenadeSpeed;
            lastdistance = distance;
            distance = Vector3.Distance(destination, transform.position);
            yield return null;
        }
        transform.position = destination;
        CreateGrendeEffect();
        CircleRaycastEffectPlayers();

        GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1;
        DestroySpecial();
        yield return null;

    }

    private void CreateGrendeEffect(){
        GameObject firePS = Instantiate(GrenadeEffect) as GameObject;
        firePS.transform.position = transform.position;
    }

    private void CircleRaycastEffectPlayers(){
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radiousExplosion, playersLayer);
        foreach(Collider2D hit in hits){
            if(hit.gameObject.CompareTag("Player")){

                Debug.Log("hittt:"+hit.name);
                int plind= int.Parse(hit.transform.name[1].ToString());
                int chind= int.Parse(hit.transform.name[2].ToString()); 
                float dis0 = Vector3.Distance(hit.transform.position,transform.position);
                
                // show BLOOODD
                GameObject blood = Instantiate(bloodObject) as GameObject;
                blood.transform.position = hit.transform.position;

                if (dis0<0.8f*radiousExplosion){                
                    sdata.vitalDatas[plind].health[chind]-= healthDecreaseValue;
                }
                else{
                    sdata.vitalDatas[plind].health[chind]-= healthDecreaseValue*0.5f;
                }

                if(sdata.vitalDatas[plind].health[chind]<=0){
                    hit.gameObject.SetActive(false);
                }
                GM.gm.updataMGH();
            }
        }
    }

    void DestroySpecial()
    {
        if(gameObject && active){
            active=false; // stop the movement of the bullet
            Debug.Log("!! destinationroy :: "+gameObject.transform.name);
            StopCoroutine(coroutineFire); 
            GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1; // complete to the other action
            Destroy(gameObject);
        }
    }

}
