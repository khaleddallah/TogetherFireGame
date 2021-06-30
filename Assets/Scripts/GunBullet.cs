using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBullet : MonoBehaviour
{

    public float speed;
    public float healthDec;
    public List<string> barriers = new List<string>();

    public Vector3 dest;

    Coroutine coroutineFire;
    bool active;
    Sdata sdata;
    int myparent;


    void Start()
    {
        sdata = Sdata.sdata;
    }

    public void launch(int myparent0)
    {
        active = true;
        coroutineFire = StartCoroutine(fire(myparent0));
    }


    public IEnumerator fire(int parent){
        myparent=parent;
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
            while (active) {
                transform.position += Time.deltaTime * movementDir * speed;
                yield return null;
            }
            yield return null;
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(transform.name+":: Exit ::"+other.transform.name);
        if(transform.name.Substring(0,5) == "Sword") return;
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(transform.name+":: Enter ::"+other.transform.name);
        if(other.transform.name == "MainBase") {
            DestroySpecial();
        }
        // if bullet bumped into Player (character)
        if(other.gameObject.CompareTag("Player")){
            Debug.Log("..P..");
            int plind= int.Parse(other.transform.name[1].ToString());
            if(plind != myparent){
                Debug.Log("000DD))");
                sdata.vitalDatas[plind].health-=healthDec;
                DestroySpecial();
            }
            else{
                return;
            }
        }

        // if bullet bumped into barriers (that are assigned into the Inspector )
        else if(barriers.Contains(other.gameObject.tag)) {
            DestroySpecial();
        }
    }


    void DestroySpecial()
    {
        if(gameObject && active){
            active=false; // stop the movement of the bullet
            StopCoroutine(coroutineFire); 
            Debug.Log("!! Destroy :: "+gameObject.transform.name);
            GM.gm.transform.gameObject.GetComponent<EpisodeMngr>().actionFire-=1; // complete to the other action
            Destroy(gameObject);
        }
    }

}
