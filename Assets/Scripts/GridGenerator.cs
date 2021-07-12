using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject line;
    public GameObject PlayersParent;
    public GameObject gold;
    public GameObject GoldParent;

    public GameObject stone;
    public GameObject StoneParent;

    public GameObject diamond;
    public GameObject DiamondParent;
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
        
        PlayersParent.transform.GetChild(0).GetChild(0).transform.position = new Vector3(0f, -12*sdata.gridLen, 0f);
        PlayersParent.transform.GetChild(1).GetChild(0).transform.position = new Vector3(-12*sdata.gridLen, 0f, 0f);
        PlayersParent.transform.GetChild(2).GetChild(0).transform.position = new Vector3(0f, 12*sdata.gridLen, 0f);
        PlayersParent.transform.GetChild(3).GetChild(0).transform.position = new Vector3(12*sdata.gridLen, 0f, 0f);

        for(int i=0 ; i<sdata.participantNum ; i++){
            if(i==sdata.playerIndex){
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else{
                PlayersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }

        GoldDistributor();
        StoneDistributor();
        DiamondDistributor();
    }



    public void GoldDistributor(){
        for(int x = -4 ; x<=4 ; x+=2){
            for(int y = -4 ; y<=4 ; y+=2){
                if(x==0 && y==0){
                    continue;
                }
                if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(gold) as GameObject;
                    g.transform.SetParent(GoldParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridLen, y*sdata.gridLen, 0f);
                }
            }
        }
        for(int x = -3 ; x<=3 ; x+=3){
            for(int y = -3 ; y<=3 ; y+=3){
                // if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    // Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(gold) as GameObject;
                    g.transform.SetParent(GoldParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridLen, y*sdata.gridLen, 0f);
                // }
            }
        }
    }


    public void StoneDistributor(){
        for(int x = -2 ; x<=2 ; x+=2){
            for(int y = -2 ; y<=2 ; y+=2){
                if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    // Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(stone) as GameObject;
                    g.transform.SetParent(StoneParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridLen, y*sdata.gridLen, 0f);
                }
            }
        }
    }


    public void DiamondDistributor(){
        int x = 0;
        int y = 0;
        GameObject g = Instantiate(diamond) as GameObject;
        g.transform.SetParent(DiamondParent.transform);
        g.transform.position = new Vector3(x*sdata.gridLen, y*sdata.gridLen, 0f);
    }

}
