using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetAssignHelper : MonoBehaviour
{

    [SerializeField] private GameObject targetsParent;
    [SerializeField] private GameObject fireTarget;
    [SerializeField] private GameObject charactersParent;
    [SerializeField] private GameObject lineRendererObject;
    [SerializeField] private GameObject circleMarker;
    [SerializeField] private GameObject circleGrenadeMarker;
    [SerializeField] private GameObject circlePistolMarker;
    [SerializeField] private GameObject markersParent;
    [SerializeField] private Color fireLinesTempMarkerColor;
    [SerializeField] private Color fireLineFinalMarkerColor;
    [SerializeField] private Color moveLinesTempMarkerColor;
    [SerializeField] private Color moveLineFinalMarkerColor;
    [SerializeField] private GameObject swordTargetMarker;
    [SerializeField] private float circleMarkerZoffset = -0.03f;

    
    public static TargetAssignHelper tah;
    public float radiousEnv;
    public LayerMask moveLayersFiltered;
    public LayerMask fireLayersFiltered;


    private const int MOVE_ACTION_INDEX = 0;
    private Sdata sdata;
    private Vector3 currentPosition;
    private GameObject myCharacter;
    private float vhStep;
    private float slStep;
    private float maxRadius;

    void Awake()
    {
        if(tah != null){
            GameObject.Destroy(tah);
        }
        else{
            tah = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        sdata = Sdata.sdata;
        // myCharacter = charactersParent.transform.GetChild(sdata.playerIndex).GetChild(0).gameObject;
        // Debug.Log("myCharacter:"+myCharacter.transform.position);
        maxRadius = Mathf.Sqrt(Mathf.Pow(Mathf.Floor(1.5f*sdata.gridRadious)*sdata.gridCellSize,2)+Mathf.Pow(Mathf.Floor(0.5f*sdata.gridRadious)*sdata.gridCellSize,2));
        Debug.Log(maxRadius);
        vhStep = sdata.gridCellSize;
        slStep = Mathf.Sqrt(2*Mathf.Pow(vhStep,2));
    }

    // ======= choose selected character =========
    public void DrawSelectionMarkers(){
        foreach (var point in GetSelectionMarkers(sdata.playerIndex))
        {
            CreateCircleMarker(point, circleMarker);
        }
    }



    public List<Vector3> GetSelectionMarkers(int playerIndex){
        List<Vector3> res = new List<Vector3>();
        for(int cindex = 0; cindex<sdata.charactersNum; cindex++){
            res.Add(charactersParent.transform.GetChild(playerIndex).GetChild(cindex).transform.position);
        }
        return res;
    }

    // ======= sword =========

    public void MarkerSword(){
        DestroyMarkers();
        SetCurrentPositionAfterMove();
        swordTargetMarker.SetActive(true);
        swordTargetMarker.transform.position = currentPosition;
        float rotz = sdata.playerIndex*(-90);
        swordTargetMarker.transform.rotation = Quaternion.Euler( 0, 0, rotz);
    }

    public void MarkerSwordApply(){
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = currentPosition;
        swordTargetMarker.SetActive(false);
    }

    private void SetCurrentPositionAfterMove(){
        currentPosition = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[MOVE_ACTION_INDEX].target;
    }

    public void DestroyMarkers(){
        swordTargetMarker.SetActive(false);
        foreach(Transform child in markersParent.transform)
        {
            Destroy(child.gameObject);
        }
    }


    // ======= Grenade =========
    public void DrawGrendeMarkers(){
        DestroyMarkers();
        SetCurrentPositionAfterMove();
        DrawGrenadeMarkers();
    }

    private void DrawGrenadeMarkers(){
        foreach (var point in GetGrenadeMarkers(currentPosition))
        {
            CreateCircleMarker(point, circleGrenadeMarker);
        }
    }

    public List<Vector3> GetGrenadeMarkers(Vector3 origin){
        List<Vector3> res = new List<Vector3>();
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridCellSize);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            for (int j=-(int)NumLines ; j<NumLines; j++ ){
                Vector3 posTemp = new Vector3(i*sdata.gridCellSize, j*sdata.gridCellSize, 0.0f);
                bool iscurrentPosition = i*sdata.gridCellSize==currentPosition.x && j*sdata.gridCellSize==currentPosition.y ; 
                bool isOutsideEnv = !CheckIfInsideEnv(posTemp);
                if(iscurrentPosition || isOutsideEnv){
                    continue;
                }
                else{
                    Vector3 point = new Vector3(i*sdata.gridCellSize, j*sdata.gridCellSize, -0.03f);
                    res.Add(point);
                }
            }
        }
        return res;
    }


    // ======= Pistol =========
    public void DrawPistolMarkers(){
        DestroyMarkers();
        SetCurrentPositionAfterMove();
        DrawPistolLinesPointsMarkers();
    }

    private void DrawPistolLinesPointsMarkers(){
        foreach (var point in GetPistolLinesPointsMarkers(currentPosition))
        {
            CreateLineRenderer(fireLinesTempMarkerColor, 0.05f, point, currentPosition, markersParent);
            CreateArrowMarker(point);
        }
    }


    public List<Vector3> GetPistolLinesPointsMarkers(Vector3 origin){
        List<Vector3> res = new List<Vector3>();
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                Vector3 lastPoint = origin+(new Vector3(x, y, 0)*vhStep);
                if(CheckIfInsideEnv(lastPoint) && !(x==0 && y==0)){
                    res.Add(lastPoint);
                }
            }
        }
        return res;
    }


    private void CreateArrowMarker(Vector3 lastPoint){
        GameObject cr0 = Instantiate(circlePistolMarker) as GameObject;
        cr0.transform.SetParent(markersParent.transform);
        Vector3 movementDir =  (lastPoint-currentPosition).normalized;    
        float rotz = Mathf.Atan2(movementDir.y,movementDir.x)*Mathf.Rad2Deg;    
        cr0.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotz);
        cr0.transform.position =  lastPoint;
        cr0.transform.position = new Vector3(cr0.transform.position.x, cr0.transform.position.y, circleMarkerZoffset);
    }




    // ===== move =====
    public void DrawMoveMarkers(){
        DestroyMarkers();
        SetCurrentPosition();
        DrawMoveLinesPointsMarkers();
    }

    private void SetCurrentPosition(){
        myCharacter = charactersParent.transform.GetChild(sdata.playerIndex).GetChild(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].cindex).gameObject;
        currentPosition = myCharacter.transform.position;
    }

    private void DrawMoveLinesPointsMarkers(){
        foreach (var oneDirectionPoints in GetMovePossiblePoints(currentPosition))
        {
            CreateLineRenderer(moveLinesTempMarkerColor, 0.05f, oneDirectionPoints[oneDirectionPoints.Count-1], currentPosition, markersParent);
            foreach (var point in oneDirectionPoints)
            {
                CreateCircleMarker(point, circleMarker);
            }
        }
    }

    public List<List<Vector3>> GetMovePossiblePoints(Vector3 origin){
        List<List<Vector3>> res = new List<List<Vector3>>();
        List<Vector3> temp = new List<Vector3>();
        temp.Add(origin);
        res.Add(temp);
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                var tempMoveMarkersOnDirection = GetMoveMarkersOnDirection(x, y, origin);
                if (tempMoveMarkersOnDirection.Count>0){
                    res.Add(tempMoveMarkersOnDirection);
                }
            }
        }
        return res;
    }


    private List<Vector3> GetMoveMarkersOnDirection(int x, int y, Vector3 origin){
        List<Vector3> points = new List<Vector3>();
        RaycastHit2D hit = RaycastToDirection(origin, x, y, moveLayersFiltered);
        float distance = hit.distance;
        if(CheckHitGold(hit)){
            distance += vhStep;
            float NumOfPoints = GetNumOfPoints(distance, x, y, origin);
            Vector3 lastPoint = origin+new Vector3(x, y, 0)*(NumOfPoints)*vhStep;
            for(int p=1; p<=NumOfPoints; p++){
                Vector3 posTemp = origin+new Vector3(x, y, 0)*p*vhStep;
                points.Add(posTemp);
            }
        }
        else{
            int tmpX=x, tmpY=y;
            Vector3 tmp = origin+new Vector3(tmpX, tmpY, 0)*sdata.gridCellSize;
            while(CheckIfInsideEnv(tmp)){
                points.Add(tmp);
                tmpX+=x;
                tmpY+=y;
                tmp = origin+new Vector3(tmpX, tmpY, 0)*sdata.gridCellSize;
                if(x==0 && y==0){
                    break;
                }
            }
        }
        return points;
    }

    private bool CheckIfInsideEnv(Vector3 tmp){
        return Vector3.Distance(Vector3.zero, tmp)<=maxRadius;
    }

    private RaycastHit2D RaycastToDirection(Vector3 origin, int x, int y, LayerMask LayersFiltered){
        Vector3 dir = new Vector3(x,y,0f);
        RaycastHit2D hit;
        hit=Physics2D.Raycast(origin, dir, Mathf.Infinity, ~LayersFiltered);
        return hit;
    }

    private int GetNumOfPoints(float distance, int x, int y, Vector3 origin){
        int NumOfPoints=0;
        if(x==0 || y==0){
            NumOfPoints = Mathf.FloorToInt(distance/vhStep);
        }
        else{
            NumOfPoints = Mathf.FloorToInt(distance/slStep);
        }
        Vector3 initialLastPoint = origin+(new Vector3(x, y, 0)*vhStep*NumOfPoints);
        
        NumOfPoints = CheckOutOfCircle(initialLastPoint) ? NumOfPoints-1 : NumOfPoints;
        return NumOfPoints;
    }

    private bool CheckHitGold(RaycastHit2D hit){
        return hit.distance>0 && hit.collider.gameObject.CompareTag("Gold");
    }

    private bool CheckOutOfCircle(Vector3 posTemp){
        return Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
    }

    private void CreateLineRenderer(Color color, float width, Vector3 lastPoint, Vector3 origin, GameObject Parent){
        GameObject gl = Instantiate(lineRendererObject) as GameObject;
        gl.transform.SetParent(Parent.transform);
        LineRenderer lRend = gl.GetComponent<LineRenderer>();
        lRend.startColor = color;
        lRend.endColor = color;                    
        lRend.startWidth = width;
        lRend.endWidth = width;
        lRend.SetPosition(0, origin);
        lRend.SetPosition(1, lastPoint); 
    }

    private void CreateCircleMarker(Vector3 posTemp, GameObject TempCircleMarker){
        GameObject cr0 = Instantiate(TempCircleMarker) as GameObject;
        cr0.transform.SetParent(markersParent.transform);
        cr0.transform.position =  posTemp;
        cr0.transform.position = new Vector3(cr0.transform.position.x, cr0.transform.position.y, circleMarkerZoffset);
    }



    // ============================================================================
    public void InstTarget(Vector3 pos0){
        DestroyPriorTargetObject();
        DestroyMarkers();

        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="0"){
            SetSelectedCharacter(pos0);
        }

        else if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
            SetFinalMoveMarker(pos0);
        }

        else if (CheckFireExceptSword()){
            SetFinalFireTarget(pos0);
        }

    }

    private void DestroyPriorTargetObject(){
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
        }
    }

    private void SetSelectedCharacter(Vector3 pos0){
        List<Vector3> charactersPositions = GetSelectionMarkers(sdata.playerIndex);
        for(int i = 0; i<sdata.charactersNum; i++){
            if(pos0==charactersPositions[i]){
                sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].cindex = i;
                break;
            }
        }
    }

    private void SetFinalMoveMarker(Vector3 pos0){
        GameObject x = CopyMyCharacterSprite(pos0);
        CreateLineRenderer(moveLineFinalMarkerColor, 0.1f, pos0, currentPosition, x);
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
    }

    private void SetFinalFireTarget(Vector3 pos0){
        GameObject x = CreateFireTarget(pos0);
        CreateLineRenderer(fireLineFinalMarkerColor, 0.1f, pos0, currentPosition, x);
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
    }
    private GameObject CopyMyCharacterSprite(Vector3 pos0){
        GameObject x = Instantiate(myCharacter) as GameObject;
        Destroy(x.GetComponent<BoxCollider2D>());
        Destroy(x.GetComponent<Rigidbody2D>());
        DestoyMyChilds(x);
        x.transform.position = pos0;
        float rotz = sdata.playerIndex*(-90);
        x.transform.rotation = Quaternion.Euler( 0, 0, rotz);

        x.transform.SetParent(targetsParent.transform);
        x.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);
        return x;
    }

    private void DestoyMyChilds(GameObject x){
        foreach(Transform child in x.transform)
        {
            Destroy(child.gameObject);
        }   
    }

    private GameObject CreateFireTarget(Vector3 pos){
        GameObject x = Instantiate(fireTarget) as GameObject;
        x.transform.position = pos;
        return x;
    }

    private bool CheckFireExceptSword(){
        bool isFire = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire";
        bool isSword = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name=="Sword";
        return isFire && !isSword;
    }

}
