using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public GameObject diamondPS;
    Sdata sdata;
    void Start()
    {
        sdata = Sdata.sdata;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            GameObject dps = Instantiate(diamondPS) as GameObject;
            dps.transform.position = transform.position;

            Debug.Log(other.name);
            sdata.vitalDatas[(int)Char.GetNumericValue(other.gameObject.transform.name[1])].golds +=1;
            GM.gm.updataMGH();
            Destroy(gameObject);
        }

    }
}
