using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class uib_gunType : MonoBehaviour
{
    [SerializeField] private GameObject fireButtonS;

    public string gunType;

    public Color colorAfterPressed;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Image>().color = Color.white;
    }


    public void GunTypePressed(){
        GetComponent<Image>().color = colorAfterPressed;
        GM.gm.gd.episodes[GM.gm.episodeIndex].roleplays[GM.gm.gd.playerIndex].actions[GM.gm.actionIndex].gunType = gunType;

        for(int i=0; i<fireButtonS.transform.GetChildCount(); i++){
            if(fireButtonS.transform.GetChild(i).GetComponent<uib_gunType>().gunType!=gunType){
                fireButtonS.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }


    }
}
