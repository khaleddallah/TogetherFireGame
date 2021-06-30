using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    Sdata sdata;
    void Start()
    {
        sdata = Sdata.sdata;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            Debug.Log(other.name);
            sdata.vitalDatas[(int)Char.GetNumericValue(other.gameObject.transform.name[1])].golds +=1;
            GM.gm.updataMGH();
            Destroy(gameObject);
        }

    }
}
