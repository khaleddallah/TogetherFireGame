using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public GameObject goldPS;
    Sdata sdata;
    void Start()
    {
        sdata = Sdata.sdata;
        float rotz = sdata.playerIndex*(-90);
        transform.rotation = Quaternion.Euler( 0, 0, rotz);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            GameObject gps = Instantiate(goldPS) as GameObject;
            gps.transform.position = transform.position;
            float rotz = sdata.playerIndex*(-90);
            gps.transform.rotation = Quaternion.Euler( 0, 0, rotz);
            
            Debug.Log(other.name);
            sdata.vitalDatas[(int)Char.GetNumericValue(other.gameObject.transform.name[1])].golds +=1;
            GM.gm.updataMGH();

            GM.gm.StartCheckGoldWinner();

            Destroy(gameObject);
        }

    }
}
