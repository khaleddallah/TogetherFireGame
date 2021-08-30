using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject playersParent;
    [SerializeField] private GameObject goldObject1;
    [SerializeField] private GameObject goldObject2;
    [SerializeField] private GameObject goldObject3;

    [SerializeField] private GameObject goldParent;
    [SerializeField] private GameObject stoneObject;
    [SerializeField] private GameObject stoneParent;
    [SerializeField] private GameObject diamondObject;
    [SerializeField] private GameObject diamoneParent;
    [SerializeField] private float offsetCameraShift;
    [SerializeField] private float offsetCameraRotation;
    [SerializeField] private int playerPositionOffset;
    [SerializeField] private List<GameObject> playersTemplates;
    [SerializeField] private int[] ModPosPlayers = {0,2,1,3};
    Sdata sdata;
    public bool isGenerated;

    void Start(){
        Start0();
    }
    public void Start0()
    {
        if(!isGenerated){
            sdata = Sdata.sdata;
            playerPositionOffset = Mathf.FloorToInt(sdata.maxRadius/sdata.gridCellSize)-2;
            GenerateGrid();
            AdjustPlayersPositions();
            Camera_Players_Adjusting();
            GoldDistributor();
            isGenerated=true;

            // StoneDistributor();
            // DiamondDistributor();
        }
    }


    private void GenerateGrid(){
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridCellSize);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            GameObject x = Instantiate(lineObject) as GameObject;
            x.transform.SetParent(gameObject.transform);
            x.transform.position = new Vector3(i*sdata.gridCellSize, 0f, 0f);
        }
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            GameObject x = Instantiate(lineObject) as GameObject;
            x.transform.SetParent(gameObject.transform);
            x.transform.Rotate(0,0,90);
            x.transform.position = new Vector3(0f, i*sdata.gridCellSize, 0f);
        }
    }

    private void AdjustPlayersPositions(){
        int i = 0;
        int x = -1; 
        int y = -1;
        foreach (Transform player in playersParent.transform)
        {
            player.gameObject.SetActive(false);
        }

        while(i<sdata.participantNum){
            if(y<=1){
                for(int c = 0; c<sdata.charactersNum; c++){
                    GameObject g = CreateObject(2*(c-1), y*playerPositionOffset, playersTemplates[i], playersParent.transform.GetChild(i).gameObject);
                    g.transform.name = "P"+i.ToString()+c.ToString();
                }

                y+=2;
                i++;
                continue;
            }
            if(x<=1){
                for(int c = 0; c<sdata.charactersNum; c++){
                    GameObject g = CreateObject(x*playerPositionOffset, 2*(c-1), playersTemplates[i], playersParent.transform.GetChild(i).gameObject);
                    g.transform.name = "P"+i.ToString()+c.ToString();
                }

                x+=2;
                i++;
            }
        }
    }

    private void GoldDistributor(){
        for(int x = -4 ; x<=4 ; x+=2){
            for(int y = -4 ; y<=4 ; y+=2){
                if(x==0 && y==0){
                    GameObject g = CreateObject(x, y, goldObject3, goldParent);
                    g.transform.rotation =  playersParent.transform.GetChild(0).GetChild(0).transform.rotation;
                }
                else if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    if(Mathf.Abs(x)==4 || Mathf.Abs(y)==4){
                        GameObject g = CreateObject(x, y, goldObject1, goldParent);
                        g.transform.rotation =  playersParent.transform.GetChild(0).GetChild(0).transform.rotation;
                    }
                    else{
                        GameObject g = CreateObject(x, y, goldObject2, goldParent);
                        g.transform.rotation =  playersParent.transform.GetChild(0).GetChild(0).transform.rotation;
                    }
                }
            }
        }
    }

    private GameObject CreateObject(int x, int y, GameObject gameObject, GameObject parent){
        GameObject g = Instantiate(gameObject) as GameObject;
        g.transform.SetParent(parent.transform);
        g.transform.position = new Vector3(x*sdata.gridCellSize, y*sdata.gridCellSize, 0f);
        return g;
    }



    private void Camera_Players_Adjusting(){
        float x = Mathf.Cos((-90*(ModPosPlayers[sdata.playerIndex]+1))*Mathf.Deg2Rad) * offsetCameraShift;
        float y = Mathf.Sin((-90*(ModPosPlayers[sdata.playerIndex]+1))*Mathf.Deg2Rad) * offsetCameraShift;
        
        float rotx = Mathf.Cos((-90*(ModPosPlayers[sdata.playerIndex]+2))*Mathf.Deg2Rad) * offsetCameraRotation;
        float roty = Mathf.Sin((-90*(ModPosPlayers[sdata.playerIndex]+2))*Mathf.Deg2Rad) * offsetCameraRotation;     
        float rotz = ModPosPlayers[sdata.playerIndex]*(-90);

        cam.transform.position = new Vector3(x,y,-10f);
        cam.transform.rotation = Quaternion.Euler( rotx, roty, rotz);

        for(int i=0; i<sdata.participantNum; i++){
            for(int c=0; c<sdata.charactersNum; c++){
                playersParent.transform.GetChild(i).GetChild(c).transform.rotation = Quaternion.Euler( 0, 0, rotz);
            }
        }
    
    }

}
