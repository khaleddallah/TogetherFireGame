using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : MonoBehaviour
{

    public float speed;
    public float offsetSpeeding;
    public float healthDec;
    public List<string> barriers = new List<string>();

    public Vector3 dest;

    public GameObject bloodObj;

    public GameObject firePSObj;
    Coroutine coroutineFire;
    bool active;
    Sdata sdata;
    public int myparent;
    bool launched = false;

    void Start()
    {
        sdata = Sdata.sdata;
        active = true;
        dest = GetComponent<BulletData>().dest;
        myparent = GetComponent<BulletData>().myparent;

        coroutineFire = StartCoroutine(fire());
    }




    public IEnumerator fire(){
        Debug.Log("####"+transform.name.Substring(0,5));
        if(transform.name.Substring(0,5) == "Sword"){
            float z = 0 ; 
            Debug.Log("Sword");
            while(z<=359 && active)
            {
                z+=1*speed*Time.deltaTime;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
                yield return null;
            }
            DestroySpecial();
            yield return null;
        }
        else{
            Vector3 movementDir =  (dest-transform.position).normalized; 
            float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz-90);
            
            GameObject firePS = Instantiate(firePSObj) as GameObject;
            firePS.transform.position = transform.position;
            firePS.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz);
            yield return new WaitForSeconds(0.2f);
            // yield return new WaitUntil(() => !firePS);
            transform.position += movementDir * offsetSpeeding;
            while (active) {
                transform.position += Time.deltaTime * movementDir * speed;
                yield return null;
                if(!launched){
                    launched=true;
                }
            }
            yield return null;
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if(!launched){
            Debug.Log("notLaunched_exit");
            return;
        }
        Debug.Log(transform.name+":: Exit ::"+other.transform.name);
        if(transform.name.Substring(0,5) == "Sword") return;
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(!launched){
            Debug.Log("notLaunched_enter");
            // Vector3 movementDir =  (dest-transform.position).normalized; 
            // transform.position += Time.deltaTime * movementDir * speed * 100;
            return;
        }
        Debug.Log(transform.name+":: Enter ::"+other.transform.name);
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }
        // if bullet bumped into Player (character)
        if(other.gameObject.CompareTag("Player")){

            Debug.Log("..P..");
            int plind= int.Parse(other.transform.name[1].ToString());
            if(plind != myparent){
                // show BLOOODD
                GameObject blood = Instantiate(bloodObj) as GameObject;
                blood.transform.position = other.transform.position;

                Debug.Log("000DD))");
                sdata.vitalDatas[plind].health-=healthDec;
                // Disable chrc if died
                if(sdata.vitalDatas[plind].health<=0){
                    other.gameObject.SetActive(false);
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
