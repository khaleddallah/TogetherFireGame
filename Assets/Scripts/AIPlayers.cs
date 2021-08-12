using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayers : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject GoldParent, StoneParent, DiamondParent;
    [SerializeField] private GameObject PlayersParent;
    [SerializeField] private List<GameObject> gunsTypesObjects;
    
    List<Vector3> golds, stones, diamonds, players;
    LayerMask layerMove;
    LayerMask layerFire;
    float vhstep, slstep, radiousEnv;    
    Sdata sdata;

    void Start()
    {    
        sdata = Sdata.sdata;
        vhstep = sdata.gridCellSize;
        slstep = Mathf.Sqrt(2*Mathf.Pow(vhstep,2));
        
        layerFire = TargetAssignHelper.tah.fireLayersFiltered;
        layerMove = TargetAssignHelper.tah.moveLayersFiltered;
        radiousEnv = TargetAssignHelper.tah.radiousEnv;
       
        golds = new List<Vector3>();
        stones = new List<Vector3>();
        diamonds = new List<Vector3>();

        players = new List<Vector3>(4);
        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            Vector3 temp = PlayersParent.transform.GetChild(p).GetChild(0).position;
            players.Add(temp);
        }
        
    }



    public List<Vector3> MoveMarkers(int playerInd){
        List<Vector3> res = new List<Vector3>();
        Vector3 currentPos = PlayersParent.transform.GetChild(playerInd).GetChild(0).position;

        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 dir = new Vector3(x,y,0f);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit;
                float distance0=0f;                
                hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerMove);
                distance0 = hit.distance;

                if(distance0>vhstep){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
                    Vector3 last ; 
                    float lastPoint;
                    if(x==0 || y==0){
                        lastPoint = Mathf.Floor(distance0/vhstep);
                    }
                    else{
                        lastPoint = Mathf.Floor(distance0/slstep);
                    }
                    last = currentPos+(new Vector3(x, y, 0)*vhstep*lastPoint);
                    //draw points for available targets
                    for(int p=1; p<=lastPoint; p++){
                        Vector3 posTemp = currentPos+new Vector3(x, y, 0)*p*vhstep;
                        bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
                        if(isOutsideEnv){
                            continue;
                        }
                        res.Add(posTemp);
                    }
                }
            }
        }

        res.Add(currentPos);
        return res;
    }

    public List<Vector3> GrendeMarkers(int playerInd){
        List<Vector3> res = new List<Vector3>();
        Vector3 currentPos = sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target;
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridCellSize);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            for (int j=-(int)NumLines ; j<NumLines; j++ ){
                Vector3 posTemp = new Vector3(i*sdata.gridCellSize, j*sdata.gridCellSize, 0.0f);
                bool isCurrentPos = i*sdata.gridCellSize==currentPos.x && j*sdata.gridCellSize==currentPos.y ; 
                bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
                if(isCurrentPos || isOutsideEnv){
                    continue;
                }
                else{
                    res.Add(posTemp);
                }
            }
        }
        return res;
    }

    public List<Vector3> PistolMarkers(int playerInd){
        List<Vector3> res = new List<Vector3>();
        Vector3 currentPos = sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target;

        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 dir = new Vector3(x,y,0f);
                Debug.Log("dir::"+dir);
                RaycastHit2D hit;
                float distance0=0f;

                hit=Physics2D.Raycast(currentPos, dir, Mathf.Infinity, ~layerFire);
                distance0 = hit.distance;
            

                if(distance0>vhstep){
                    Debug.Log("hit::"+hit.transform.name+":::"+hit.distance);
                    Vector3 last ; 
                    float lastPoint;
                    if(x==0 || y==0){
                        lastPoint = Mathf.Floor(distance0/vhstep);
                    }
                    else{
                        lastPoint = Mathf.Floor(distance0/slstep);
                    }

                    last = currentPos+(new Vector3(x, y, 0)*vhstep);
                    res.Add(last);

                }
            }
        }
        return res;
    }

    public void UpdateGeneralData(){
        // resources
        collectRes(GoldParent, golds);
        collectRes(StoneParent, stones);
        collectRes(DiamondParent, diamonds);

        // players
        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            if(sdata.vitalDatas[p].health<=0){
                players[p] = new Vector3 (-999f,-999f,-999f);
            }
            else{
                players[p] = PlayersParent.transform.GetChild(p).GetChild(0).position;
            }
        }
    }

    public void collectRes(GameObject XP, List<Vector3> x){
        x.Clear();
        foreach(Transform child in XP.transform)
        {
            x.Add(child.position);
        }
    }


    public void PlayerAI(int playerInd){
        Debug.Log("FromPlayerAI:"+playerInd);
        // select the move target 
        List<Vector3> possibleMoves = new List<Vector3>();
        possibleMoves = MoveMarkers(playerInd);
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].type = "move";
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target = possibleMoves[Random.Range(0, possibleMoves.Count)];

        // select fire 
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].type = "fire";

        // select a gun
        GameObject gun = gunsTypesObjects[Random.Range(0, gunsTypesObjects.Count)];
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].gunTypeObj = gun;

        // select fire target
        string gunName = gun.transform.name;
        List<Vector3> possibleTargets = new List<Vector3>();
        if(gunName=="Grenade"){
            possibleTargets = GrendeMarkers(playerInd);
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].target = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
        else if(gunName=="Pistol"||gunName=="MachineGun"){
            possibleTargets = PistolMarkers(playerInd);
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].target = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
    }

    public void psai(){
        UpdateGeneralData();

        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            if(sdata.vitalDatas[p].health>0 && p!=sdata.playerIndex){
                Debug.Log("##P:"+p);
                PlayerAI(p);
            }
        }

    }


}
