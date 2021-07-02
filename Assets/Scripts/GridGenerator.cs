using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject line;
    public GameObject PlayersParent;

    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
        generateGrid();
    }


    public void generateGrid(){
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridLen);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            GameObject x = Instantiate(line) as GameObject;
            x.transform.SetParent(gameObject.transform);
            x.transform.position = new Vector3(i*sdata.gridLen, 0f, 0f);
        }
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            GameObject x = Instantiate(line) as GameObject;
            x.transform.SetParent(gameObject.transform);
            x.transform.Rotate(0,0,90);
            x.transform.position = new Vector3(0f, i*sdata.gridLen, 0f);
        }
        
        PlayersParent.transform.GetChild(0).GetChild(0).transform.position = new Vector3(0f, -6*sdata.gridLen, 0f);
        PlayersParent.transform.GetChild(1).GetChild(0).transform.position = new Vector3(-6*sdata.gridLen, 0f, 0f);
        PlayersParent.transform.GetChild(2).GetChild(0).transform.position = new Vector3(0f, 6*sdata.gridLen, 0f);
        PlayersParent.transform.GetChild(3).GetChild(0).transform.position = new Vector3(6*sdata.gridLen, 0f, 0f);

        for(int i=0 ; i<sdata.participantNum ; i++){
            if(i==sdata.playerIndex){
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else{
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }

    
    }

}
