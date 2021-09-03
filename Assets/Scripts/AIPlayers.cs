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

        players = new List<Vector3>();
        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            for(int c=0; c<sdata.charactersNum; c++){
                Vector3 temp = new Vector3 (-999f,-999f,-999f);
                // Vector3 temp = PlayersParent.transform.GetChild(p).GetChild(c).position;
                players.Add(temp);
            }
        }
        
    }


    public void UpdateGeneralData(){
        // resources
        collectRes(GoldParent, golds);
        collectRes(StoneParent, stones);
        collectRes(DiamondParent, diamonds);

        // players
        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            for(int c=0 ; c<sdata.charactersNum; c++){
                if(sdata.vitalDatas[p].health[c]<=0){
                    players[p] = new Vector3 (-999f,-999f,-999f);
                }
                else{
                    players[p] = PlayersParent.transform.GetChild(p).GetChild(0).position;
                }
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

    private Vector3 GetPlayerPositionByIndex(int index, int cindex){
        Vector3 currentPos = PlayersParent.transform.GetChild(index).GetChild(cindex).position;
        return currentPos;
    }


    public void PlayerAI(int playerInd){
        Debug.Log("FromPlayerAI:"+playerInd);

        // select the character 
        int cindexTmp = Random.Range(0, sdata.charactersNum);
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].cindex = cindexTmp;

        // select the move target 
        List<List<Vector3>> possibleMoves;
        possibleMoves = TargetAssignHelper.tah.GetMovePossiblePoints(GetPlayerPositionByIndex(playerInd,cindexTmp));
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].type = "move";
        List<List<Vector3>> goldDirections = GetGoldDirection(possibleMoves);
        if(goldDirections.Count>0){
            List<Vector3> SelectedDirection = goldDirections[Random.Range(0, goldDirections.Count)];
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target = SelectedDirection[SelectedDirection.Count-1];
        }
        else{
            List<Vector3> SelectedDirection = possibleMoves[Random.Range(0, possibleMoves.Count)];
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target = SelectedDirection[Random.Range(0, SelectedDirection.Count)];
        }

        // select fire 
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].type = "fire";

        // select a gun
        GameObject gun = gunsTypesObjects[Random.Range(0, gunsTypesObjects.Count)];
        sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].gunTypeObj = gun;

        // select fire target
        string gunName = gun.transform.name;
        List<Vector3> possibleTargets = new List<Vector3>();
        Vector3 playerPosition = sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[0].target;
        if(gunName=="Grenade"){
            possibleTargets = TargetAssignHelper.tah.GetGrenadeMarkers(playerPosition);
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].target = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
        else if(gunName=="Pistol"||gunName=="MachineGun"){
            possibleTargets = TargetAssignHelper.tah.GetPistolLinesPointsMarkers(playerPosition);
            sdata.episodes[sdata.episodeIndex].roleplays[playerInd].actions[1].target = possibleTargets[Random.Range(0, possibleTargets.Count)];
        }
    }


    private List<List<Vector3>> GetGoldDirection(List<List<Vector3>> directions){
        List<List<Vector3>> res = new List<List<Vector3>>();
        foreach (var direction in directions)
        {
            foreach (var gold in golds)
            {
                if(direction[direction.Count-1]==gold){
                    res.Add(direction);
                    break;
                }
            }
        }
        Debug.Log(res.Count);
        return res;
    }


    public void psai(){
        UpdateGeneralData();

        for(int p = 0 ; p<sdata.participantNum ; p++)
        {
            if(GM.gm.GetWholeHealth(p)>0 && p!=sdata.playerIndex){
                Debug.Log("##P:"+p);
                PlayerAI(p);
            }
        }
    }




}
