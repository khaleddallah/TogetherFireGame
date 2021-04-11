using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_bullet : MonoBehaviour
{

    public GameObject dest;
    public float speed;
    
    public List<GameObject> barriers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fire());
    }

    // Update Gun_bullet.csis called once per frame
    void Update()
    {
        
    }


    public IEnumerator fire(){
        Debug.Log("####"+transform.name.Substring(0,5));
        if(transform.name.Substring(0,5) == "Sword"){
            float z = 0 ; 
            Debug.Log("Sword");
            while(z<=359)
            {
            z+=1*speed*Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
            yield return null;
            }
            Destroy(gameObject);
        }
        else{
            Vector3 movementDir =  (dest.transform.position-transform.position).normalized; 

            float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz-90);

            while (true) {
                transform.position += Time.deltaTime * movementDir * speed;
                yield return null;
            }
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(transform.name+":: Exit ::"+other.transform.name);
        if(other.transform.name == "MainBase") Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(transform.name+":: Enter ::"+other.transform.name);
        if(barriers.Contains(other.transform.gameObject)) Destroy(gameObject);
    }


    void OnDestroy()
    {
        GM.gm.transform.gameObject.GetComponent<phys_action>().working=false; 
        GM.gm.transform.gameObject.GetComponent<phys_action>().action+=1; 

    }

}
