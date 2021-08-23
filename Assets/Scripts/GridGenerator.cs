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

    void Start()
    {
        sdata = Sdata.sdata;
        playerPositionOffset = Mathf.FloorToInt(sdata.maxRadius/sdata.gridCellSize)-2;
        GenerateGrid();
        AdjustPlayersPositions();
        Camera_Players_Adjusting();
        GoldDistributor();
        StoneDistributor();
        DiamondDistributor();
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
                // playersParent.transform.GetChild(i).GetChild(0).transform.position = new Vector3(0f, y*playerPositionOffset*sdata.gridCellSize, 0f);      
                // playersParent.transform.GetChild(i).gameObject.SetActive(true);
                for(int c = 0; c<sdata.charactersNum; c++){
                    GameObject g = CreateObject(2*(c-1), y*playerPositionOffset, playersTemplates[i], playersParent.transform.GetChild(i).gameObject);
                    g.transform.name = "P"+i.ToString()+c.ToString();
                }

                y+=2;
                i++;
                continue;
            }
            if(x<=1){
                // playersParent.transform.GetChild(i).GetChild(0).transform.position = new Vector3(x*playerPositionOffset*sdata.gridCellSize, 0f, 0f);      
                // playersParent.transform.GetChild(i).gameObject.SetActive(true);
                // CreateObject(x*playerPositionOffset, -2, playersParent.transform.GetChild(i).GetChild(0).gameObject, playersParent.transform.GetChild(i).gameObject);
                // CreateObject(x*playerPositionOffset, +2, playersParent.transform.GetChild(i).GetChild(0).gameObject, playersParent.transform.GetChild(i).gameObject);
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
                    CreateObject(x, y, goldObject3, goldParent);
                }
                else if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    if(Mathf.Abs(x)==4 || Mathf.Abs(y)==4){
                        CreateObject(x, y, goldObject1, goldParent);
                    }
                    else{
                        CreateObject(x, y, goldObject2, goldParent);
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

    private void StoneDistributor(){
        for(int x = -2 ; x<=2 ; x+=2){
            for(int y = -2 ; y<=2 ; y+=2){
                if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    // Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(stoneObject) as GameObject;
                    g.transform.SetParent(stoneParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridCellSize, y*sdata.gridCellSize, 0f);
                }
            }
        }
    }

    private void DiamondDistributor(){
        int x = 0;
        int y = 0;
        GameObject g = Instantiate(diamondObject) as GameObject;
        g.transform.SetParent(diamoneParent.transform);
        g.transform.position = new Vector3(x*sdata.gridCellSize, y*sdata.gridCellSize, 0f);
    }

    private void Camera_Players_Adjusting(){
        float x = Mathf.Cos((-90*(ModPosPlayers[sdata.playerIndex]+1))*Mathf.Deg2Rad) * offsetCameraShift;
        float y = Mathf.Sin((-90*(ModPosPlayers[sdata.playerIndex]+1))*Mathf.Deg2Rad) * offsetCameraShift;
        
        float rotx = Mathf.Cos((-90*(ModPosPlayers[sdata.playerIndex]+2))*Mathf.Deg2Rad) * offsetCameraRotation;
        float roty = Mathf.Sin((-90*(ModPosPlayers[sdata.playerIndex]+2))*Mathf.Deg2Rad) * offsetCameraRotation;     
        float rotz = ModPosPlayers[sdata.playerIndex]*(-90);


        // Debug.Log("yangle:"+(-90*(sdata.playerIndex+1)));
        // Debug.Log("ysin"+Mathf.Sin((-90*(sdata.playerIndex+1))*Mathf.Deg2Rad));
        // Debug.Log("x:"+x+"  y:"+y);
        // Debug.Log("rotxyz"+rotx+","+roty+","+rotz);

        cam.transform.position = new Vector3(x,y,-10f);
        cam.transform.rotation = Quaternion.Euler( rotx, roty, rotz);

        
        for(int i=0; i<sdata.participantNum; i++){
            for(int c=0; c<sdata.charactersNum; c++){
                playersParent.transform.GetChild(i).GetChild(c).transform.rotation = Quaternion.Euler( 0, 0, rotz);
            }
        }
    
    }

}
