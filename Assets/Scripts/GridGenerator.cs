using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject playersParent;
    [SerializeField] private GameObject goldObject;
    [SerializeField] private GameObject goldParent;
    [SerializeField] private GameObject stoneObject;
    [SerializeField] private GameObject stoneParent;
    [SerializeField] private GameObject diamondObject;
    [SerializeField] private GameObject diamoneParent;
    [SerializeField] private float offsetCameraShift;
    [SerializeField] private float offsetCameraRotation;
    [SerializeField] private float playerPositionOffset;

    Sdata sdata;

    void Start()
    {
        sdata = Sdata.sdata;
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
        playersParent.transform.GetChild(0).GetChild(0).transform.position = new Vector3(0f, -1*playerPositionOffset*sdata.gridCellSize, 0f);
        playersParent.transform.GetChild(1).GetChild(0).transform.position = new Vector3(-1*playerPositionOffset*sdata.gridCellSize, 0f, 0f);
        playersParent.transform.GetChild(2).GetChild(0).transform.position = new Vector3(0f, playerPositionOffset*sdata.gridCellSize, 0f);
        playersParent.transform.GetChild(3).GetChild(0).transform.position = new Vector3(playerPositionOffset*sdata.gridCellSize, 0f, 0f);

        for(int i=0 ; i<sdata.participantNum ; i++){
            if(i==sdata.playerIndex){
                playersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
            else{
                playersParent.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void GoldDistributor(){
        for(int x = -4 ; x<=4 ; x+=2){
            for(int y = -4 ; y<=4 ; y+=2){
                if(x==0 && y==0){
                    continue;
                }
                if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(goldObject) as GameObject;
                    g.transform.SetParent(goldParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridCellSize, y*sdata.gridCellSize, 0f);
                }
            }
        }
        for(int x = -3 ; x<=3 ; x+=3){
            for(int y = -3 ; y<=3 ; y+=3){
                // if(Mathf.Abs(x)!=Mathf.Abs(y)){
                    // Debug.Log("("+x+","+y+")");
                    GameObject g = Instantiate(goldObject) as GameObject;
                    g.transform.SetParent(goldParent.transform);
                    g.transform.position = new Vector3(x*sdata.gridCellSize, y*sdata.gridCellSize, 0f);
                // }
            }
        }
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
        float x = Mathf.Cos((-90*(sdata.playerIndex+1))*Mathf.Deg2Rad) * offsetCameraShift;
        float y = Mathf.Sin((-90*(sdata.playerIndex+1))*Mathf.Deg2Rad) * offsetCameraShift;
        
        float rotx = Mathf.Cos((-90*(sdata.playerIndex+2))*Mathf.Deg2Rad) * offsetCameraRotation;
        float roty = Mathf.Sin((-90*(sdata.playerIndex+2))*Mathf.Deg2Rad) * offsetCameraRotation;     
        float rotz = sdata.playerIndex*(-90);


        // Debug.Log("yangle:"+(-90*(sdata.playerIndex+1)));
        // Debug.Log("ysin"+Mathf.Sin((-90*(sdata.playerIndex+1))*Mathf.Deg2Rad));
        // Debug.Log("x:"+x+"  y:"+y);
        // Debug.Log("rotxyz"+rotx+","+roty+","+rotz);

        cam.transform.position = new Vector3(x,y,-10f);
        cam.transform.rotation = Quaternion.Euler( rotx, roty, rotz);

        
        for(int i=0; i<sdata.participantNum; i++){
            playersParent.transform.GetChild(i).GetChild(0).transform.rotation = Quaternion.Euler( 0, 0, rotz);
        }
    
    }

}
