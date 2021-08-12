using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongTermData : MonoBehaviour
{

    public static LongTermData longTermData;
    public string serverURL = "";
    public string myName = "";
    public int playerIndex;

    void Awake()
    {
        if(longTermData != null){
            GameObject.Destroy(longTermData.gameObject);
        }
        else{
            longTermData = this;
        }

        DontDestroyOnLoad(this);
    }
}
