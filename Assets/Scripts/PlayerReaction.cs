using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReaction : MonoBehaviour
{
    // Start is called before the first frame update
    public int playerIndex;
    Sdata sdata;
    void Start()
    {
        sdata = Sdata.sdata;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Gold")){
            Destroy(other.gameObject);
            sdata.vitalDatas[playerIndex].golds +=1;
            GM.gm.updataMGH();
        }

    }
}
