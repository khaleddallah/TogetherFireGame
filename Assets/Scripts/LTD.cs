using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongTermData : MonoBehaviour
{

    public static LongTermData ltd;
    public string serverURL = "";
    public string myName = "";
    public int playerIndex;

    void Awake()
    {
        if(ltd != null){
            GameObject.Destroy(ltd.gameObject);
        }
        else{
            ltd = this;
        }

        DontDestroyOnLoad(this);
    }
}
