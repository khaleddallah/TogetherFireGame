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
    [SerializeField] private LayerMask moveLayersFiltered;
    [SerializeField] private LayerMask fireLayersFiltered;
    [SerializeField] private Color fireLinesTempMarkerColor;
    [SerializeField] private Color fireLineFinalMarkerColor;
    [SerializeField] private Color moveLinesTempMarkerColor;
    [SerializeField] private Color moveLineFinalMarkerColor;
    [SerializeField] private GameObject swordTargetMarker;
    [SerializeField] private float circleMarkerZoffset = -0.03f;

    
    public static TargetAssignHelper tah;
    public float radiousEnv;

    private const int MOVE_ACTION_INDEX = 0;
    private Sdata sdata;
    private Vector3 currentPosition;
    private GameObject myCharacter;
    private float vhStep;
    private float slStep;


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
        myCharacter = charactersParent.transform.GetChild(sdata.playerIndex).transform.GetChild(0).gameObject;
        vhStep = sdata.gridCellSize;
        slStep = Mathf.Sqrt(2*Mathf.Pow(vhStep,2));
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
        DrawPointsOnAllGrid();
    }

    private void DrawPointsOnAllGrid(){
        float NumLines = Mathf.Floor(sdata.maxRadius/sdata.gridCellSize);
        for (int i=-(int)NumLines ; i<NumLines; i++ ){
            for (int j=-(int)NumLines ; j<NumLines; j++ ){
                Vector3 posTemp = new Vector3(i*sdata.gridCellSize, j*sdata.gridCellSize, 0.0f);
                bool iscurrentPosition = i*sdata.gridCellSize==currentPosition.x && j*sdata.gridCellSize==currentPosition.y ; 
                bool isOutsideEnv = Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
                if(iscurrentPosition || isOutsideEnv){
                    continue;
                }
                else{
                    GameObject cr = Instantiate(circleGrenadeMarker) as GameObject;
                    cr.transform.SetParent(markersParent.transform);
                    cr.transform.position = new Vector3(i*sdata.gridCellSize, j*sdata.gridCellSize, -0.03f);
                }
            }
        }
    }



    // ======= Pistol =========
    public void DrawPistolMarkers(){
        DestroyMarkers();
        SetCurrentPositionAfterMove();
        DrawPistolLinesPointsMarkers();
    }

    private void DrawPistolLinesPointsMarkers(){
        // Draw lines & points
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                distance = RaycastToDirection(x, y, fireLayersFiltered);
                if(distance>vhStep){
                    Vector3 lastPoint = currentPosition+(new Vector3(x, y, 0)*vhStep);
                    CreateLineRenderer(fireLinesTempMarkerColor, 0.05f, lastPoint, markersParent);
                    CreateArrowMarker(x, y, lastPoint);
                    
                }
            }
        }
    }

    private void CreateArrowMarker(int x, int y, Vector3 lastPoint){
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
        currentPosition = myCharacter.transform.position;
        MoveLinesPointsMarkers();
    }

    private void DrawMoveLinesPointsMarkers(){
        CreateCircleMarker(currentPosition, circleMarker);
        for(int x=-1 ; x<=1 ; x++){
            for(int y=-1 ; y<=1 ; y++){
                PutMoveMarkersOnDirection(x,y);
            }
        }
    }

    private void PutMoveMarkersOnDirection(int x, int y){
        float distance = RaycastToDirection(x, y, moveLayersFiltered);
        distance += isHitGold() ? vhStep : 0.0f; 
        if(distance>vhStep){
            float NumOfPoints = GetNumOfPoints(distance, x, y);
            Vecotr3 lastPoint = currentPosition+new Vector3(x, y, 0)*(NumOfPoints-1)*vhStep;
            CreateLineRenderer(moveLinesTempMarkerColor, 0.05f, lastPoint, markersParent);

            for(int p=1; p<=NumOfPoints; p++){
                Vector3 posTemp = currentPosition+new Vector3(x, y, 0)*p*vhStep;
                CreateCircleMarker(posTemp, circleMarker);
            }
        }
    }

    private float RaycastToDirection(int x, int y, LayerMask LayersFiltered){
        Vector3 dir = new Vector3(x,y,0f);
        RaycastHit2D hit;
        hit=Physics2D.Raycast(currentPosition, dir, Mathf.Infinity, ~LayersFiltered);
        return hit.distance;
    }

    private int GetNumOfPoints(float distance, int x, int y){
        int NumOfPoints=0;
        if(x==0 || y==0){
            NumOfPoints = Mathf.Floor(distance/vhStep);
        }
        else{
            NumOfPoints = Mathf.Floor(distance/slStep);
        }
        Vector3 initialLastPoint = currentPosition+(new Vector3(x, y, 0)*vhStep*NumOfPoints);
        NumOfPoints = isOutOfCircle(initialLastPoint) ? NumOfPoints-1 : NumOfPoints;
        return NumOfPoints;
    }

    private bool isHitGold(){
        return hit.distance>0 && hit.collider.gameObject.CompareTag("Gold");
    }

    private bool isOutOfCircle(Vector3 posTemp){
        return Vector3.Distance(Vector3.zero, posTemp)>=radiousEnv;
    }

    private void CreateLineRenderer(Color color, float width, Vector3 lastPoint, GameObject Parent){
        GameObject gl = Instantiate(lineRendererObject) as GameObject;
        gl.transform.SetParent(Parent.transform);
        LineRenderer lRend = gl.GetComponent<LineRenderer>();
        lRend.startColor = color;
        lRend.endColor = color;                    
        lRend.startWidth = width;
        lRend.endWidth = width;
        lRend.SetPosition(0, currentPosition);
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

        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="move"){
            SetFinalMoveMarker(pos0);
        }

        else if (CheckIfFireAndNotSword()){
            SetFinalFireTarget(pos0);
        }

    }

    private void DestroyPriorTargetObject(){
        if(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj){
            Destroy(sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj);
        }
    }

    private void SetFinalMoveMarker(Vector3 pos0){
        GameObject x = CopyMyCharacterSprite(pos0);
        CreateLineRenderer(moveLineFinalMarkerColor, 0.1f, pos0, x)
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
    }

    private void SetFinalFireTarget(Vector3 pos0){
        GameObject x = CreateFireTarget(pos0);
        CreateLineRenderer(fireLineFinalMarkerColor, 0.1f, pos0, x);
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].targetObj = x;
        sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].target = pos0;
    }
    private GameObject CopyMyCharacterSprite(Vector3 pos0){
        GameObject x = Instantiate(myCharacter) as GameObject;
        Destroy(x.GetComponent<PlayerReaction>());
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

    private GameObect CreateFireTarget(Vector3 pos){
        GameObject x = Instantiate(fireTarget) as GameObject;
        x.transform.position = pos0;
        return x;
    }

    private bool CheckFireExceptSword(){
        isFire = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].type=="fire";
        isSword = sdata.episodes[sdata.episodeIndex].roleplays[sdata.playerIndex].actions[sdata.actionIndex].gunTypeObj.transform.name=="Sword"
        return isFire && !isSword;
    }

}
