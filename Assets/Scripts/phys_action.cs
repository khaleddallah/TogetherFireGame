using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phys_action : MonoBehaviour
{

    public float wholeTime = 10.0f;
    public GameObject dest;
    public Vector3 origin;

    void Start()
    {
        origin = transform.position;
        StartCoroutine(move());
        // StartCoroutine(Move_Routine(this.transform, origin, dest.transform.position));

        // Debug.Log((dest.transform.position-transform.position).normalized);
    }

    void Update()
    {
        // Vector3 xy =  (dest.transform.position-transform.position).normalized; 
        // Debug.Log(xy);

        // if(xy.y>0){
            // transform.position += Time.fixedDeltaTime * xy * speed;
        // }
    }


    public IEnumerator move() {
        Vector3 movementDir =  (dest.transform.position-transform.position).normalized;         

        float distance = Vector3.Distance(dest.transform.position, transform.position);
        float speed0 = distance/wholeTime;
        float t0 = 0f;

        while (t0<wholeTime) {
            transform.position += Time.deltaTime * movementDir * speed0;
            t0+=Time.deltaTime;
            yield return null;
        }
    }



}
