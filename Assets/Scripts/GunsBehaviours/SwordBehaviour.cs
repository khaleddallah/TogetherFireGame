using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehaviour : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float healthDecreaseValue;
    [SerializeField] private List<string> barriers;
    [SerializeField] private GameObject bloodParticleSystemObject;
    [SerializeField] private float MinSwordAngle;
    [SerializeField] private float MaxSwordAngle;

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
        Debug.Log("####"+transform.name.Substring(0,5));
        if(AssertSword()){
            float z=MinSwordAngle; 
            while(z<=MaxSwordAngle && active)
            {
                z+=1*speed*Time.deltaTime;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
                yield return null;
            }
            DestroySpecial();
            yield return null;
        }
    }

    private bool AssertSword(){
        return transform.name.Substring(0,5)=="Sword";

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(transform.name+":: Enter ::"+other.transform.name);

        if(other.gameObject.CompareTag("Player")){
            int plind= int.Parse(other.transform.name[1].ToString());
            int chind= int.Parse(other.transform.name[2].ToString());

            if(plind != myParent){
                ShowBlood(other.gameObject);
                sdata.vitalDatas[plind].health[chind]-=healthDecreaseValue;
                // Disable chrc if died
                if(sdata.vitalDatas[plind].health[chind]<=0){
                    other.gameObject.SetActive(false);
                }
                GM.gm.updataMGH();
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

    private void ShowBlood(GameObject other){
        GameObject blood = Instantiate(bloodParticleSystemObject) as GameObject;
        blood.transform.position = other.transform.position;

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
